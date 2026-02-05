using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Core product entity representing items available for browsing
    /// </summary>
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string SKU { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        // Computed property
        [NotMapped]
        public decimal FinalPrice
        {
            get
            {
                if (DiscountPercentage.HasValue && DiscountPercentage.Value > 0)
                {
                    return Price - (Price * DiscountPercentage.Value / 100);
                }
                return Price;
            }
        }

        [NotMapped]
        public bool IsInStock => StockQuantity > 0;
    }
}
