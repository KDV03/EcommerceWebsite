using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// User review for completed orders
    /// </summary>
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int? ListingId { get; set; }

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public ApplicationUser Buyer { get; set; }

        [Required]
        public string SellerId { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsVerifiedPurchase { get; set; } = true;

        // Seller can respond
        [StringLength(2000)]
        public string SellerResponse { get; set; }

        public DateTime? SellerResponseDate { get; set; }

        // Moderation
        public bool IsApproved { get; set; } = true;
        public bool IsFlagged { get; set; }

        [StringLength(500)]
        public string FlagReason { get; set; }
    }

    /// <summary>
    /// Order dispute/complaint
    /// </summary>
    public class Dispute
    {
        [Key]
        public int DisputeId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        public string InitiatedBy { get; set; }

        [ForeignKey("InitiatedBy")]
        public ApplicationUser Initiator { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        public DisputeStatus Status { get; set; } = DisputeStatus.Open;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedDate { get; set; }

        [StringLength(2000)]
        public string Resolution { get; set; }

        public string ResolvedBy { get; set; }

        [ForeignKey("ResolvedBy")]
        public ApplicationUser Resolver { get; set; }

        // Evidence
        [StringLength(500)]
        public string EvidenceUrl { get; set; }

        // Outcome
        public DisputeOutcome? Outcome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }

        [StringLength(1000)]
        public string AdminNotes { get; set; }
    }

    /// <summary>
    /// Dispute status
    /// </summary>
    public enum DisputeStatus
    {
        Open = 0,
        UnderReview = 1,
        AwaitingEvidence = 2,
        Resolved = 3,
        Closed = 4,
        Escalated = 5
    }

    /// <summary>
    /// Dispute resolution outcome
    /// </summary>
    public enum DisputeOutcome
    {
        BuyerFavorable = 1,
        SellerFavorable = 2,
        PartialRefund = 3,
        NoAction = 4
    }
}
