# E-Commerce Website - ASP.NET Core MVC

A professional, production-ready e-commerce browsing platform built with C# and ASP.NET Core 8.0. This application demonstrates modern web development practices with clean architecture, dependency injection, and responsive design.

## 🎯 Project Overview

This is a **browsing-focused** e-commerce platform that allows users to:
- Browse products across multiple categories
- Search and filter products
- View detailed product information
- Experience responsive, modern UI/UX
- **Note: Shopping cart functionality is reserved for future implementation**

## 🏗️ Architecture & Design Patterns

### Clean Architecture
- **Separation of Concerns**: Models, Services, Controllers, and Views are properly separated
- **Dependency Injection**: Services are injected via constructor injection
- **Repository Pattern**: Data access abstracted through service layer
- **SOLID Principles**: Code follows Single Responsibility, Interface Segregation, and Dependency Inversion

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Database**: In-Memory (development) / SQL Server (production)
- **Frontend**: Razor Views, HTML5, CSS3, JavaScript
- **Language**: C# 12.0

## 📁 Project Structure

```
EcommerceWebsite/
│
├── Controllers/              # MVC Controllers
│   ├── HomeController.cs     # Homepage, About, Contact
│   └── ProductController.cs  # Product browsing, filtering, details
│
├── Models/                   # Domain Models
│   ├── Product.cs            # Product entity
│   ├── Category.cs           # Category entity
│   └── ViewModels.cs         # View-specific models
│
├── Data/                     # Data Access Layer
│   └── ApplicationDbContext.cs  # EF Core DbContext with seeding
│
├── Services/                 # Business Logic Layer
│   ├── IProductService.cs    # Product service interface
│   ├── ProductService.cs     # Product service implementation
│   ├── ICategoryService.cs   # Category service interface
│   └── CategoryService.cs    # Category service implementation
│
├── Views/                    # Razor Views
│   ├── Home/
│   │   └── Index.cshtml      # Homepage
│   ├── Product/
│   │   ├── Index.cshtml      # Product listing
│   │   └── Details.cshtml    # Product details
│   ├── Shared/
│   │   └── _Layout.cshtml    # Master layout
│   ├── _ViewImports.cshtml   # Shared directives
│   └── _ViewStart.cshtml     # Default layout
│
├── wwwroot/                  # Static files
│   ├── css/
│   │   └── site.css          # Main stylesheet
│   ├── js/
│   │   └── site.js           # JavaScript
│   └── images/               # Image assets
│
├── Program.cs                # Application entry point
├── appsettings.json          # Configuration
└── EcommerceWebsite.csproj   # Project file
```

## 🚀 Getting Started

### Prerequisites

1. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Visual Studio 2022** (recommended) or **VS Code** with C# extension
3. **SQL Server** (for production) or use In-Memory database (development)

### Installation Steps

#### Option 1: Using Visual Studio

1. **Clone or download** this project
2. **Open the solution**:
   - Double-click `EcommerceWebsite.csproj`
   - Or open Visual Studio → File → Open → Project/Solution
3. **Restore NuGet packages**:
   - Right-click solution → Restore NuGet Packages
   - Or: Tools → NuGet Package Manager → Package Manager Console
   - Run: `dotnet restore`
4. **Build the project**:
   - Build → Build Solution (Ctrl+Shift+B)
5. **Run the application**:
   - Press F5 or click the green "play" button
   - Application will launch at `https://localhost:5001`

#### Option 2: Using Command Line

1. **Navigate to project directory**:
   ```bash
   cd path/to/EcommerceWebsite
   ```

2. **Restore packages**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the application**:
   - Open browser to: `https://localhost:5001` or `http://localhost:5000`

## 🗄️ Database Configuration

### Development (In-Memory Database)

By default, the application uses an **in-memory database** for quick setup:

```csharp
// In Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EcommerceDb"));
```

**Advantages**:
- No database installation required
- Fast development and testing
- Automatic data seeding on startup

**Limitations**:
- Data resets on application restart
- Not suitable for production

### Production (SQL Server)

For production deployment with persistent data:

1. **Update connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=EcommerceWebsite;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

