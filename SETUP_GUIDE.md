# COMPLETE SETUP GUIDE FOR BEGINNERS
## Step-by-Step Instructions for Running Your E-Commerce Website

This guide assumes you're starting from scratch with no prior ASP.NET Core experience.

---

## 📋 PREREQUISITES CHECKLIST

Before starting, you need to install the following software:

### 1. Install .NET 8 SDK

**What is it?** The .NET SDK is the development kit that allows you to build and run C# applications.

**Download & Install:**
1. Visit: https://dotnet.microsoft.com/download/dotnet/8.0
2. Click "Download .NET SDK x64" (for Windows)
3. Run the installer
4. Follow the installation wizard (keep default options)
5. **Verify installation:**
   - Open Command Prompt (Windows) or Terminal (Mac/Linux)
   - Type: `dotnet --version`
   - You should see something like: `8.0.xxx`

### 2. Install Visual Studio 2022 (Recommended Option)

**What is it?** Visual Studio is a powerful IDE (Integrated Development Environment) specifically designed for .NET development.

**Download & Install:**
1. Visit: https://visualstudio.microsoft.com/downloads/
2. Download "Visual Studio 2022 Community" (free version)
3. Run the installer
4. **Select workloads:**
   - ✅ Check "ASP.NET and web development"
   - ✅ Check ".NET desktop development" (optional but helpful)
5. Click "Install" (this may take 30-60 minutes)

**Alternative: Visual Studio Code**
If you prefer a lighter editor:
1. Download from: https://code.visualstudio.com/
2. Install the "C# Dev Kit" extension
3. Install the "C#" extension by Microsoft

### 3. Install SQL Server (Optional for Production)

**For development, you can skip this** - the application uses an in-memory database.

**For production:**
1. Download SQL Server Express (free): https://www.microsoft.com/sql-server/sql-server-downloads
2. Download SQL Server Management Studio (SSMS): https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms

---

## 🚀 RUNNING THE APPLICATION

### METHOD 1: Using Visual Studio 2022 (EASIEST)

#### Step 1: Open the Project
1. Launch Visual Studio 2022
2. Click "Open a project or solution"
3. Navigate to the folder containing `EcommerceWebsite.csproj`
4. Select `EcommerceWebsite.csproj` and click "Open"

#### Step 2: Restore NuGet Packages
NuGet packages are libraries that the application needs to run.

**Automatic (Recommended):**
- Visual Studio should automatically restore packages when you open the project
- Look for a message in the status bar: "Restoring NuGet packages..."

**Manual:**
1. Right-click the project in Solution Explorer
2. Click "Restore NuGet Packages"

**Via Package Manager Console:**
1. Go to: Tools → NuGet Package Manager → Package Manager Console
2. Type: `dotnet restore`
3. Press Enter

#### Step 3: Build the Project
Building compiles your C# code into executable format.

1. Click "Build" in the top menu
2. Select "Build Solution" (or press Ctrl+Shift+B)
3. Wait for the build to complete
4. Check the "Output" window - you should see: "Build succeeded"

**If build fails:**
- Check the "Error List" window (View → Error List)
- Common issues:
  - Missing NuGet packages → Restore packages again
  - Wrong .NET version → Ensure .NET 8 SDK is installed

#### Step 4: Run the Application
1. Press F5 (or click the green "Play" button that says "EcommerceWebsite")
2. Your default browser will open automatically
3. The application will launch at `https://localhost:5001` or `http://localhost:5000`

**What happens behind the scenes:**
- Database is created in memory
- Sample data (categories & products) is automatically seeded
- Web server (Kestrel) starts
- Your browser connects to the local server

#### Step 5: Explore the Application
Once running, you can:
- Browse the homepage
- Click "Browse Products" to see all items
- Use the search bar to find products
- Filter by category in the sidebar
- Click on any product to see details
- Sort products by price, name, or date

#### Step 6: Stop the Application
- In Visual Studio: Click the red "Stop" button or press Shift+F5
- Or simply close the browser (the server will still run in background until stopped)

---

### METHOD 2: Using Command Line

#### Step 1: Navigate to Project Directory
```bash
# Open Command Prompt (Windows) or Terminal (Mac/Linux)
cd path\to\EcommerceWebsite

# Example:
cd C:\Users\YourName\Downloads\EcommerceWebsite
```

#### Step 2: Restore Packages
```bash
dotnet restore
```
**Expected output:**
```
Determining projects to restore...
Restored C:\...\EcommerceWebsite.csproj (in XX ms).
```

#### Step 3: Build the Project
```bash
dotnet build
```
**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

#### Step 4: Run the Application
```bash
dotnet run
```
**Expected output:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

#### Step 5: Open in Browser
- Open your web browser
- Navigate to: `https://localhost:5001` or `http://localhost:5000`

#### Step 6: Stop the Application
- Press Ctrl+C in the terminal
- Type 'Y' if prompted to terminate the job

