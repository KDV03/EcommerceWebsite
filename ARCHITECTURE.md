# TECHNICAL ARCHITECTURE DOCUMENTATION
## E-Commerce Website - ASP.NET Core MVC

---

## 📐 ARCHITECTURE OVERVIEW

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Razor Views (HTML/CSS/JS)               │   │
│  │  • Layout Templates  • Product Listings             │   │
│  │  • Search Interface  • Detail Pages                 │   │
│  └─────────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              MVC Controllers                          │  │
│  │  • HomeController    • ProductController             │  │
│  │  • Request Routing   • View Model Assembly           │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                     BUSINESS LOGIC LAYER                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Service Layer                            │  │
│  │  • IProductService / ProductService                   │  │
│  │  • ICategoryService / CategoryService                 │  │
│  │  • Business Rules & Validation                        │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                      DATA ACCESS LAYER                       │
│  ┌──────────────────────────────────────────────────────┐  │
│  │         Entity Framework Core (ORM)                   │  │
│  │  • ApplicationDbContext  • Migrations                 │  │
│  │  • LINQ Queries          • Change Tracking            │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                        DATABASE LAYER                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  In-Memory DB (Dev) / SQL Server (Production)         │  │
│  │  • Products Table    • Categories Table               │  │
│  │  • Indexes          • Relationships                  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## 🏗️ DESIGN PATTERNS IMPLEMENTED

### 1. Model-View-Controller (MVC) Pattern

**Purpose:** Separation of concerns between data, presentation, and logic

**Implementation:**
```
Models/          → Domain entities (Product, Category)
Views/           → UI templates (Razor .cshtml files)
Controllers/     → Request handlers, coordinate between Model and View
```

**Benefits:**
- Clear responsibility boundaries
- Testable components
- Parallel development possible
- Easier maintenance

### 2. Repository Pattern (via Service Layer)

**Purpose:** Abstract data access logic from business logic

**Implementation:**
```csharp
// Interface defines contract
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int productId);
}

// Concrete implementation
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // Implementation details...
}
```

**Benefits:**
- Centralized data access logic
- Easy to mock for unit testing
- Can switch data sources without affecting controllers
- Consistent error handling

### 3. Dependency Injection (DI) Pattern

**Purpose:** Achieve loose coupling and improve testability

**Implementation:**
```csharp
// Registration in Program.cs
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Usage in controllers
public class ProductController : Controller
{
    private readonly IProductService _productService;
    
    public ProductController(IProductService productService)
    {
        _productService = productService; // Injected by DI container
    }
}
```

**Benefits:**
- Loose coupling between components
- Easy to swap implementations
- Simplified unit testing
- Better code organization
- Lifecycle management handled by framework

### 4. Unit of Work Pattern (via DbContext)

**Purpose:** Manage database transactions and ensure consistency

**Implementation:**
```csharp
public class ApplicationDbContext : DbContext
{
    // DbContext itself acts as Unit of Work
    // All operations tracked until SaveChangesAsync() is called
    
    public async Task<int> SaveChangesAsync()
    {
        // Commits all tracked changes in a single transaction
    }
}
```

**Benefits:**
- ACID transaction guarantees
- Change tracking
- Reduced database round trips
- Automatic rollback on errors

### 5. View Model Pattern

**Purpose:** Separate presentation concerns from domain models

**Implementation:**
```csharp
// Domain Model (represents database entity)
public class Product { ... }

// View Model (represents view requirements)
public class ProductListViewModel
{
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<Category> Categories { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    // View-specific properties
}
```

**Benefits:**
- Views don't expose database structure
- Can combine multiple entities
- Add view-specific logic without polluting domain models
- Improved security (no over-posting)

### 6. Factory Pattern (via DI Container)

**Purpose:** Create objects without specifying exact classes

**Implementation:**
```csharp
// DI container acts as factory
var productService = serviceProvider.GetService<IProductService>();
// Returns ProductService instance without direct instantiation
```

### 7. Strategy Pattern (via Interface Segregation)

**Purpose:** Define family of algorithms and make them interchangeable

**Implementation:**
```csharp
// Could easily swap implementations
public interface IProductService { ... }
public class ProductService : IProductService { ... }
public class CachedProductService : IProductService { ... } // Future
public class MockProductService : IProductService { ... } // Testing
```

---

## 🎯 SOLID PRINCIPLES APPLICATION

### Single Responsibility Principle (SRP)

**Definition:** Each class should have one, and only one, reason to change

