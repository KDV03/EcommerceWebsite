using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Models;
using EcommerceWebsite.Models.ViewModels;
using EcommerceWebsite.Services;

namespace EcommerceWebsite.Controllers
{
    /// <summary>
    /// Handles checkout process and payment initiation
    /// </summary>
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /Checkout/Listing/5
        public async Task<IActionResult> Listing(int id, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Seller)
                .Include(l => l.Category)
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.Status == ListingStatus.Active);

            if (listing == null)
            {
                TempData["Error"] = "Listing not found or no longer available.";
                return RedirectToAction("Index", "Product");
            }

            // Prevent buying own listing
            if (listing.SellerId == user.Id)
            {
                TempData["Error"] = "You cannot purchase your own listing.";
                return RedirectToAction("Details", "Listing", new { id });
            }

            // Validate quantity
            if (quantity < 1 || quantity > listing.Quantity)
            {
                TempData["Error"] = "Invalid quantity.";
                return RedirectToAction("Details", "Listing", new { id });
            }

            var viewModel = new CheckoutViewModel
            {
                Listing = listing,
                Quantity = quantity,
                BuyerAddress = user.AddressLine1,
                BuyerCity = user.City,
                BuyerProvince = user.Province,
                BuyerPostalCode = user.PostalCode,
                BuyerPhone = user.PhoneNumber
            };

            // Calculate totals
            viewModel.SubTotal = listing.Price * quantity;
            viewModel.ShippingCost = CalculateShipping(listing, user);
            viewModel.TaxAmount = viewModel.SubTotal * 0.15m; // 15% VAT
            viewModel.TotalAmount = viewModel.SubTotal + viewModel.ShippingCost + viewModel.TaxAmount;

            return View(viewModel);
        }

        // POST: /Checkout/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.ListingId == model.ListingId);

            if (listing == null || listing.Status != ListingStatus.Active)
            {
                TempData["Error"] = "Listing is no longer available.";
                return RedirectToAction("Index", "Product");
            }

            if (listing.SellerId == user.Id)
            {
                TempData["Error"] = "You cannot purchase your own listing.";
                return RedirectToAction("Details", "Listing", new { id = listing.ListingId });
            }

            if (model.Quantity > listing.Quantity)
            {
                TempData["Error"] = "Insufficient stock available.";
                return RedirectToAction("Listing", new { id = listing.ListingId });
            }

            if (ModelState.IsValid)
            {
                // Create order
                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    BuyerId = user.Id,
                    SellerId = listing.SellerId,
                    SubTotal = listing.Price * model.Quantity,
                    TaxAmount = (listing.Price * model.Quantity) * 0.15m,
                    ShippingCost = model.ShippingCost,
                    TotalAmount = model.TotalAmount,
                    Currency = "ZAR",
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    ShippingAddress = model.ShippingAddress,
                    ShippingCity = model.ShippingCity,
                    ShippingProvince = model.ShippingProvince,
                    ShippingPostalCode = model.ShippingPostalCode,
                    ShippingCountry = "South Africa",
                    BuyerNotes = model.BuyerNotes,
                    CreatedDate = DateTime.UtcNow,
                    FundsHeld = true, // Escrow
                    AutoReleaseEnabled = true,
                    AutoReleaseDays = 7
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order item
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ListingId = listing.ListingId,
                    ProductName = listing.Title,
                    ProductDescription = listing.Description,
                    Quantity = model.Quantity,
                    UnitPrice = listing.Price,
                    TotalPrice = listing.Price * model.Quantity,
                    ProductImageUrl = listing.PrimaryImageUrl,
                    SKU = $"LST-{listing.ListingId}"
                };

                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                // Create order status history
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = OrderStatus.Pending,
                    UpdatedBy = user.Id,
                    UpdatedDate = DateTime.UtcNow,
                    Notes = "Order created"
                };

                _context.OrderStatusHistory.Add(statusHistory);
                await _context.SaveChangesAsync();

                // Redirect to payment gateway
                return RedirectToAction("Payment", new { orderId = order.OrderId });
            }

            // Reload listing data for view
            model.Listing = listing;
            return View("Listing", model);
        }

        // GET: /Checkout/Payment/5
        public async Task<IActionResult> Payment(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Listing)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == user.Id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            if (order.PaymentStatus != PaymentStatus.Pending)
            {
                TempData["Info"] = "Payment already processed.";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            var viewModel = new PaymentViewModel
            {
                Order = order,
                AvailablePaymentMethods = Enum.GetValues(typeof(PaymentMethod))
                    .Cast<PaymentMethod>()
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: /Checkout/InitiatePayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitiatePayment(int orderId, PaymentMethod paymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Seller)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == user.Id);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            // Create payment record
            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = order.TotalAmount,
                Currency = order.Currency,
                PaymentMethod = paymentMethod,
                TransactionId = GenerateTransactionId(),
                Status = PaymentStatus.Processing,
                CreatedDate = DateTime.UtcNow,
                IsEscrow = true,
                IsReleased = false
            };

            _context.Payments.Add(payment);

            // In a real implementation, integrate with payment gateway here
            // For now, simulate successful payment
            await Task.Delay(2000); // Simulate API call

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.ProcessedDate = DateTime.UtcNow;
            payment.GatewayResponse = "Simulated successful payment";

            // Update order status
            order.Status = OrderStatus.Confirmed;
            order.PaymentStatus = PaymentStatus.Completed;
            order.PaidDate = DateTime.UtcNow;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = OrderStatus.Confirmed,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Payment completed via {paymentMethod}"
            };

            _context.OrderStatusHistory.Add(statusHistory);

            // Update listing quantity
            foreach (var item in order.Items)
            {
                if (item.Listing != null)
                {
                    item.Listing.Quantity -= item.Quantity;
                    if (item.Listing.Quantity == 0)
                    {
                        item.Listing.Status = ListingStatus.Sold;
                        item.Listing.SoldDate = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Send confirmation emails
            await _emailService.SendEmailAsync(
                user.Email,
                "Order Confirmation",
                $"Your order #{order.OrderNumber} has been confirmed and payment received."
            );

            await _emailService.SendEmailAsync(
                order.Seller.Email,
                "New Order Received",
                $"You have received a new order #{order.OrderNumber}. Please prepare for shipment."
            );

            return Json(new
            {
                success = true,
                message = "Payment successful!",
                orderId = order.OrderId
            });
        }

        // GET: /Checkout/Success/5
        public async Task<IActionResult> Success(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Listing)
                        .ThenInclude(l => l.Images)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == user.Id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        #region Helper Methods

        private string GenerateOrderNumber()
        {
            var date = DateTime.UtcNow;
            var random = new Random().Next(1000, 9999);
            return $"ORD-{date:yyyyMMdd}-{random}";
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper()}";
        }

        private decimal CalculateShipping(Listing listing, ApplicationUser buyer)
        {
            // Simple shipping calculation
            // In production, integrate with shipping API

            // Same city: R50
            if (listing.City?.Equals(buyer.City, StringComparison.OrdinalIgnoreCase) == true)
                return 50m;

            // Same province: R100
            if (listing.Province?.Equals(buyer.Province, StringComparison.OrdinalIgnoreCase) == true)
                return 100m;

            // Different province: R150
            return 150m;
        }

        #endregion
    }
}
