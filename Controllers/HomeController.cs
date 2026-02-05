using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EcommerceWebsite.Services;
using EcommerceWebsite.Models;
using System.Diagnostics;

namespace EcommerceWebsite.Controllers
{
    /// <summary>
    /// Handles homepage and general site navigation
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Homepage displaying featured products and categories
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.GetFeaturedProductsAsync(8);
            var categories = await _categoryService.GetActiveCategoriesAsync();

            var viewModel = new
            {
                FeaturedProducts = featuredProducts,
                Categories = categories
            };

            return View(viewModel);
        }

        /// <summary>
        /// About page
        /// </summary>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Contact page
        /// </summary>
        public IActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Error handling page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }

    /// <summary>
    /// Error view model
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