**Implementation:**
```csharp
// ✅ GOOD: ProductService only handles product operations
public class ProductService : IProductService
{
    public async Task<Product> GetProductByIdAsync(int id) { ... }
    public async Task<IEnumerable<Product>> GetAllProductsAsync() { ... }
}

// ✅ GOOD: CategoryService only handles category operations
public class CategoryService : ICategoryService
{
    public async Task<Category> GetCategoryByIdAsync(int id) { ... }
}

// ❌ BAD: Would violate SRP
public class DataService
{
    public async Task<Product> GetProduct() { ... }
    public async Task<Category> GetCategory() { ... }
    public async Task SendEmail() { ... } // Unrelated responsibility
}
```

### Open/Closed Principle (OCP)

**Definition:** Classes should be open for extension but closed for modification

**Implementation:**
```csharp
// Base interface allows extension without modification
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
}

// Can extend behavior by creating new implementations
public class CachedProductService : IProductService
{
    private readonly IProductService _innerService;
    private readonly IMemoryCache _cache;
    
    // Extends behavior without modifying original
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Add caching layer
        return _cache.GetOrCreate("all-products", 
            entry => _innerService.GetAllProductsAsync());
    }
}
```

### Liskov Substitution Principle (LSP)

**Definition:** Derived classes must be substitutable for their base classes

**Implementation:**
```csharp
// Any implementation of IProductService can replace another
IProductService service = new ProductService(context);
// OR
IProductService service = new CachedProductService(new ProductService(context));
// OR (for testing)
IProductService service = new MockProductService();

// All work the same way from controller's perspective
var products = await service.GetAllProductsAsync();
```

### Interface Segregation Principle (ISP)

**Definition:** Clients shouldn't be forced to depend on interfaces they don't use

**Implementation:**
```csharp
// ✅ GOOD: Focused interfaces
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
}

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
}

// ❌ BAD: Would force clients to implement unused methods
public interface IMassiveService
{
    Task<Product> GetProduct();
    Task<Category> GetCategory();
    Task<Order> GetOrder(); // Not needed in browsing app
    Task<User> GetUser();   // Not needed in browsing app
}
```

### Dependency Inversion Principle (DIP)

**Definition:** Depend on abstractions, not concretions

**Implementation:**
```csharp
// ✅ GOOD: Controller depends on abstraction
public class ProductController : Controller
{
    private readonly IProductService _productService; // Interface, not concrete class
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
}

// ❌ BAD: Direct dependency on concrete class
public class BadProductController : Controller
{
    private readonly ProductService _productService; // Concrete class
    private readonly ApplicationDbContext _context;  // Concrete class
    
    public BadProductController()
    {
        _context = new ApplicationDbContext(); // Creates coupling
        _productService = new ProductService(_context);
    }
}
```

---

## 🔄 REQUEST FLOW DIAGRAM

### Typical Product Listing Request

```
┌──────────┐
│ Browser  │
└────┬─────┘
     │ GET /Product?categoryId=1
     ↓
┌─────────────────────────────────────────┐
│  Kestrel Web Server                      │
│  (ASP.NET Core Runtime)                  │
└────┬────────────────────────────────────┘
     │
     ↓
┌─────────────────────────────────────────┐
│  Routing Middleware                      │
│  Matches: Product/Index                  │
└────┬────────────────────────────────────┘
     │
     ↓
┌─────────────────────────────────────────┐
│  ProductController.Index()               │
│  Parameters: categoryId = 1              │
└────┬────────────────────────────────────┘
     │
     ↓ _productService.GetProductsByCategoryAsync(1)
     │
┌─────────────────────────────────────────┐
│  ProductService                          │
│  Business Logic Layer                    │
└────┬────────────────────────────────────┘
     │
     ↓ _context.Products.Where(...).ToListAsync()
     │
┌─────────────────────────────────────────┐
│  Entity Framework Core                   │
│  Builds SQL Query                        │
└────┬────────────────────────────────────┘
     │
     ↓ SQL: SELECT * FROM Products WHERE CategoryId = 1
     │
┌─────────────────────────────────────────┐
│  Database (In-Memory / SQL Server)       │
│  Returns: List<Product>                  │
└────┬────────────────────────────────────┘
     │
     ↓ Returns IEnumerable<Product>
     │
┌─────────────────────────────────────────┐
│  ProductService                          │
│  Returns data to Controller              │
└────┬────────────────────────────────────┘
     │
     ↓
┌─────────────────────────────────────────┐
│  ProductController.Index()               │
│  Creates ProductListViewModel            │
│  return View(viewModel)                  │
└────┬────────────────────────────────────┘
     │
     ↓
┌─────────────────────────────────────────┐
│  Razor View Engine                       │
│  Processes: Product/Index.cshtml         │
│  Combines data with HTML template        │
└────┬────────────────────────────────────┘
     │
     ↓ HTML Response
     │
┌─────────────────────────────────────────┐
│  Response Middleware                     │
│  Adds headers, compression, etc.         │
└────┬────────────────────────────────────┘
     │
     ↓ HTTP Response
     │
┌──────────┐
│ Browser  │ Renders HTML
└──────────┘
```

