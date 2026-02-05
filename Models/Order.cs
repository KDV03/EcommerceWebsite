using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Customer order with items and payment tracking
    /// </summary>
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } // e.g., ORD-2026-00001

        [Required]
        public string BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public ApplicationUser Buyer { get; set; }

        public string SellerId { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; }

        // Pricing
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "ZAR";

        // Status tracking
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Shipping information
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [StringLength(100)]
        public string ShippingCity { get; set; }

        [StringLength(100)]
        public string ShippingProvince { get; set; }

        [StringLength(20)]
        public string ShippingPostalCode { get; set; }

        [StringLength(100)]
        public string ShippingCountry { get; set; }

        [StringLength(50)]
        public string TrackingNumber { get; set; }

        [StringLength(100)]
        public string CourierService { get; set; }

        // Timestamps
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaidDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        // Notes and communication
        [StringLength(1000)]
        public string BuyerNotes { get; set; }

        [StringLength(1000)]
        public string SellerNotes { get; set; }

        [StringLength(1000)]
        public string CancellationReason { get; set; }

        // Escrow management
        public bool FundsHeld { get; set; }
        public DateTime? FundsReleasedDate { get; set; }
        public bool AutoReleaseEnabled { get; set; } = true;
        public int AutoReleaseDays { get; set; } = 7;

        // Navigation properties
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        public Dispute Dispute { get; set; }

        // Computed properties
        [NotMapped]
        public bool CanCancel => Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;

        [NotMapped]
        public bool CanShip => Status == OrderStatus.Confirmed && PaymentStatus == PaymentStatus.Completed;

        [NotMapped]
        public bool CanConfirmDelivery => Status == OrderStatus.Shipped;

        [NotMapped]
        public int DaysSinceDelivery
        {
            get
            {
                if (DeliveredDate.HasValue)
                    return (DateTime.UtcNow - DeliveredDate.Value).Days;
                return 0;
            }
        }

        [NotMapped]
        public bool ShouldAutoReleaseFunds => 
            AutoReleaseEnabled && 
            DeliveredDate.HasValue && 
            DaysSinceDelivery >= AutoReleaseDays && 
            !FundsReleasedDate.HasValue;
    }

    /// <summary>
    /// Individual item in an order
    /// </summary>
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int? ListingId { get; set; }

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string ProductDescription { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [StringLength(500)]
        public string ProductImageUrl { get; set; }

        [StringLength(100)]
        public string SKU { get; set; }
    }

    /// <summary>
    /// Order status values
    /// </summary>
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Completed = 5,
        Cancelled = 6,
        Refunded = 7,
        Disputed = 8
    }

    /// <summary>
    /// Payment status values
    /// </summary>
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5
    }

    /// <summary>
    /// Order status change history
    /// </summary>
    public class OrderStatusHistory
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public OrderStatus Status { get; set; }

        public string UpdatedBy { get; set; }

        [ForeignKey("UpdatedBy")]
        public ApplicationUser UpdatedByUser { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Notes { get; set; }
    }
}
