using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Services
{
    /// <summary>
    /// Service interface defining product-related operations
    /// Promotes loose coupling and testability
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 6);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4);
    }
}