---

## 🗄️ UNDERSTANDING THE DATABASE

### In-Memory Database (Default - What You're Using)

**How it works:**
- Database exists only in RAM (computer memory)
- Created automatically when application starts
- **Data is lost when you stop the application**
- Perfect for development and testing

**Seed Data:**
When the app starts, it automatically creates:
```
5 Categories:
├── Electronics (💻)
├── Clothing (👕)
├── Books (📚)
├── Home & Garden (🏡)
└── Sports & Outdoors (⚽)

12 Sample Products:
├── Wireless Bluetooth Headphones ($89.99 - 15% off)
├── 4K Ultra HD Smart TV ($599.99)
├── Mechanical Gaming Keyboard ($129.99 - 10% off)
├── Premium Cotton T-Shirt ($24.99)
├── Denim Jacket ($79.99 - 20% off)
└── ... and 7 more products
```

**Why use in-memory?**
- No database installation required
- Fast setup
- Automatic cleanup
- Ideal for learning

### Switching to SQL Server (For Persistent Data)

If you want data to persist between application restarts:

#### Step 1: Install SQL Server
1. Download SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
2. Run installer, choose "Basic" installation
3. Note down the server name (usually: `localhost\SQLEXPRESS` or `.\SQLEXPRESS`)

#### Step 2: Update Connection String
Open `appsettings.json` and modify:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EcommerceWebsite;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### Step 3: Modify Program.cs
Find this line:
```csharp
options.UseInMemoryDatabase("EcommerceDb"));
```

Replace with:
```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

#### Step 4: Install EF Core Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

#### Step 5: Create Database
```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply to database
dotnet ef database update
```

#### Step 6: Run Application
The database will now persist data between restarts!

---

## 🔧 CUSTOMIZING THE APPLICATION

### Adding New Products

#### Option 1: Through Code (Recommended for Learning)

1. Open `Data/ApplicationDbContext.cs`
2. Find the `SeedData` method
3. Add a new product in the `modelBuilder.Entity<Product>().HasData()` section:

```csharp
new Product
{
    ProductId = 13,  // Must be unique
    Name = "Your Product Name",
    Price = 99.99m,
    Description = "Your product description",
    CategoryId = 1,  // 1=Electronics, 2=Clothing, etc.
    StockQuantity = 50,
    SKU = "YOUR-SKU-001",  // Must be unique
    ImageUrl = "/images/products/yourimage.jpg",
    IsActive = true,
    DiscountPercentage = 10m  // Optional: 10% discount
}
```

4. **Important:** If using in-memory database:
   - Stop the application
   - Restart it
   - New product will appear automatically

5. **If using SQL Server:**
   - Create a new migration: `dotnet ef migrations add AddNewProduct`
   - Update database: `dotnet ef database update`

#### Option 2: Admin Panel (Future Enhancement)
Create an admin controller to add products through a web interface.

### Changing Colors and Styling

1. Open `wwwroot/css/site.css`
2. Find the `:root` section at the top:

```css
:root {
    --primary-color: #2563eb;     /* Main blue color */
    --primary-hover: #1d4ed8;     /* Darker blue on hover */
    --secondary-color: #64748b;   /* Gray color */
    --accent-color: #f59e0b;      /* Orange/yellow accent */
    /* ... more colors ... */
}
```

3. Change these hex color codes to your preferences:
   - Use a color picker: https://htmlcolorcodes.com/
   - Example: Change `--primary-color: #2563eb;` to `--primary-color: #22c55e;` for green

4. Save the file and refresh your browser (Ctrl+F5)

### Changing Site Name

1. **In Layout:** Open `Views/Shared/_Layout.cshtml`
   - Find: `<span class="logo-text">TechMart</span>`
   - Change "TechMart" to your site name

2. **In Page Titles:** Open `Views/Shared/_Layout.cshtml`
   - Find: `<title>@ViewData["Title"] - TechMart</title>`
   - Change "TechMart" to your site name

3. **In Configuration:** Open `appsettings.json`
   - Change: `"SiteName": "TechMart"` to your name

### Adding More Categories

1. Open `Data/ApplicationDbContext.cs`
2. Find `modelBuilder.Entity<Category>().HasData(`
3. Add new category:

```csharp
new Category 
{ 
    CategoryId = 6,  // Must be unique
    Name = "Toys & Games", 
    Description = "Fun for all ages", 
    IsActive = true 
}
```

4. Restart application (in-memory) or run migrations (SQL Server)

---

## 🐛 TROUBLESHOOTING COMMON ISSUES

### Issue: "dotnet is not recognized as an internal or external command"

**Cause:** .NET SDK not installed or not in PATH

**Solution:**
1. Verify .NET is installed: Open new Command Prompt and type `dotnet --version`
2. If still doesn't work:
   - Reinstall .NET SDK
   - Restart computer
   - Check Environment Variables (Windows):
     - Search "Environment Variables" in Windows
     - Ensure `C:\Program Files\dotnet\` is in PATH

### Issue: "Build Failed" with NuGet errors

**Cause:** Missing or corrupted NuGet packages

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Build again
dotnet build
```

