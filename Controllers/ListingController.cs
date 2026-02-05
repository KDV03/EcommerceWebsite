using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Models;
using EcommerceWebsite.Models.ViewModels;

namespace EcommerceWebsite.Controllers
{
    /// <summary>
    /// Handles seller listing creation, editing, and management
    /// </summary>
    [Authorize]
    public class ListingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ListingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: /Listing/MyListings
        public async Task<IActionResult> MyListings(string status = "all")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var query = _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Images)
                .Where(l => l.SellerId == user.Id);

            // Filter by status
            if (status != "all")
            {
                if (Enum.TryParse<ListingStatus>(status, true, out var listingStatus))
                {
                    query = query.Where(l => l.Status == listingStatus);
                }
            }

            var listings = await query
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(listings);
        }

        // GET: /Listing/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Check if user is verified
            if (!user.IsFullyVerified)
            {
                TempData["Error"] = "You must complete account verification before creating listings.";
                return RedirectToAction("Profile", "Account");
            }

            var viewModel = new CreateListingViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: /Listing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateListingViewModel model, IFormFileCollection images)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!user.IsFullyVerified)
            {
                TempData["Error"] = "Account verification required.";
                return RedirectToAction("Profile", "Account");
            }

            if (ModelState.IsValid)
            {
                // Create listing
                var listing = new Listing
                {
                    SellerId = user.Id,
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Price = model.Price,
                    Currency = "ZAR",
                    LocationAddress = model.LocationAddress,
                    City = model.City,
                    Province = model.Province,
                    PostalCode = model.PostalCode,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Condition = model.Condition,
                    Quantity = model.Quantity,
                    Brand = model.Brand,
                    Model = model.Model,
                    IsNegotiable = model.IsNegotiable,
                    Status = ListingStatus.Draft,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();

                // Handle image uploads
                if (images != null && images.Count > 0)
                {
                    await UploadListingImages(listing.ListingId, images);
                }

                TempData["Success"] = "Listing created successfully! Add photos to publish.";
                return RedirectToAction(nameof(EditPhotos), new { id = listing.ListingId });
            }

            model.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(model);
        }

        // GET: /Listing/EditPhotos/5
        public async Task<IActionResult> EditPhotos(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.SellerId == user.Id);

            if (listing == null) return NotFound();

            return View(listing);
        }

        // POST: /Listing/UploadPhoto
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int listingId, IFormFile photo)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.ListingId == listingId && l.SellerId == user.Id);

            if (listing == null)
                return Json(new { success = false, message = "Listing not found" });

            if (photo == null || photo.Length == 0)
                return Json(new { success = false, message = "No file uploaded" });

            // Validate file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Invalid file type" });

            if (photo.Length > 5 * 1024 * 1024) // 5MB
                return Json(new { success = false, message = "File too large (max 5MB)" });

            // Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listings", listingId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            // Create database record
            var imageCount = await _context.ListingImages.CountAsync(i => i.ListingId == listingId);
            var listingImage = new ListingImage
            {
                ListingId = listingId,
                ImageUrl = $"/uploads/listings/{listingId}/{uniqueFileName}",
                DisplayOrder = imageCount,
                IsPrimary = imageCount == 0, // First image is primary
                FileSizeBytes = photo.Length,
                UploadedDate = DateTime.UtcNow
            };

            _context.ListingImages.Add(listingImage);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                imageId = listingImage.ImageId,
                imageUrl = listingImage.ImageUrl,
                isPrimary = listingImage.IsPrimary
            });
        }

        // POST: /Listing/DeletePhoto
        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int imageId)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _context.ListingImages
                .Include(i => i.Listing)
                .FirstOrDefaultAsync(i => i.ImageId == imageId && i.Listing.SellerId == user.Id);

            if (image == null)
                return Json(new { success = false, message = "Image not found" });

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.ListingImages.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: /Listing/SetPrimaryPhoto
        [HttpPost]
        public async Task<IActionResult> SetPrimaryPhoto(int imageId)
        {
            var user = await _userManager.GetUserAsync(User);
            var image = await _context.ListingImages
                .Include(i => i.Listing)
                .FirstOrDefaultAsync(i => i.ImageId == imageId && i.Listing.SellerId == user.Id);

            if (image == null)
                return Json(new { success = false });

            // Remove primary flag from all other images
            var otherImages = await _context.ListingImages
                .Where(i => i.ListingId == image.ListingId && i.ImageId != imageId)
                .ToListAsync();

            foreach (var img in otherImages)
            {
                img.IsPrimary = false;
            }

            image.IsPrimary = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: /Listing/Publish
        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.SellerId == user.Id);

            if (listing == null)
                return Json(new { success = false, message = "Listing not found" });

            // Validation
            if (!listing.Images.Any())
                return Json(new { success = false, message = "Please add at least one photo" });

            listing.Status = ListingStatus.Active;
            listing.PublishedDate = DateTime.UtcNow;
            listing.ExpiryDate = DateTime.UtcNow.AddDays(30); // 30-day listing

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Listing published successfully!" });
        }

        // GET: /Listing/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.SellerId == user.Id);

            if (listing == null) return NotFound();

            var viewModel = new EditListingViewModel
            {
                ListingId = listing.ListingId,
                Title = listing.Title,
                Description = listing.Description,
                CategoryId = listing.CategoryId,
                Price = listing.Price,
                LocationAddress = listing.LocationAddress,
                City = listing.City,
                Province = listing.Province,
                PostalCode = listing.PostalCode,
                Condition = listing.Condition,
                Quantity = listing.Quantity,
                Brand = listing.Brand,
                Model = listing.Model,
                IsNegotiable = listing.IsNegotiable,
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: /Listing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditListingViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.ListingId == model.ListingId && l.SellerId == user.Id);

            if (listing == null) return NotFound();

            if (ModelState.IsValid)
            {
                listing.Title = model.Title;
                listing.Description = model.Description;
                listing.CategoryId = model.CategoryId;
                listing.Price = model.Price;
                listing.LocationAddress = model.LocationAddress;
                listing.City = model.City;
                listing.Province = model.Province;
                listing.PostalCode = model.PostalCode;
                listing.Condition = model.Condition;
                listing.Quantity = model.Quantity;
                listing.Brand = model.Brand;
                listing.Model = model.Model;
                listing.IsNegotiable = model.IsNegotiable;
                listing.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Listing updated successfully!";
                return RedirectToAction(nameof(MyListings));
            }

            model.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(model);
        }

        // POST: /Listing/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var listing = await _context.Listings
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.SellerId == user.Id);

            if (listing == null)
                return Json(new { success = false, message = "Listing not found" });

            // Check if listing has active orders
            var hasOrders = await _context.Orders
                .AnyAsync(o => o.Items.Any(i => i.ListingId == id) && 
                              o.Status != OrderStatus.Cancelled &&
                              o.Status != OrderStatus.Completed);

            if (hasOrders)
                return Json(new { success = false, message = "Cannot delete listing with active orders" });

            // Delete images
            foreach (var image in listing.Images)
            {
                var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            listing.Status = ListingStatus.Deleted;
            listing.IsActive = false;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: /Listing/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Seller)
                .Include(l => l.Images)
                .Include(l => l.Reviews)
                    .ThenInclude(r => r.Buyer)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.Status == ListingStatus.Active);

            if (listing == null) return NotFound();

            // Increment view count
            listing.ViewCount++;
            await _context.SaveChangesAsync();

            // Get seller's other listings
            var otherListings = await _context.Listings
                .Include(l => l.Images)
                .Where(l => l.SellerId == listing.SellerId && 
                           l.ListingId != id && 
                           l.Status == ListingStatus.Active)
                .Take(4)
                .ToListAsync();

            var viewModel = new ListingDetailViewModel
            {
                Listing = listing,
                OtherListings = otherListings
            };

            return View(viewModel);
        }

        #region Helper Methods

        private async Task UploadListingImages(int listingId, IFormFileCollection images)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listings", listingId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            int order = 0;
            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    var listingImage = new ListingImage
                    {
                        ListingId = listingId,
                        ImageUrl = $"/uploads/listings/{listingId}/{uniqueFileName}",
                        DisplayOrder = order,
                        IsPrimary = order == 0,
                        FileSizeBytes = image.Length,
                        UploadedDate = DateTime.UtcNow
                    };

                    _context.ListingImages.Add(listingImage);
                    order++;
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
