using System.Collections.Generic;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// ViewModel for product listing pages with filtering and pagination support
    /// </summary>
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalProducts { get; set; }
        public int TotalPages => (TotalProducts + PageSize - 1) / PageSize;
        
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    /// <summary>
    /// ViewModel for individual product details page
    /// </summary>
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; }
    }
}
