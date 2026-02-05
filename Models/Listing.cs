using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// User-created product listing for marketplace
    /// </summary>
    public class Listing
    {
        [Key]
        public int ListingId { get; set; }

        [Required]
        public string SellerId { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 9999999.99)]
        public decimal Price { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "ZAR";

        // Location information
        [Required]
        [StringLength(200)]
        public string LocationAddress { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Province { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Listing details
        [StringLength(50)]
        public string Condition { get; set; } // New, Used, Refurbished

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [StringLength(100)]
        public string Brand { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        // Status tracking
        public ListingStatus Status { get; set; } = ListingStatus.Draft;

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsNegotiable { get; set; }

        // Metrics
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }
        public int InquiryCount { get; set; }

        // Timestamps
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? SoldDate { get; set; }

        // Moderation
        public bool RequiresApproval { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectionReason { get; set; }

        // Navigation properties
        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Computed properties
        [NotMapped]
        public bool IsAvailable => Status == ListingStatus.Active && Quantity > 0;

        [NotMapped]
        public string PrimaryImageUrl => Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl 
                                         ?? Images?.FirstOrDefault()?.ImageUrl 
                                         ?? "/images/placeholder.jpg";

        [NotMapped]
        public int DaysActive
        {
            get
            {
                if (PublishedDate.HasValue)
                    return (DateTime.UtcNow - PublishedDate.Value).Days;
                return 0;
            }
        }
    }

    /// <summary>
    /// Listing status workflow
    /// </summary>
    public enum ListingStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Active = 2,
        Sold = 3,
        Expired = 4,
        Suspended = 5,
        Deleted = 6
    }
}
