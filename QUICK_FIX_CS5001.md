# QUICK FIX: CS5001 Error - Missing Main Method

## The Problem
Error CS5001: "Program does not contain a static 'Main' method suitable for an entry point"

This happens because Visual Studio isn't recognizing the top-level statements in Program.cs.

---

## ✅ SOLUTION 1: Edit Your .csproj File Directly (RECOMMENDED)

### Step 1: Close Visual Studio

### Step 2: Open the .csproj file in Notepad
1. Navigate to your project folder
2. Right-click `EcommerceWebsite.csproj`
3. Choose "Open with" → Notepad

### Step 3: Verify it looks like this:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.0" />
  </ItemGroup>

</Project>
```

**Key points to check:**
- Line 1: MUST be `Sdk="Microsoft.NET.Sdk.Web"` (not Sdk.Console)
- Line 7: MUST have `<OutputType>Exe</OutputType>`
- Line 6: MUST have `<ImplicitUsings>enable</ImplicitUsings>`

### Step 4: Save and Reopen Visual Studio
1. Save the file
2. Open Visual Studio
3. Open the project
4. Clean Solution (Build → Clean Solution)
5. Rebuild Solution (Build → Rebuild Solution)

---

## ✅ SOLUTION 2: Delete obj and bin Folders

### Step 1: Close Visual Studio

### Step 2: Delete Build Artifacts
Navigate to your project folder and delete:
- `bin` folder (if exists)
- `obj` folder (if exists)

### Step 3: Reopen and Rebuild
1. Open Visual Studio
2. Open the project
3. Restore NuGet packages (right-click solution → Restore NuGet Packages)
4. Rebuild (Ctrl+Shift+B)

---

## ✅ SOLUTION 3: Use Command Line

Open Command Prompt or PowerShell in your project directory:

```bash
# Navigate to project
cd "D:\OneDrive - O Fearghail (Pty) Ltd TA ProbityX\Documents\EcommerceWebsite"

# Clean everything
dotnet clean

# Remove obj and bin folders
rmdir /s /q obj
rmdir /s /q bin

# Restore packages
dotnet restore

# Build
dotnet build

# If build succeeds, run it
dotnet run
```

---

## ✅ SOLUTION 4: Verify Program.cs Content

Your Program.cs should start like this:

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EcommerceDb"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddControllersWithViews();
builder.Services.AddResponseCaching();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ... rest of the code
```

**Make sure there is NO** `static void Main()` or `class Program` - we're using top-level statements!

---

## ✅ SOLUTION 5: Check Visual Studio Version

This project requires:
- **Visual Studio 2022** (any edition)
- **.NET 8 SDK**

### Verify .NET Version:
Open Command Prompt and type:
```bash
dotnet --version
```

Should show: `8.0.xxx`

If not, download .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0

### Verify Visual Studio 2022:
Help → About Microsoft Visual Studio

Should show: Version 17.x.x

---

## ✅ SOLUTION 6: Create New Project File

If nothing works, replace your .csproj with this exact content:

1. **Close Visual Studio**
2. **Backup your current .csproj** (just in case)
3. **Open EcommerceWebsite.csproj in Notepad**
4. **Replace entire content** with:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.0" />
  </ItemGroup>

</Project>
```

5. **Save and close Notepad**
6. **Open Visual Studio**
7. **Clean and Rebuild**

---

## 🔍 Diagnostic: What's Causing This?

The CS5001 error with top-level statements typically occurs when:

1. **Wrong SDK type** - Using `Microsoft.NET.Sdk` instead of `Microsoft.NET.Sdk.Web`
2. **Cached build artifacts** - Old obj/bin folders causing confusion
3. **ImplicitUsings not enabled** - Top-level statements need this
4. **Old Visual Studio version** - Need VS 2022 for .NET 8
5. **Corrupt NuGet cache** - Packages not restored properly

---

## 📊 Quick Checklist

Before asking for more help, verify:

- [ ] .NET 8 SDK is installed (`dotnet --version` shows 8.0.xxx)
- [ ] Using Visual Studio 2022 (not 2019)
- [ ] .csproj has `Sdk="Microsoft.NET.Sdk.Web"`
- [ ] .csproj has `<ImplicitUsings>enable</ImplicitUsings>`
- [ ] Deleted obj and bin folders
- [ ] Ran `dotnet restore`
- [ ] Program.cs uses top-level statements (no Main method)
- [ ] Closed and reopened Visual Studio
- [ ] Tried Clean Solution then Rebuild

---

## 🎯 Most Common Fix

**99% of the time, this works:**

1. Close Visual Studio
2. Delete `obj` and `bin` folders
3. Open Command Prompt in project folder
4. Run: `dotnet clean`
5. Run: `dotnet restore`
6. Run: `dotnet build`
7. If successful, open Visual Studio and run (F5)

---

## 💡 Still Not Working?

Try running via command line to get more detailed error:

```bash
cd "D:\OneDrive - O Fearghail (Pty) Ltd TA ProbityX\Documents\EcommerceWebsite"
dotnet run --verbosity detailed
```

This will show exactly what's failing.

---

## 📞 Last Resort: Manual Main Method

If you absolutely cannot get top-level statements working, you can convert Program.cs to use a traditional Main method:

```csharp
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.Data;
using EcommerceWebsite.Services;

namespace EcommerceWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("EcommerceDb"));

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddResponseCaching();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }

            // Configure pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseResponseCaching();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "productDetails",
                pattern: "Product/Details/{id:int}",
                defaults: new { controller = "Product", action = "Details" });

            app.MapControllerRoute(
                name: "productCategory",
                pattern: "Product/Category/{id:int}",
                defaults: new { controller = "Product", action = "Category" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
```

This is the "old-style" way but will definitely work!

---

**Try Solution 1 first - it fixes 95% of cases!**
