using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Models;
using EcommerceWebsite.Services;

namespace EcommerceWebsite.Controllers
{
    /// <summary>
    /// Handles order management, tracking, and lifecycle
    /// </summary>
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public OrderController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /Order/MyOrders
        public async Task<IActionResult> MyOrders(string type = "purchases", string status = "all")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            IQueryable<Order> query;

            if (type == "sales")
            {
                // Orders where user is seller
                query = _context.Orders
                    .Include(o => o.Buyer)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Listing)
                            .ThenInclude(l => l.Images)
                    .Where(o => o.SellerId == user.Id);
            }
            else
            {
                // Orders where user is buyer
                query = _context.Orders
                    .Include(o => o.Seller)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Listing)
                            .ThenInclude(l => l.Images)
                    .Where(o => o.BuyerId == user.Id);
            }

            // Filter by status
            if (status != "all")
            {
                if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    query = query.Where(o => o.Status == orderStatus);
                }
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            ViewBag.OrderType = type;
            ViewBag.CurrentStatus = status;
            return View(orders);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Listing)
                        .ThenInclude(l => l.Images)
                .Include(o => o.Payments)
                .Include(o => o.StatusHistory)
                    .ThenInclude(h => h.UpdatedByUser)
                .Include(o => o.Dispute)
                .FirstOrDefaultAsync(o => o.OrderId == id && 
                    (o.BuyerId == user.Id || o.SellerId == user.Id));

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            ViewBag.UserRole = order.BuyerId == user.Id ? "buyer" : "seller";
            return View(order);
        }

        // POST: /Order/MarkAsShipped
        [HttpPost]
        public async Task<IActionResult> MarkAsShipped(int id, string trackingNumber, string courierService)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.SellerId == user.Id);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.Processing)
                return Json(new { success = false, message = "Order cannot be shipped in current status" });

            if (order.PaymentStatus != PaymentStatus.Completed)
                return Json(new { success = false, message = "Payment not completed" });

            // Update order
            order.Status = OrderStatus.Shipped;
            order.ShippedDate = DateTime.UtcNow;
            order.TrackingNumber = trackingNumber;
            order.CourierService = courierService;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = OrderStatus.Shipped,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Order shipped via {courierService}. Tracking: {trackingNumber}"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            // Send notification to buyer
            await _emailService.SendEmailAsync(
                order.Buyer.Email,
                "Order Shipped",
                $"Your order #{order.OrderNumber} has been shipped. Tracking number: {trackingNumber}"
            );

            return Json(new { success = true, message = "Order marked as shipped" });
        }

        // POST: /Order/ConfirmDelivery
        [HttpPost]
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.BuyerId == user.Id);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (order.Status != OrderStatus.Shipped)
                return Json(new { success = false, message = "Order must be shipped first" });

            // Update order
            order.Status = OrderStatus.Delivered;
            order.DeliveredDate = DateTime.UtcNow;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = OrderStatus.Delivered,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = "Delivery confirmed by buyer"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            // Send notification to seller
            await _emailService.SendEmailAsync(
                order.Seller.Email,
                "Delivery Confirmed",
                $"Order #{order.OrderNumber} has been confirmed as delivered. Funds will be released in 7 days unless disputed."
            );

            return Json(new { success = true, message = "Delivery confirmed" });
        }

        // POST: /Order/ReleaseFunds
        [HttpPost]
        public async Task<IActionResult> ReleaseFunds(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            // Only admin or system can release funds
            // For now, allow buyer to release early
            var order = await _context.Orders
                .Include(o => o.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.BuyerId == user.Id);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (order.Status != OrderStatus.Delivered)
                return Json(new { success = false, message = "Order must be delivered first" });

            if (!order.FundsHeld)
                return Json(new { success = false, message = "Funds already released" });

            // Release funds
            order.FundsHeld = false;
            order.FundsReleasedDate = DateTime.UtcNow;
            order.Status = OrderStatus.Completed;

            // Update payment records
            foreach (var payment in order.Payments.Where(p => p.IsEscrow && !p.IsReleased))
            {
                payment.IsReleased = true;
                payment.EscrowReleaseDate = DateTime.UtcNow;
            }

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = OrderStatus.Completed,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = "Funds released to seller"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            // Notify seller
            await _emailService.SendEmailAsync(
                order.Seller.Email,
                "Payment Released",
                $"Funds for order #{order.OrderNumber} have been released to your account."
            );

            return Json(new { success = true, message = "Funds released successfully" });
        }

        // POST: /Order/Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Listing)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id && 
                    (o.BuyerId == user.Id || o.SellerId == user.Id));

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (!order.CanCancel)
                return Json(new { success = false, message = "Order cannot be cancelled in current status" });

            // Update order
            order.Status = OrderStatus.Cancelled;
            order.CancelledDate = DateTime.UtcNow;
            order.CancellationReason = reason;

            // Restore listing quantities
            foreach (var item in order.Items)
            {
                if (item.Listing != null)
                {
                    item.Listing.Quantity += item.Quantity;
                    if (item.Listing.Status == ListingStatus.Sold)
                    {
                        item.Listing.Status = ListingStatus.Active;
                    }
                }
            }

            // Process refund if payment was made
            if (order.PaymentStatus == PaymentStatus.Completed)
            {
                // Update payment status
                foreach (var payment in order.Payments)
                {
                    payment.Status = PaymentStatus.Refunded;
                }
                order.PaymentStatus = PaymentStatus.Refunded;
                order.Status = OrderStatus.Refunded;
            }

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = order.Status,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Order cancelled. Reason: {reason}"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            // Send notifications
            var otherParty = order.BuyerId == user.Id ? order.Seller : order.Buyer;
            await _emailService.SendEmailAsync(
                otherParty.Email,
                "Order Cancelled",
                $"Order #{order.OrderNumber} has been cancelled. Reason: {reason}"
            );

            return Json(new { success = true, message = "Order cancelled successfully" });
        }

        // GET: /Order/CreateDispute/5
        public async Task<IActionResult> CreateDispute(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Dispute)
                .FirstOrDefaultAsync(o => o.OrderId == id && 
                    (o.BuyerId == user.Id || o.SellerId == user.Id));

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.Dispute != null)
            {
                TempData["Info"] = "Dispute already exists for this order.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(order);
        }

        // POST: /Order/SubmitDispute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDispute(int orderId, string reason, string description)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Dispute)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && 
                    (o.BuyerId == user.Id || o.SellerId == user.Id));

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(MyOrders));
            }

            if (order.Dispute != null)
            {
                TempData["Error"] = "Dispute already exists.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            // Create dispute
            var dispute = new Dispute
            {
                OrderId = orderId,
                InitiatedBy = user.Id,
                Reason = reason,
                Description = description,
                Status = DisputeStatus.Open,
                CreatedDate = DateTime.UtcNow
            };

            _context.Disputes.Add(dispute);

            // Update order status
            order.Status = OrderStatus.Disputed;

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Disputed,
                UpdatedBy = user.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Dispute opened: {reason}"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Dispute submitted. Our team will review it shortly.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        // GET: /Order/TrackingDetails/5
        public async Task<IActionResult> TrackingDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.StatusHistory)
                    .ThenInclude(h => h.UpdatedByUser)
                .FirstOrDefaultAsync(o => o.OrderId == id && 
                    (o.BuyerId == user.Id || o.SellerId == user.Id));

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            var timeline = order.StatusHistory
                .OrderBy(h => h.UpdatedDate)
                .Select(h => new
                {
                    status = h.Status.ToString(),
                    date = h.UpdatedDate.ToString("yyyy-MM-dd HH:mm"),
                    notes = h.Notes,
                    updatedBy = h.UpdatedByUser?.FullName ?? "System"
                })
                .ToList();

            return Json(new { success = true, timeline });
        }
    }
}
