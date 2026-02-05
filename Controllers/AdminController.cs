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
    /// Administrative functions for platform management
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.Active),
                PendingVerifications = await _context.UserDocuments.CountAsync(d => d.Status == DocumentStatus.Pending),
                ActiveOrders = await _context.Orders.CountAsync(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled),
                OpenDisputes = await _context.Disputes.CountAsync(d => d.Status == DisputeStatus.Open),
                EscrowFunds = await _context.Orders
                    .Where(o => o.FundsHeld && o.PaymentStatus == PaymentStatus.Completed)
                    .SumAsync(o => o.TotalAmount)
            };

            ViewBag.Stats = stats;

            // Recent activities
            var recentOrders = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .OrderByDescending(o => o.CreatedDate)
                .Take(10)
                .ToListAsync();

            return View(recentOrders);
        }

        #region User Management

        // GET: /Admin/Users
        public async Task<IActionResult> Users(string search = "", string status = "all")
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => 
                    u.Email.Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));
            }

            if (status == "verified")
            {
                query = query.Where(u => u.IsFullyVerified);
            }
            else if (status == "unverified")
            {
                query = query.Where(u => !u.IsFullyVerified);
            }
            else if (status == "suspended")
            {
                query = query.Where(u => u.IsSuspended);
            }

            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;
            return View(users);
        }

        // GET: /Admin/UserDetails/id
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _context.Users
                .Include(u => u.Documents)
                .Include(u => u.Listings)
                .Include(u => u.OrdersAsBuyer)
                .Include(u => u.OrdersAsSeller)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            return View(user);
        }

        // POST: /Admin/SuspendUser
        [HttpPost]
        public async Task<IActionResult> SuspendUser(string userId, string reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            user.IsSuspended = true;
            user.SuspensionReason = reason;

            await _userManager.UpdateAsync(user);

            // Send notification
            await _emailService.SendEmailAsync(
                user.Email,
                "Account Suspended",
                $"Your account has been suspended. Reason: {reason}. Contact support for assistance."
            );

            return Json(new { success = true, message = "User suspended successfully" });
        }

        // POST: /Admin/ReinstateUser
        [HttpPost]
        public async Task<IActionResult> ReinstateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            user.IsSuspended = false;
            user.SuspensionReason = null;

            await _userManager.UpdateAsync(user);

            await _emailService.SendEmailAsync(
                user.Email,
                "Account Reinstated",
                "Your account has been reinstated. You can now access all features."
            );

            return Json(new { success = true, message = "User reinstated successfully" });
        }

        #endregion

        #region Document Verification

        // GET: /Admin/PendingVerifications
        public async Task<IActionResult> PendingVerifications()
        {
            var documents = await _context.UserDocuments
                .Include(d => d.User)
                .Where(d => d.Status == DocumentStatus.Pending || d.Status == DocumentStatus.UnderReview)
                .OrderBy(d => d.UploadedDate)
                .ToListAsync();

            return View(documents);
        }

        // GET: /Admin/ReviewDocument/5
        public async Task<IActionResult> ReviewDocument(int id)
        {
            var document = await _context.UserDocuments
                .Include(d => d.User)
                    .ThenInclude(u => u.Documents)
                .FirstOrDefaultAsync(d => d.DocumentId == id);

            if (document == null)
            {
                TempData["Error"] = "Document not found.";
                return RedirectToAction(nameof(PendingVerifications));
            }

            return View(document);
        }

        // POST: /Admin/ApproveDocument
        [HttpPost]
        public async Task<IActionResult> ApproveDocument(int documentId, string notes)
        {
            var admin = await _userManager.GetUserAsync(User);
            var document = await _context.UserDocuments
                .Include(d => d.User)
                    .ThenInclude(u => u.Documents)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (document == null)
                return Json(new { success = false, message = "Document not found" });

            document.Status = DocumentStatus.Approved;
            document.ReviewedDate = DateTime.UtcNow;
            document.ReviewedBy = admin.Id;
            document.AdminNotes = notes;

            // Check if user is now fully verified
            var user = document.User;
            var hasRequiredDocs = await _context.UserDocuments
                .Where(d => d.UserId == user.Id && 
                           d.Status == DocumentStatus.Approved &&
                           (d.DocumentType == DocumentType.NationalID || 
                            d.DocumentType == DocumentType.Passport))
                .AnyAsync();

            if (hasRequiredDocs && user.IsEmailVerified)
            {
                user.IsDocumentVerified = true;
                user.VerifiedDate = DateTime.UtcNow;

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Account Verified",
                    "Congratulations! Your account has been fully verified. You can now create listings and make purchases."
                );
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Document approved" });
        }

        // POST: /Admin/RejectDocument
        [HttpPost]
        public async Task<IActionResult> RejectDocument(int documentId, string reason)
        {
            var admin = await _userManager.GetUserAsync(User);
            var document = await _context.UserDocuments
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (document == null)
                return Json(new { success = false, message = "Document not found" });

            document.Status = DocumentStatus.Rejected;
            document.ReviewedDate = DateTime.UtcNow;
            document.ReviewedBy = admin.Id;
            document.RejectionReason = reason;

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                document.User.Email,
                "Document Rejected",
                $"Your {document.DocumentType} document was rejected. Reason: {reason}. Please upload a new document."
            );

            return Json(new { success = true, message = "Document rejected" });
        }

        #endregion

        #region Fund Management

        // GET: /Admin/EscrowManagement
        public async Task<IActionResult> EscrowManagement()
        {
            var escrowOrders = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .Where(o => o.FundsHeld && o.PaymentStatus == PaymentStatus.Completed)
                .OrderByDescending(o => o.DeliveredDate ?? o.ShippedDate ?? o.PaidDate)
                .ToListAsync();

            // Identify orders eligible for auto-release
            var autoReleaseEligible = escrowOrders
                .Where(o => o.ShouldAutoReleaseFunds)
                .ToList();

            ViewBag.AutoReleaseEligible = autoReleaseEligible;
            return View(escrowOrders);
        }

        // POST: /Admin/ReleaseFunds
        [HttpPost]
        public async Task<IActionResult> AdminReleaseFunds(int orderId, string notes)
        {
            var admin = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (!order.FundsHeld)
                return Json(new { success = false, message = "Funds already released" });

            // Release funds
            order.FundsHeld = false;
            order.FundsReleasedDate = DateTime.UtcNow;
            order.Status = OrderStatus.Completed;

            foreach (var payment in order.Payments.Where(p => p.IsEscrow && !p.IsReleased))
            {
                payment.IsReleased = true;
                payment.EscrowReleaseDate = DateTime.UtcNow;
            }

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Completed,
                UpdatedBy = admin.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Funds released by admin. {notes}"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                order.Seller.Email,
                "Payment Released",
                $"Funds for order #{order.OrderNumber} have been released to your account."
            );

            return Json(new { success = true, message = "Funds released successfully" });
        }

        // POST: /Admin/ProcessAutoRelease
        [HttpPost]
        public async Task<IActionResult> ProcessAutoRelease()
        {
            var admin = await _userManager.GetUserAsync(User);
            var eligibleOrders = await _context.Orders
                .Include(o => o.Seller)
                .Include(o => o.Payments)
                .Where(o => o.FundsHeld && 
                           o.AutoReleaseEnabled &&
                           o.DeliveredDate != null &&
                           o.PaymentStatus == PaymentStatus.Completed)
                .ToListAsync();

            var releasedCount = 0;
            foreach (var order in eligibleOrders)
            {
                if (order.ShouldAutoReleaseFunds)
                {
                    order.FundsHeld = false;
                    order.FundsReleasedDate = DateTime.UtcNow;
                    order.Status = OrderStatus.Completed;

                    foreach (var payment in order.Payments.Where(p => p.IsEscrow && !p.IsReleased))
                    {
                        payment.IsReleased = true;
                        payment.EscrowReleaseDate = DateTime.UtcNow;
                    }

                    var statusHistory = new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        Status = OrderStatus.Completed,
                        UpdatedBy = admin.Id,
                        UpdatedDate = DateTime.UtcNow,
                        Notes = "Funds auto-released after delivery period"
                    };

                    _context.OrderStatusHistory.Add(statusHistory);

                    await _emailService.SendEmailAsync(
                        order.Seller.Email,
                        "Payment Released",
                        $"Funds for order #{order.OrderNumber} have been automatically released."
                    );

                    releasedCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"Released funds for {releasedCount} order(s)" 
            });
        }

        #endregion

        #region Dispute Management

        // GET: /Admin/Disputes
        public async Task<IActionResult> Disputes(string status = "open")
        {
            var query = _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Seller)
                .Include(d => d.Initiator)
                .AsQueryable();

            if (status == "open")
            {
                query = query.Where(d => d.Status == DisputeStatus.Open || 
                                        d.Status == DisputeStatus.UnderReview);
            }
            else if (status == "resolved")
            {
                query = query.Where(d => d.Status == DisputeStatus.Resolved);
            }

            var disputes = await query
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();

            ViewBag.Status = status;
            return View(disputes);
        }

        // GET: /Admin/DisputeDetails/5
        public async Task<IActionResult> DisputeDetails(int id)
        {
            var dispute = await _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Seller)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Listing)
                .Include(d => d.Order)
                    .ThenInclude(o => o.StatusHistory)
                .Include(d => d.Initiator)
                .FirstOrDefaultAsync(d => d.DisputeId == id);

            if (dispute == null)
            {
                TempData["Error"] = "Dispute not found.";
                return RedirectToAction(nameof(Disputes));
            }

            return View(dispute);
        }

        // POST: /Admin/ResolveDispute
        [HttpPost]
        public async Task<IActionResult> ResolveDispute(
            int disputeId, 
            DisputeOutcome outcome, 
            string resolution,
            decimal? refundAmount)
        {
            var admin = await _userManager.GetUserAsync(User);
            var dispute = await _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Seller)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Payments)
                .Include(d => d.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Listing)
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId);

            if (dispute == null)
                return Json(new { success = false, message = "Dispute not found" });

            var order = dispute.Order;

            dispute.Status = DisputeStatus.Resolved;
            dispute.ResolvedDate = DateTime.UtcNow;
            dispute.ResolvedBy = admin.Id;
            dispute.Outcome = outcome;
            dispute.Resolution = resolution;

            // Handle outcomes
            switch (outcome)
            {
                case DisputeOutcome.BuyerFavorable:
                    // Full refund
                    dispute.RefundAmount = order.TotalAmount;
                    order.Status = OrderStatus.Refunded;
                    order.PaymentStatus = PaymentStatus.Refunded;
                    
                    foreach (var payment in order.Payments)
                    {
                        payment.Status = PaymentStatus.Refunded;
                    }

                    // Restore listing quantities
                    foreach (var item in order.Items)
                    {
                        if (item.Listing != null)
                        {
                            item.Listing.Quantity += item.Quantity;
                        }
                    }

                    await _emailService.SendEmailAsync(
                        order.Buyer.Email,
                        "Dispute Resolved - Refund Issued",
                        $"Your dispute for order #{order.OrderNumber} has been resolved in your favor. Full refund issued."
                    );
                    break;

                case DisputeOutcome.SellerFavorable:
                    // Release funds to seller
                    order.FundsHeld = false;
                    order.FundsReleasedDate = DateTime.UtcNow;
                    order.Status = OrderStatus.Completed;

                    foreach (var payment in order.Payments.Where(p => !p.IsReleased))
                    {
                        payment.IsReleased = true;
                        payment.EscrowReleaseDate = DateTime.UtcNow;
                    }

                    await _emailService.SendEmailAsync(
                        order.Seller.Email,
                        "Dispute Resolved - Funds Released",
                        $"The dispute for order #{order.OrderNumber} has been resolved in your favor. Funds released."
                    );
                    break;

                case DisputeOutcome.PartialRefund:
                    if (refundAmount.HasValue)
                    {
                        dispute.RefundAmount = refundAmount.Value;
                        order.PaymentStatus = PaymentStatus.PartiallyRefunded;

                        await _emailService.SendEmailAsync(
                            order.Buyer.Email,
                            "Dispute Resolved - Partial Refund",
                            $"Dispute for order #{order.OrderNumber} resolved. Partial refund of R{refundAmount:F2} issued."
                        );
                    }
                    break;

                case DisputeOutcome.NoAction:
                    // Keep funds in escrow or maintain current status
                    break;
            }

            // Add order status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = order.Status,
                UpdatedBy = admin.Id,
                UpdatedDate = DateTime.UtcNow,
                Notes = $"Dispute resolved: {outcome}. {resolution}"
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Dispute resolved successfully" });
        }

        #endregion

        #region Listing Management

        // GET: /Admin/Listings
        public async Task<IActionResult> Listings(string status = "all")
        {
            var query = _context.Listings
                .Include(l => l.Seller)
                .Include(l => l.Category)
                .Include(l => l.Images)
                .AsQueryable();

            if (status != "all")
            {
                if (Enum.TryParse<ListingStatus>(status, true, out var listingStatus))
                {
                    query = query.Where(l => l.Status == listingStatus);
                }
            }

            var listings = await query
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();

            ViewBag.Status = status;
            return View(listings);
        }

        // POST: /Admin/SuspendListing
        [HttpPost]
        public async Task<IActionResult> SuspendListing(int listingId, string reason)
        {
            var listing = await _context.Listings
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.ListingId == listingId);

            if (listing == null)
                return Json(new { success = false, message = "Listing not found" });

            listing.Status = ListingStatus.Suspended;
            listing.IsActive = false;
            listing.RejectionReason = reason;

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                listing.Seller.Email,
                "Listing Suspended",
                $"Your listing '{listing.Title}' has been suspended. Reason: {reason}"
            );

            return Json(new { success = true, message = "Listing suspended" });
        }

        #endregion

        #region Reports

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            var report = new
            {
                Revenue = await _context.Orders
                    .Where(o => o.CreatedDate >= startDate && 
                               o.PaymentStatus == PaymentStatus.Completed)
                    .SumAsync(o => o.TotalAmount),
                
                OrderCount = await _context.Orders
                    .CountAsync(o => o.CreatedDate >= startDate),
                
                NewUsers = await _context.Users
                    .CountAsync(u => u.CreatedDate >= startDate),
                
                NewListings = await _context.Listings
                    .CountAsync(l => l.CreatedDate >= startDate),
                
                CompletedOrders = await _context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Completed && 
                                    o.CreatedDate >= startDate),
                
                DisputeRate = await CalculateDisputeRate(startDate, endDate)
            };

            return View(report);
        }

        private async Task<double> CalculateDisputeRate(DateTime start, DateTime end)
        {
            var totalOrders = await _context.Orders
                .CountAsync(o => o.CreatedDate >= start && o.CreatedDate <= end);

            if (totalOrders == 0) return 0;

            var disputedOrders = await _context.Disputes
                .CountAsync(d => d.CreatedDate >= start && d.CreatedDate <= end);

            return (double)disputedOrders / totalOrders * 100;
        }

        #endregion
    }
}