2. **Modify Program.cs**:
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(
           builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

3. **Create database migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## 📊 Sample Data

The application automatically seeds the database with:
- **5 Categories**: Electronics, Clothing, Books, Home & Garden, Sports & Outdoors
- **12 Products** across all categories with:
  - Realistic pricing
  - Stock quantities
  - Product descriptions
  - SKU codes
  - Discount percentages

## 🎨 Features Explained

### 1. Homepage
- **Hero Section**: Eye-catching banner with call-to-action
- **Category Grid**: Visual category navigation
- **Featured Products**: Showcases discounted and popular items
- **Features Section**: Trust indicators (shipping, returns, support)

### 2. Product Browsing (`/Product`)
- **Grid Layout**: Responsive product cards
- **Category Filtering**: Filter by category from sidebar
- **Search Functionality**: Full-text search across product names, descriptions, SKUs
- **Sorting Options**:
  - Newest First (default)
  - Price: Low to High
  - Price: High to Low
  - Name: A to Z
  - Name: Z to A
- **Pagination**: 12 products per page with page navigation
- **Stock Indicators**: Visual availability status

### 3. Product Details (`/Product/Details/{id}`)
- **Large Image Display**: Product visualization
- **Comprehensive Information**: Price, stock, description, SKU
- **Discount Display**: Original price vs. discounted price with savings
- **Breadcrumb Navigation**: Easy navigation back to listings
- **Related Products**: Suggestions from same category
- **Product Metadata**: Category badge, stock quantity

### 4. Search (`/Product/Search?term={query}`)
- Searches across: product name, description, SKU
- Case-insensitive matching
- Results maintain all filtering/sorting capabilities

## 🔧 Key Technical Implementation

### Service Layer Pattern

```csharp
// Interface defines contract
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int productId);
    // ... other methods
}

// Implementation contains business logic
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    // ... implementation
}

// Registration in Program.cs
builder.Services.AddScoped<IProductService, ProductService>();
```

**Benefits**:
- Testability (mock interfaces in unit tests)
- Flexibility (swap implementations)
- Maintainability (centralized business logic)

### Entity Framework Core Usage

```csharp
// Eager loading with Include
var products = await _context.Products
    .Include(p => p.Category)
    .Where(p => p.IsActive)
    .ToListAsync();
```

**Best Practices**:
- Async operations for scalability
- Eager loading to prevent N+1 queries
- Strongly-typed queries with LINQ
- Data annotations for validation

### Computed Properties

```csharp
[NotMapped]
public decimal FinalPrice
{
    get
    {
        if (DiscountPercentage.HasValue && DiscountPercentage.Value > 0)
            return Price - (Price * DiscountPercentage.Value / 100);
        return Price;
    }
}
```

Calculated at runtime without database storage.

## 🎯 Adding Shopping Cart (Future Enhancement)

To add shopping cart functionality:

1. **Create Cart Models**:
   ```csharp
   public class ShoppingCart { ... }
   public class CartItem { ... }
   ```

2. **Implement Cart Service**:
   ```csharp
   public interface ICartService
   {
       Task AddToCartAsync(int productId, int quantity);
       Task RemoveFromCartAsync(int cartItemId);
       Task<ShoppingCart> GetCartAsync();
   }
   ```

3. **Add Cart Controller**:
   - Actions: AddToCart, RemoveFromCart, ViewCart, Checkout

4. **Update Views**:
   - Add "Add to Cart" buttons
   - Create cart view
   - Add cart icon to navigation

5. **Session/Cookie Management**:
   ```csharp
   builder.Services.AddSession();
   app.UseSession();
   ```

## 📱 Responsive Design

The application is fully responsive with breakpoints:
- **Desktop**: 1024px+ (full layout)
- **Tablet**: 768px-1023px (adjusted grid)
- **Mobile**: <768px (stacked layout)

CSS uses:
- CSS Grid for layouts
- Flexbox for components
- CSS Custom Properties (variables)
- Mobile-first approach

## 🔒 Security Considerations

Current implementation includes:
- Input validation via Data Annotations
- HTTPS redirection
- CSRF protection (built-in with ASP.NET Core)
- SQL injection prevention (EF Core parameterization)

**For production**, add:
- Authentication/Authorization (ASP.NET Identity)
- Rate limiting
- Content Security Policy headers
- Data encryption

## 🧪 Testing Recommendations

### Unit Testing
```csharp
// Test service layer with mocked DbContext
[Fact]
public async Task GetProductById_ReturnsProduct()
{
    // Arrange
    var mockContext = CreateMockContext();
    var service = new ProductService(mockContext);
    
    // Act
    var result = await service.GetProductByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Expected Product Name", result.Name);
}
```

### Integration Testing
- Test controllers with TestServer
- Verify database operations
- Test complete request/response cycle

## 🚀 Deployment

### IIS Deployment
1. Publish application: `dotnet publish -c Release`
2. Copy publish folder to IIS wwwroot
3. Create IIS application pool (.NET Core)
4. Configure application in IIS Manager
5. Update connection strings for production database

### Azure Deployment
1. Create Azure App Service
2. Configure SQL Azure Database
3. Update connection strings
4. Deploy via Visual Studio, CLI, or GitHub Actions

### Docker Deployment
Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY published/ .
ENTRYPOINT ["dotnet", "EcommerceWebsite.dll"]
```

## 📈 Performance Optimization

Current optimizations:
- Async/await patterns throughout
- Response caching enabled
- Pagination to limit data transfer
- Eager loading to prevent N+1 queries
- Static file caching

**Additional recommendations**:
- Image optimization and CDN
- Output caching for product lists
- Database query optimization with indexes
- Implement Redis for distributed caching

## 🐛 Troubleshooting

### Common Issues

**Issue**: Application won't start
- **Solution**: Ensure .NET 8 SDK is installed: `dotnet --version`

**Issue**: NuGet packages missing
- **Solution**: Run `dotnet restore` in project directory

**Issue**: Database errors
- **Solution**: Check connection string, ensure SQL Server is running

**Issue**: Port already in use
- **Solution**: Modify `launchSettings.json` to use different ports

## 📚 Learning Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [C# Programming Guide](https://docs.microsoft.com/dotnet/csharp)
- [Razor Pages Documentation](https://docs.microsoft.com/aspnet/core/razor-pages)

## 🤝 Contributing

This is a demonstration project. For enhancements:
1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Submit pull request with detailed description

## 📄 License

This project is provided as-is for educational and demonstration purposes.

## 👨‍💻 Support

For questions or issues:
- Review this README thoroughly
- Check the inline code comments
- Consult ASP.NET Core documentation
- Open an issue with detailed error messages

---

**Built with ❤️ using ASP.NET Core MVC**
