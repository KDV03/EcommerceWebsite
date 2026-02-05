using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Image associated with a listing
    /// </summary>
    public class ListingImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int ListingId { get; set; }

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(500)]
        public string ThumbnailUrl { get; set; }

        [Range(0, 100)]
        public int DisplayOrder { get; set; }

        public bool IsPrimary { get; set; }

        [StringLength(200)]
        public string AltText { get; set; }

        public long FileSizeBytes { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
