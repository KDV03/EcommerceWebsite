using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// User document for verification purposes
    /// </summary>
    public class UserDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; }

        [StringLength(100)]
        public string FileName { get; set; }

        public long FileSizeBytes { get; set; }

        [StringLength(50)]
        public string FileExtension { get; set; }

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedDate { get; set; }

        public string ReviewedBy { get; set; }

        [ForeignKey("ReviewedBy")]
        public ApplicationUser Reviewer { get; set; }

        [StringLength(500)]
        public string RejectionReason { get; set; }

        [StringLength(1000)]
        public string AdminNotes { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }

    /// <summary>
    /// Types of documents for verification
    /// </summary>
    public enum DocumentType
    {
        NationalID = 1,
        Passport = 2,
        DriversLicense = 3,
        ProofOfAddress = 4,
        BankStatement = 5,
        BusinessRegistration = 6
    }

    /// <summary>
    /// Document verification status
    /// </summary>
    public enum DocumentStatus
    {
        Pending = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Expired = 4
    }
}
