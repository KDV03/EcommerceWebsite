using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Data
{
    /// <summary>
    /// Database context managing entity relationships and database operations
    /// Now extends IdentityDbContext for authentication support
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        // New marketplace DbSets
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Dispute> Disputes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);
                
                entity.Property(e => e.DiscountPercentage)
                    .HasPrecision(5, 2);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.SKU)
                    .IsUnique();
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
            });

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.Documents)
                    .WithOne(d => d.User)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Listings)
                    .WithOne(l => l.Seller)
                    .HasForeignKey(l => l.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure UserDocument
            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(e => e.DocumentId);

                entity.HasOne(d => d.Reviewer)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Listing
            modelBuilder.Entity<Listing>(entity =>
            {
                entity.HasKey(e => e.ListingId);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.HasMany(l => l.Images)
                    .WithOne(i => i.Listing)
                    .HasForeignKey(i => i.ListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure ListingImage
            modelBuilder.Entity<ListingImage>(entity =>
            {
                entity.HasKey(e => e.ImageId);
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.ShippingCost).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.HasOne(o => o.Buyer)
                    .WithMany(u => u.OrdersAsBuyer)
                    .HasForeignKey(o => o.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Seller)
                    .WithMany(u => u.OrdersAsSeller)
                    .HasForeignKey(o => o.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.Payments)
                    .WithOne(p => p.Order)
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.BuyerId);
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.Status);
            });

            // Configure OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);

                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });

            // Configure Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasIndex(e => e.TransactionId);
            });

            // Configure Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId);

                entity.HasOne(r => r.Buyer)
                    .WithMany()
                    .HasForeignKey(r => r.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Seller)
                    .WithMany()
                    .HasForeignKey(r => r.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Listing)
                    .WithMany(l => l.Reviews)
                    .HasForeignKey(r => r.ListingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Dispute
            modelBuilder.Entity<Dispute>(entity =>
            {
                entity.HasKey(e => e.DisputeId);

                entity.Property(e => e.RefundAmount).HasPrecision(18, 2);

                entity.HasOne(d => d.Order)
                    .WithOne(o => o.Dispute)
                    .HasForeignKey<Dispute>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Initiator)
                    .WithMany()
                    .HasForeignKey(d => d.InitiatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Resolver)
                    .WithMany()
                    .HasForeignKey(d => d.ResolvedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Seeds initial categories and products for demonstration
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
                new Category { CategoryId = 2, Name = "Clothing", Description = "Fashion and apparel", IsActive = true },
                new Category { CategoryId = 3, Name = "Books", Description = "Physical and digital books", IsActive = true },
                new Category { CategoryId = 4, Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
                new Category { CategoryId = 5, Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear", IsActive = true }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                // Electronics
                new Product
                {
                    ProductId = 1,
                    Name = "Wireless Bluetooth Headphones",
                    Price = 89.99m,
                    Description = "Premium noise-cancelling wireless headphones with 30-hour battery life",
                    CategoryId = 1,
                    StockQuantity = 45,
                    SKU = "ELEC-WH-001",
                    ImageUrl = "/images/products/headphones.jpg",
                    IsActive = true,
                    DiscountPercentage = 15m
                },
                new Product
                {
                    ProductId = 2,
                    Name = "4K Ultra HD Smart TV 55\"",
                    Price = 599.99m,
                    Description = "Experience stunning picture quality with HDR and smart features",
                    CategoryId = 1,
                    StockQuantity = 12,
                    SKU = "ELEC-TV-002",
                    ImageUrl = "/images/products/tv.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Mechanical Gaming Keyboard",
                    Price = 129.99m,
                    Description = "RGB backlit mechanical keyboard with programmable keys",
                    CategoryId = 1,
                    StockQuantity = 28,
                    SKU = "ELEC-KB-003",
                    ImageUrl = "/images/products/keyboard.jpg",
                    IsActive = true,
                    DiscountPercentage = 10m
                },
                
                // Clothing
                new Product
                {
                    ProductId = 4,
                    Name = "Premium Cotton T-Shirt",
                    Price = 24.99m,
                    Description = "100% organic cotton, comfortable fit for everyday wear",
                    CategoryId = 2,
                    StockQuantity = 150,
                    SKU = "CLO-TS-001",
                    ImageUrl = "/images/products/tshirt.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductId = 5,
                    Name = "Denim Jacket",
                    Price = 79.99m,
                    Description = "Classic denim jacket with modern fit and style",
                    CategoryId = 2,
                    StockQuantity = 35,
                    SKU = "CLO-JK-002",
                    ImageUrl = "/images/products/jacket.jpg",
                    IsActive = true,
                    DiscountPercentage = 20m
                },
                
                // Books
                new Product
                {
                    ProductId = 6,
                    Name = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Price = 42.99m,
                    Description = "Essential guide for writing maintainable and elegant code",
                    CategoryId = 3,
                    StockQuantity = 67,
                    SKU = "BOOK-CC-001",
                    ImageUrl = "/images/products/cleancode.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductId = 7,
                    Name = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    Price = 54.99m,
                    Description = "The foundational text on software design patterns",
                    CategoryId = 3,
                    StockQuantity = 42,
                    SKU = "BOOK-DP-002",
                    ImageUrl = "/images/products/designpatterns.jpg",
                    IsActive = true
                },
                
                // Home & Garden
                new Product
                {
                    ProductId = 8,
                    Name = "Stainless Steel Cookware Set",
                    Price = 199.99m,
                    Description = "Professional-grade 12-piece cookware set",
                    CategoryId = 4,
                    StockQuantity = 18,
                    SKU = "HOME-CW-001",
                    ImageUrl = "/images/products/cookware.jpg",
                    IsActive = true,
                    DiscountPercentage = 25m
                },
                new Product
                {
                    ProductId = 9,
                    Name = "Electric Lawn Mower",
                    Price = 349.99m,
                    Description = "Cordless electric mower with 45-minute runtime",
                    CategoryId = 4,
                    StockQuantity = 8,
                    SKU = "HOME-LM-002",
                    ImageUrl = "/images/products/lawnmower.jpg",
                    IsActive = true
                },
                
                // Sports & Outdoors
                new Product
                {
                    ProductId = 10,
                    Name = "Professional Yoga Mat",
                    Price = 39.99m,
                    Description = "Non-slip, eco-friendly yoga mat with carrying strap",
                    CategoryId = 5,
                    StockQuantity = 95,
                    SKU = "SPORT-YM-001",
                    ImageUrl = "/images/products/yogamat.jpg",
                    IsActive = true,
                    DiscountPercentage = 5m
                },
                new Product
                {
                    ProductId = 11,
                    Name = "Camping Tent 4-Person",
                    Price = 189.99m,
                    Description = "Waterproof tent with easy setup for family camping",
                    CategoryId = 5,
                    StockQuantity = 22,
                    SKU = "SPORT-TN-002",
                    ImageUrl = "/images/products/tent.jpg",
                    IsActive = true
                },
                new Product
                {
                    ProductId = 12,
                    Name = "Mountain Bike 27.5\"",
                    Price = 899.99m,
                    Description = "Aluminum frame mountain bike with 21-speed gearing",
                    CategoryId = 5,
                    StockQuantity = 6,
                    SKU = "SPORT-BK-003",
                    ImageUrl = "/images/products/bike.jpg",
                    IsActive = true,
                    DiscountPercentage = 12m
                }
            );
        }
    }
}
