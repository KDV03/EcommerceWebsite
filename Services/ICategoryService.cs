using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Services
{
    /// <summary>
    /// Service interface for category-related operations
    /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }
}