### Issue: Port 5000/5001 already in use

**Cause:** Another application is using these ports

**Solution 1 - Stop other process:**
- Windows: Open Task Manager, find processes using port 5000/5001, end them
- Mac/Linux: `lsof -i :5000` then `kill -9 [PID]`

**Solution 2 - Change ports:**
1. Open `Properties/launchSettings.json`
2. Find:
```json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```
3. Change to:
```json
"applicationUrl": "https://localhost:7001;http://localhost:7000"
```

### Issue: Application starts but pages don't load

**Cause:** Database context initialization failed

**Solution:**
1. Check Output window for errors
2. Verify `Program.cs` database seeding section
3. Try clearing and rebuilding:
```bash
dotnet clean
dotnet build
dotnet run
```

### Issue: CSS/JavaScript not loading

**Cause:** Static files middleware not configured or files in wrong location

**Solution:**
1. Verify `app.UseStaticFiles();` is in `Program.cs`
2. Ensure files are in `wwwroot` folder:
   - `wwwroot/css/site.css`
   - `wwwroot/js/site.js`
3. Hard refresh browser: Ctrl+F5 (Windows) or Cmd+Shift+R (Mac)

### Issue: "Unable to resolve service" error

**Cause:** Service not registered in dependency injection container

**Solution:**
Check `Program.cs` contains:
```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

---

## 📖 UNDERSTANDING KEY CONCEPTS

### MVC Pattern

**Model-View-Controller** separates your application:

**Model** (`Models/` folder):
- Represents data structure
- Example: `Product.cs` defines what a product is

**View** (`Views/` folder):
- User interface (HTML)
- Example: `Product/Index.cshtml` shows product list

**Controller** (`Controllers/` folder):
- Handles user requests
- Example: `ProductController.cs` manages product pages

**Flow:**
```
User clicks link
    ↓
Controller receives request
    ↓
Controller gets data from Service/Database
    ↓
Controller sends data to View
    ↓
View renders HTML
    ↓
Browser displays page to User
```

### Dependency Injection

Instead of creating objects directly:
```csharp
// ❌ Bad - tight coupling
var context = new ApplicationDbContext();
var service = new ProductService(context);
```

Use injection:
```csharp
// ✅ Good - loose coupling
public ProductController(IProductService productService)
{
    _productService = productService;
}
```

**Benefits:**
- Easy to test (mock services)
- Easy to swap implementations
- Better code organization

### Async/Await

Many methods use `async` and `await`:
```csharp
public async Task<IEnumerable<Product>> GetAllProductsAsync()
{
    return await _context.Products.ToListAsync();
}
```

**Why?**
- Doesn't block the thread while waiting for database
- Application can handle more concurrent users
- Better performance

### Entity Framework Core

ORM (Object-Relational Mapping) - work with database using C# objects:

```csharp
// Instead of SQL:
// SELECT * FROM Products WHERE CategoryId = 1

// Use LINQ:
var products = await _context.Products
    .Where(p => p.CategoryId == 1)
    .ToListAsync();
```

---

## 🎓 LEARNING PATH

### Beginner Level
1. ✅ Run the application successfully
2. ✅ Browse the website and understand features
3. ✅ Read through the code with comments
4. ✅ Make simple changes (colors, text)
5. ✅ Add a new product through code

### Intermediate Level
6. Understand MVC pattern by tracing a request
7. Modify a view to add new functionality
8. Create a new controller action
9. Add a new property to Product model
10. Implement basic search improvements

### Advanced Level
11. Add shopping cart functionality
12. Implement user authentication
13. Create admin panel
14. Add image upload feature
15. Deploy to Azure/IIS

---

## 📚 ADDITIONAL RESOURCES

### Official Documentation
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- C# Guide: https://docs.microsoft.com/dotnet/csharp

### Video Tutorials
- Microsoft Learn: https://docs.microsoft.com/learn/
- YouTube: Search "ASP.NET Core MVC tutorial"

### Communities
- Stack Overflow: https://stackoverflow.com/questions/tagged/asp.net-core
- Reddit: r/dotnet, r/csharp
- ASP.NET Forums: https://forums.asp.net

---

## ✅ QUICK REFERENCE COMMANDS

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Clean build artifacts
dotnet clean

# Create migration (SQL Server only)
dotnet ef migrations add MigrationName

# Update database (SQL Server only)
dotnet ef database update

# List installed .NET SDKs
dotnet --list-sdks

# Check .NET version
dotnet --version
```

---

**Need More Help?**
- Re-read relevant sections of this guide
- Check the main README.md
- Review inline code comments
- Search error messages online
- Ask in developer communities

**You've got this! Happy coding! 🚀**
