using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using EcommerceWebsite.Services;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Controllers
{
    /// <summary>
    /// Handles all product browsing, filtering, and detail view operations
    /// </summary>
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Main product listing page with filtering, sorting, and pagination
        /// GET: /Product/Index
        /// </summary>
        public async Task<IActionResult> Index(int? categoryId, string searchTerm, string sortOrder, int page = 1)
        {
            const int pageSize = 12;
            
            // Get products based on filters
            var products = categoryId.HasValue 
                ? await _productService.GetProductsByCategoryAsync(categoryId.Value)
                : !string.IsNullOrWhiteSpace(searchTerm)
                    ? await _productService.SearchProductsAsync(searchTerm)
                    : await _productService.GetAllProductsAsync();

            // Apply sorting
            products = sortOrder switch
            {
                "price_asc" => products.OrderBy(p => p.FinalPrice),
                "price_desc" => products.OrderByDescending(p => p.FinalPrice),
                "name_asc" => products.OrderBy(p => p.Name),
                "name_desc" => products.OrderByDescending(p => p.Name),
                "newest" => products.OrderByDescending(p => p.CreatedDate),
                _ => products.OrderByDescending(p => p.CreatedDate)
            };

            var productList = products.ToList();

            // Pagination
            var totalProducts = productList.Count;
            var paginatedProducts = productList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Build view model
            var viewModel = new ProductListViewModel
            {
                Products = paginatedProducts,
                Categories = await _categoryService.GetActiveCategoriesAsync(),
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                SortOrder = sortOrder,
                CurrentPage = page,
                PageSize = pageSize,
                TotalProducts = totalProducts
            };

            return View(viewModel);
        }

        /// <summary>
        /// Product detail page
        /// GET: /Product/Details/5
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _productService.GetRelatedProductsAsync(id, 4);

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }

        /// <summary>
        /// Browse products by category
        /// GET: /Product/Category/1
        /// </summary>
        public async Task<IActionResult> Category(int id, string sortOrder, int page = 1)
        {
            return await Index(id, null, sortOrder, page);
        }

        /// <summary>
        /// Search products
        /// GET: /Product/Search?term=laptop
        /// </summary>
        public async Task<IActionResult> Search(string term, string sortOrder, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return RedirectToAction(nameof(Index));
            }

            return await Index(null, term, sortOrder, page);
        }
    }
}
