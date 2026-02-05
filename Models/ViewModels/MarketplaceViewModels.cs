using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebsite.Models.ViewModels
{
    /// <summary>
    /// ViewModel for creating new listings
    /// </summary>
    public class CreateListingViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 5000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between R0.01 and R999,999.99")]
        [Display(Name = "Price (ZAR)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Location address is required")]
        [StringLength(200)]
        [Display(Name = "Location Address")]
        public string LocationAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required")]
        [StringLength(100)]
        [Display(Name = "Province")]
        public string Province { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        [StringLength(50)]
        [Display(Name = "Condition")]
        public string Condition { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity Available")]
        public int Quantity { get; set; }

        [StringLength(100)]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [StringLength(100)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Price is Negotiable")]
        public bool IsNegotiable { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }

    /// <summary>
    /// ViewModel for editing existing listings
    /// </summary>
    public class EditListingViewModel : CreateListingViewModel
    {
        public int ListingId { get; set; }
    }

    /// <summary>
    /// ViewModel for listing detail page
    /// </summary>
    public class ListingDetailViewModel
    {
        public Listing Listing { get; set; }
        public IEnumerable<Listing> OtherListings { get; set; }
    }

    /// <summary>
    /// ViewModel for checkout process
    /// </summary>
    public class CheckoutViewModel
    {
        public Listing Listing { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ListingId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Buyer information
        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(200)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string ShippingCity { get; set; }

        [Required(ErrorMessage = "Province is required")]
        [StringLength(100)]
        [Display(Name = "Province")]
        public string ShippingProvince { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string ShippingPostalCode { get; set; }

        [Phone]
        [Display(Name = "Contact Phone")]
        public string BuyerPhone { get; set; }

        [StringLength(1000)]
        [Display(Name = "Additional Notes (Optional)")]
        public string BuyerNotes { get; set; }

        // Pre-filled from user account
        public string BuyerAddress { get; set; }
        public string BuyerCity { get; set; }
        public string BuyerProvince { get; set; }
        public string BuyerPostalCode { get; set; }

        // Calculated amounts
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// ViewModel for payment processing
    /// </summary>
    public class PaymentViewModel
    {
        public Order Order { get; set; }
        public PaymentMethod SelectedPaymentMethod { get; set; }
        public List<PaymentMethod> AvailablePaymentMethods { get; set; }

        [Display(Name = "Save card for future use")]
        public bool SaveCard { get; set; }

        // Card details (for card payments)
        [StringLength(19)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; }

        [StringLength(5)]
        [Display(Name = "Expiry (MM/YY)")]
        public string ExpiryDate { get; set; }

        [StringLength(4)]
        [Display(Name = "CVV")]
        public string CVV { get; set; }
    }

    /// <summary>
    /// ViewModel for order summary
    /// </summary>
    public class OrderSummaryViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<OrderStatusHistory> StatusHistory { get; set; }
        public bool CanRate { get; set; }
        public bool CanDispute { get; set; }
        public bool CanConfirmDelivery { get; set; }
        public bool CanCancel { get; set; }
        public string UserRole { get; set; } // "buyer" or "seller"
    }

    /// <summary>
    /// ViewModel for dispute creation
    /// </summary>
    public class CreateDisputeViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Please select a reason")]
        [Display(Name = "Dispute Reason")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Please provide details")]
        [StringLength(2000, MinimumLength = 20)]
        [Display(Name = "Detailed Description")]
        public string Description { get; set; }

        [Display(Name = "Evidence (Optional)")]
        public string EvidenceUrl { get; set; }

        public Order Order { get; set; }
    }

    /// <summary>
    /// ViewModel for admin dashboard
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int VerifiedUsers { get; set; }
        public int ActiveListings { get; set; }
        public int PendingVerifications { get; set; }
        public int ActiveOrders { get; set; }
        public int OpenDisputes { get; set; }
        public decimal EscrowFunds { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public IEnumerable<Order> RecentOrders { get; set; }
        public IEnumerable<Dispute> RecentDisputes { get; set; }
        public IEnumerable<UserDocument> PendingDocuments { get; set; }
    }

    /// <summary>
    /// ViewModel for dispute resolution
    /// </summary>
    public class ResolveDisputeViewModel
    {
        [Required]
        public int DisputeId { get; set; }

        [Required]
        [Display(Name = "Outcome")]
        public DisputeOutcome Outcome { get; set; }

        [Required(ErrorMessage = "Resolution notes are required")]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Resolution Notes")]
        public string Resolution { get; set; }

        [Display(Name = "Refund Amount (if applicable)")]
        [Range(0, 999999.99)]
        public decimal? RefundAmount { get; set; }

        [StringLength(1000)]
        [Display(Name = "Admin Notes")]
        public string AdminNotes { get; set; }

        public Dispute Dispute { get; set; }
    }

    /// <summary>
    /// ViewModel for user verification
    /// </summary>
    public class DocumentVerificationViewModel
    {
        public UserDocument Document { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Decision is required")]
        public DocumentStatus Decision { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [StringLength(500)]
        [Display(Name = "Rejection Reason (if rejecting)")]
        public string RejectionReason { get; set; }
    }

    /// <summary>
    /// ViewModel for shipping information update
    /// </summary>
    public class UpdateShippingViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Tracking number is required")]
        [StringLength(50)]
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Required(ErrorMessage = "Courier service is required")]
        [StringLength(100)]
        [Display(Name = "Courier Service")]
        public string CourierService { get; set; }

        [Display(Name = "Estimated Delivery Date")]
        public DateTime? EstimatedDelivery { get; set; }

        [StringLength(500)]
        [Display(Name = "Shipping Notes")]
        public string Notes { get; set; }

        public Order Order { get; set; }
    }

    /// <summary>
    /// ViewModel for review/rating
    /// </summary>
    public class CreateReviewViewModel
    {
        [Required]
        public int OrderId { get; set; }

        public int? ListingId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating (1-5 stars)")]
        public int Rating { get; set; }

        [StringLength(2000)]
        [Display(Name = "Review Comment (Optional)")]
        public string Comment { get; set; }

        public Order Order { get; set; }
    }

    /// <summary>
    /// ViewModel for financial reports
    /// </summary>
    public class FinancialReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalRefunds { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal EscrowBalance { get; set; }
        public decimal ReleasedFunds { get; set; }

        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int DisputedOrders { get; set; }

        public Dictionary<string, decimal> RevenueByCategory { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }
        public List<TopSeller> TopSellers { get; set; }
    }

    public class TopSeller
    {
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
    }
}
