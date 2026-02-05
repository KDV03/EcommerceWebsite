using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Category entity for organizing products into logical groups
    /// </summary>
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property - one category can have many products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