---

## 💾 DATA LAYER ARCHITECTURE

### Entity Relationship Diagram

```
┌─────────────────────────┐         ┌─────────────────────────┐
│      Category            │         │       Product            │
├─────────────────────────┤         ├─────────────────────────┤
│ • CategoryId (PK)        │1      * │ • ProductId (PK)        │
│ • Name                   │────────>│ • Name                   │
│ • Description            │         │ • Price                  │
│ • ImageUrl               │         │ • Description            │
│ • IsActive               │         │ • CategoryId (FK)        │
│                          │         │ • StockQuantity          │
│ Navigation:              │         │ • SKU (Unique)           │
│ • Products (Collection)  │         │ • DiscountPercentage     │
└─────────────────────────┘         │ • ImageUrl               │
                                     │ • IsActive               │
                                     │ • CreatedDate            │
                                     │ • ModifiedDate           │
                                     │                          │
                                     │ Computed Properties:     │
                                     │ • FinalPrice             │
                                     │ • IsInStock              │
                                     │                          │
                                     │ Navigation:              │
                                     │ • Category               │
                                     └─────────────────────────┘
```

### Database Relationships

**One-to-Many: Category → Products**
- One category can have many products
- Each product belongs to one category
- Foreign Key: `Product.CategoryId` references `Category.CategoryId`
- Delete Behavior: Restrict (prevents orphan products)

**Configuration:**
```csharp
modelBuilder.Entity<Product>()
    .HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);
```

### Indexes

```csharp
// Unique index on SKU for fast lookups and data integrity
modelBuilder.Entity<Product>()
    .HasIndex(e => e.SKU)
    .IsUnique();

// Composite index for common query patterns (suggested for production)
modelBuilder.Entity<Product>()
    .HasIndex(e => new { e.CategoryId, e.IsActive });
```

---

## 🔌 DEPENDENCY INJECTION CONTAINER

### Service Lifetimes

**Transient (`AddTransient`)**
- New instance created each time it's requested
- Best for: Lightweight, stateless services

**Scoped (`AddScoped`)** ← Used in this application
- One instance per HTTP request
- Best for: Services that maintain state during a request (like DbContext)
- Implementation:
```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

**Singleton (`AddSingleton`)**
- Single instance for application lifetime
- Best for: Configuration, caching services

### Why Scoped for This App?

```csharp
// DbContext is scoped - one per request
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EcommerceDb"));

// Services depend on DbContext, so they must also be scoped
builder.Services.AddScoped<IProductService, ProductService>();
```

**Request Lifecycle:**
```
HTTP Request Received
    ↓
DI Container creates new scope
    ↓
Creates ApplicationDbContext
    ↓
Creates ProductService (with DbContext)
    ↓
Creates CategoryService (with DbContext)
    ↓
Controller receives services
    ↓
Request processed
    ↓
Response sent
    ↓
Scope disposed (all scoped instances cleaned up)
```

---

## 🎨 PRESENTATION LAYER DETAILS

### Razor View Engine

**Layout Inheritance:**
```
_Layout.cshtml (Master template)
    ├── Navbar
    ├── @RenderBody() ← Individual views render here
    └── Footer

_ViewStart.cshtml
    └── Sets default layout for all views

_ViewImports.cshtml
    └── Imports namespaces, tag helpers for all views
```

### View Components Structure

**Shared Components:**
- Navigation bar with search
- Footer with links
- Product card (reusable)
- Pagination controls

**Page-Specific Views:**
- Home/Index: Hero, categories, featured products
- Product/Index: Filterable product grid
- Product/Details: Detailed product information

### Tag Helpers

```cshtml
<!-- URL generation -->
<a asp-controller="Product" asp-action="Details" asp-route-id="@product.ProductId">

<!-- Form generation -->
<form asp-controller="Product" asp-action="Search" method="get">

<!-- Benefits: -->
<!-- • Type-safe URL generation -->
<!-- • Automatic route value resolution -->
<!-- • IntelliSense support -->
```

---

## 🚀 MIDDLEWARE PIPELINE

Order matters! Current configuration in `Program.cs`:

```csharp
// 1. Exception handling (dev vs production)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 2. HTTPS redirection
app.UseHttpsRedirection();

// 3. Static files (CSS, JS, images)
app.UseStaticFiles();

// 4. Routing
app.UseRouting();

// 5. Response caching
app.UseResponseCaching();

// 6. Session (for future cart functionality)
app.UseSession();

// 7. Authorization (for future authentication)
app.UseAuthorization();

