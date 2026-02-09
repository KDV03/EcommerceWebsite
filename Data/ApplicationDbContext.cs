using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EcommerceWebsite.Models;

namespace EcommerceWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
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
            base.OnModelCreating(modelBuilder);

            // ========== PRODUCT CONFIGURATION ==========
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

            // ========== CATEGORY CONFIGURATION ==========
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
            });

            // ========== APPLICATION USER CONFIGURATION ==========
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                // FIX: Configure decimal precision for SellerRating
                entity.Property(u => u.SellerRating)
                    .HasPrecision(18, 2);

                // Configure Documents relationship
                entity.HasMany(u => u.Documents)
                    .WithOne(d => d.User)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure Listings relationship
                entity.HasMany(u => u.Listings)
                    .WithOne(l => l.Seller)
                    .HasForeignKey(l => l.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure OrdersAsBuyer relationship
                entity.HasMany(u => u.OrdersAsBuyer)
                    .WithOne(o => o.Buyer)
                    .HasForeignKey(o => o.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure OrdersAsSeller relationship
                entity.HasMany(u => u.OrdersAsSeller)
                    .WithOne(o => o.Seller)
                    .HasForeignKey(o => o.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== USER DOCUMENT CONFIGURATION ==========
            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(e => e.DocumentId);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Documents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Reviewer)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== LISTING CONFIGURATION ==========
            modelBuilder.Entity<Listing>(entity =>
            {
                entity.HasKey(e => e.ListingId);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.HasOne(l => l.Seller)
                    .WithMany(u => u.Listings)
                    .HasForeignKey(l => l.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Category)
                    .WithMany()
                    .HasForeignKey(l => l.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.Images)
                    .WithOne(i => i.Listing)
                    .HasForeignKey(i => i.ListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(l => l.Reviews)
                    .WithOne(r => r.Listing)
                    .HasForeignKey(r => r.ListingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedDate);
            });

            // ========== LISTING IMAGE CONFIGURATION ==========
            modelBuilder.Entity<ListingImage>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.HasOne(i => i.Listing)
                    .WithMany(l => l.Images)
                    .HasForeignKey(i => i.ListingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ========== ORDER CONFIGURATION ==========
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

                entity.HasMany(o => o.StatusHistory)
                    .WithOne(h => h.Order)
                    .HasForeignKey(h => h.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.BuyerId);
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.Status);
            });

            // ========== ORDER ITEM CONFIGURATION ==========
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);

                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.HasOne(i => i.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Listing)
                    .WithMany()
                    .HasForeignKey(i => i.ListingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== ORDER STATUS HISTORY CONFIGURATION ==========
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.HasOne(h => h.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(h => h.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(h => h.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== PAYMENT CONFIGURATION ==========
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(p => p.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TransactionId);
            });

            // ========== REVIEW CONFIGURATION ==========
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId);

                entity.HasOne(r => r.Order)
                    .WithMany()
                    .HasForeignKey(r => r.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

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

            // ========== DISPUTE CONFIGURATION ==========
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

            // ========== SEED DATA ==========
            SeedData(modelBuilder);
        }

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
                    DiscountPercentage = 15m,
                    CreatedDate = DateTime.UtcNow
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
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Premium Cotton T-Shirt",
                    Price = 24.99m,
                    Description = "100% organic cotton, comfortable fit for everyday wear",
                    CategoryId = 2,
                    StockQuantity = 150,
                    SKU = "CLO-TS-001",
                    ImageUrl = "/images/products/tshirt.jpg",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            );
        }
    }
}