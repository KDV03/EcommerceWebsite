using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Services
{
    /// <summary>
    /// Concrete implementation of product service with business logic
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.CategoryId == categoryId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllProductsAsync();
            }

            var normalizedSearch = searchTerm.ToLower().Trim();

            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && 
                    (p.Name.ToLower().Contains(normalizedSearch) ||
                     p.Description.ToLower().Contains(normalizedSearch) ||
                     p.SKU.ToLower().Contains(normalizedSearch)))
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 6)
        {
            // Featured products: those with discounts or recently added
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && (p.DiscountPercentage > 0 || p.StockQuantity > 0))
                .OrderByDescending(p => p.DiscountPercentage)
                .ThenByDescending(p => p.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var product = await GetProductByIdAsync(productId);
            if (product == null) return Enumerable.Empty<Product>();

            // Get products from same category, excluding current product
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && 
                    p.CategoryId == product.CategoryId && 
                    p.ProductId != productId)
                .OrderByDescending(p => p.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
