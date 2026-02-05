using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Payment transaction record
    /// </summary>
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "ZAR";

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string TransactionId { get; set; }

        [StringLength(100)]
        public string PaymentGatewayReference { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }
        public DateTime? EscrowReleaseDate { get; set; }

        [StringLength(1000)]
        public string PaymentDetails { get; set; }

        [StringLength(500)]
        public string FailureReason { get; set; }

        public bool IsEscrow { get; set; } = true;
        public bool IsReleased { get; set; }

        // Gateway response data
        [StringLength(2000)]
        public string GatewayResponse { get; set; }
    }

    /// <summary>
    /// Payment methods
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        EFT = 3,
        PayFast = 4,
        PayPal = 5,
        Stripe = 6,
        CashOnDelivery = 7
    }
}