// 8. Endpoint execution (controllers)
app.MapControllerRoute(...);
```

**Pipeline Flow:**
```
Request → Exception Handler → HTTPS → Static Files → Routing → 
Caching → Session → Auth → Controller → Response
```

---

## 📊 PERFORMANCE CONSIDERATIONS

### Implemented Optimizations

**1. Async/Await Throughout**
```csharp
public async Task<IEnumerable<Product>> GetAllProductsAsync()
{
    return await _context.Products.ToListAsync();
}
```
- Non-blocking I/O operations
- Better thread pool utilization
- Handles more concurrent requests

**2. Eager Loading**
```csharp
var products = await _context.Products
    .Include(p => p.Category) // Loads category data in same query
    .ToListAsync();
```
- Prevents N+1 query problems
- Single database round trip
- Better performance

**3. Pagination**
```csharp
var paginatedProducts = products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```
- Reduces data transfer
- Faster page loads
- Better user experience

**4. Response Caching**
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```
- Caches static responses
- Reduces server load

### Recommended Additions for Production

**1. Output Caching**
```csharp
[ResponseCache(Duration = 300)] // 5 minutes
public async Task<IActionResult> Index()
```

**2. Database Indexes**
```csharp
modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.CategoryId, p.IsActive, p.CreatedDate });
```

**3. Compression**
```csharp
builder.Services.AddResponseCompression();
```

**4. CDN for Static Files**
- Move CSS, JS, images to CDN
- Faster content delivery
- Reduced server load

---

## 🔒 SECURITY ARCHITECTURE

### Current Security Features

**1. CSRF Protection (Built-in)**
```cshtml
<form asp-controller="..." asp-action="...">
    <!-- Automatically includes anti-forgery token -->
</form>
```

**2. SQL Injection Prevention**
- EF Core uses parameterized queries automatically
- No raw SQL in application

**3. XSS Protection**
- Razor engine HTML-encodes output by default
- User input sanitized automatically

**4. HTTPS Enforcement**
```csharp
app.UseHttpsRedirection();
app.UseHsts(); // HTTP Strict Transport Security
```

### Recommended for Production

**1. Authentication**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
```

**2. Authorization**
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
```

**3. Input Validation**
```csharp
[StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
public string Name { get; set; }
```

**4. Rate Limiting**
```csharp
builder.Services.AddRateLimiter();
```

---

## 📈 SCALABILITY CONSIDERATIONS

### Current Architecture Supports

**Horizontal Scaling:**
- Stateless application design
- Session stored externally (configured, not used yet)
- Can run multiple instances behind load balancer

**Vertical Scaling:**
- Async operations support more concurrent users
- Efficient memory usage
- Minimal resource overhead

### Future Enhancements for Scale

**1. Distributed Caching (Redis)**
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

**2. Message Queue (RabbitMQ/Azure Service Bus)**
- Process orders asynchronously
- Handle high traffic spikes
- Decouple components

**3. CQRS Pattern**
- Separate read/write models
- Optimize each independently
- Better performance at scale

**4. Database Sharding**
- Partition data across multiple databases
- Distribute load
- Improve query performance

---

## 🧪 TESTABILITY

### Unit Testing Strategy

**Service Layer Tests:**
```csharp
[Fact]
public async Task GetProductById_ExistingId_ReturnsProduct()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    
    using var context = new ApplicationDbContext(options);
    var service = new ProductService(context);
    
    // Act
    var result = await service.GetProductByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Expected Product", result.Name);
}
```

**Controller Tests:**
```csharp
[Fact]
public async Task Index_ReturnsViewResult_WithProductList()
{
    // Arrange
    var mockService = new Mock<IProductService>();
    mockService.Setup(s => s.GetAllProductsAsync())
        .ReturnsAsync(GetTestProducts());
    
    var controller = new ProductController(mockService.Object, null);
    
    // Act
    var result = await controller.Index(null, null, null, 1);
    
    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsType<ProductListViewModel>(viewResult.Model);
    Assert.NotEmpty(model.Products);
}
```

### Integration Testing

```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task GetProductsPage_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/Product");
        response.EnsureSuccessStatusCode();
    }
}
```

---

## 📚 CONCLUSION

This architecture provides:
- **Maintainability** through clear separation of concerns
- **Scalability** through stateless design and async operations
- **Testability** through dependency injection and interfaces
- **Extensibility** through open/closed principle adherence
- **Security** through framework best practices

The foundation is solid for adding features like:
- Shopping cart
- User authentication
- Payment processing
- Order management
- Admin panel
- Product reviews
- Wishlist functionality

---

**Document Version:** 1.0  
**Last Updated:** January 2026  
**Technology Stack:** ASP.NET Core 8.0, EF Core 8.0, C# 12.0
