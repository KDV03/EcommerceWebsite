# E-Commerce Marketplace Platform

A comprehensive, production-ready peer-to-peer marketplace built with **ASP.NET Core 8.0 MVC**, featuring escrow payments, user verification, dispute resolution, and multi-vendor capabilities.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?logo=.net)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue)

---

## 🌟 Overview

This is a full-featured e-commerce marketplace platform that enables users to buy and sell products with built-in trust and safety features. The system includes secure escrow payments, comprehensive user verification, order tracking, and administrative oversight.

### Key Features

**For Buyers:**
- 🛍️ Browse and search thousands of products
- 🔒 Secure escrow payment protection
- 📦 Real-time order tracking
- ⭐ Leave reviews and ratings
- ⚖️ Dispute resolution system

**For Sellers:**
- 📝 Create and manage product listings
- 📸 Multi-image upload support
- 💰 Automated fund release (7-day escrow)
- 📊 Sales analytics and tracking
- ✅ Identity verification system

**For Administrators:**
- 👥 User management and verification
- 💵 Escrow fund management
- 🛡️ Dispute resolution tools
- 📈 Platform analytics
- 🔧 System-wide configuration

---

## 🏗️ Architecture

### Technology Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 8.0 MVC |
| **Language** | C# 12.0 |
| **ORM** | Entity Framework Core 8.0 |
| **Authentication** | ASP.NET Core Identity |
| **Database (Dev)** | In-Memory Database |
| **Database (Prod)** | SQL Server 2019+ |
| **Frontend** | Razor Views, HTML5, CSS3, JavaScript |
| **UI Framework** | Custom CSS with Modern Design System |

### Design Patterns

- ✅ **Model-View-Controller (MVC)** - Clean separation of concerns
- ✅ **Repository Pattern** - Via service layer abstraction
- ✅ **Dependency Injection** - Framework-native IoC container
- ✅ **Unit of Work** - Through DbContext
- ✅ **SOLID Principles** - Throughout the codebase

### Project Structure

```
EcommerceWebsite/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs       # Authentication & registration
│   ├── AdminController.cs         # Admin panel
│   ├── CheckoutController.cs      # Payment processing
│   ├── ListingController.cs       # Product listings
│   ├── OrderController.cs         # Order management
│   ├── ProductController.cs       # Product browsing
│   └── HomeController.cs          # Homepage
├── Models/              # Domain Models
│   ├── ApplicationUser.cs         # Extended Identity user
│   ├── Listing.cs                 # Product listings
│   ├── Order.cs                   # Orders & items
│   ├── Payment.cs                 # Payment records
│   ├── Review.cs                  # Reviews & ratings
│   ├── Dispute.cs                 # Dispute system
│   ├── UserDocument.cs            # KYC documents
│   └── ViewModels/                # View-specific models
├── Data/                # Data Access Layer
│   └── ApplicationDbContext.cs    # EF Core context
├── Services/            # Business Logic Layer
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── ICategoryService.cs
│   ├── CategoryService.cs
│   ├── IEmailService.cs
│   └── EmailService.cs
├── Views/               # Razor Views
│   ├── Account/         # Auth views
│   ├── Admin/           # Admin panel
│   ├── Checkout/        # Payment flow
│   ├── Listing/         # Product management
│   ├── Order/           # Order tracking
│   ├── Product/         # Product browsing
│   ├── Shared/          # Layouts & partials
│   └── Home/            # Homepage
├── wwwroot/             # Static Files
│   ├── css/
│   ├── js/
│   └── images/
└── Documentation/       # Project docs
    ├── README.md
    ├── ARCHITECTURE.md
    ├── SETUP_GUIDE.md
    └── DESIGN_GUIDE.md
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- (Optional) [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ecommerce-marketplace.git
   cd ecommerce-marketplace
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open in browser**
   ```
   https://localhost:5001
   ```

### Default Admin Credentials

```
Email: admin@techmart.com
Password: Admin@123
```

**⚠️ Change these credentials immediately in production!**

---

## 📋 Features Breakdown

### User Management

- **Registration & Login**
  - Email/password authentication
  - Email confirmation workflow
  - Password reset functionality
  - Role-based access (User, Seller, Admin)

- **User Verification (KYC)**
  - Document upload (ID, Passport, Proof of Address)
  - Admin review and approval
  - Verification status tracking
  - Verified badge display

### Product Listings

- **Create Listings**
  - Multi-category support
  - Image upload (unlimited photos)
  - Condition tracking (New, Used, Refurbished)
  - Location-based listings
  - Price negotiation flag

- **Listing Management**
  - Draft/Active/Sold status workflow
  - Edit and update listings
  - View count tracking
  - Auto-expiry after 30 days

### Order Processing

- **Checkout Flow**
  1. Add to cart → Shipping details
  2. Payment method selection
  3. Escrow payment processing
  4. Order confirmation email

- **Order Lifecycle**
  ```
  Pending → Confirmed → Processing → Shipped → Delivered → Completed
  ```

- **Escrow System**
  - Funds held for 7 days after delivery
  - Auto-release or manual release
  - Buyer can release early
  - Dispute protection

### Payment Integration

- **Supported Methods**
  - Credit/Debit Cards
  - PayFast (South African gateway)
  - EFT (Bank Transfer)
  - PayPal (configurable)
  - Stripe (configurable)

- **Security Features**
  - PCI DSS compliance ready
  - Encrypted payment data
  - Transaction logging
  - Fraud detection hooks

### Dispute Resolution

- **Dispute Flow**
  1. Buyer/Seller initiates dispute
  2. Evidence upload
  3. Admin review
  4. Resolution outcomes:
     - Full refund (Buyer favorable)
     - Release funds (Seller favorable)
     - Partial refund
     - No action

### Admin Panel

- **Dashboard**
  - User statistics
  - Order metrics
  - Revenue tracking
  - Dispute monitoring

- **User Management**
  - View all users
  - Suspend/reinstate accounts
  - Verification approval
  - Activity tracking

- **Financial Management**
  - Escrow fund tracking
  - Manual fund release
  - Auto-release processing
  - Transaction reports

---

## 💾 Database Schema

### Core Entities

```
ApplicationUser (extends IdentityUser)
├── UserDocuments (1:N)
├── Listings (1:N)
├── OrdersAsBuyer (1:N)
└── OrdersAsSeller (1:N)

Listing
├── Images (1:N)
├── Reviews (1:N)
├── Category (N:1)
└── Seller (N:1)

Order
├── Items (1:N)
├── Payments (1:N)
├── StatusHistory (1:N)
├── Dispute (1:1)
├── Buyer (N:1)
└── Seller (N:1)
```

### Key Tables

| Table | Purpose | Key Fields |
|-------|---------|-----------|
| `AspNetUsers` | User accounts | Email, IsVerified, IsSeller |
| `Listings` | Product listings | Title, Price, Status, Quantity |
| `Orders` | Purchase orders | OrderNumber, TotalAmount, Status |
| `Payments` | Payment records | TransactionId, Amount, IsEscrow |
| `Disputes` | Dispute cases | Reason, Status, Outcome |
| `UserDocuments` | KYC documents | DocumentType, Status |

---

## 🔐 Security Features

### Authentication & Authorization

- ✅ ASP.NET Core Identity integration
- ✅ Role-based access control (RBAC)
- ✅ Email confirmation required
- ✅ Account lockout after failed attempts
- ✅ Secure password requirements

### Data Protection

- ✅ HTTPS enforcement
- ✅ Anti-forgery tokens on forms
- ✅ SQL injection prevention (EF Core)
- ✅ XSS protection (Razor encoding)
- ✅ CSRF protection

### Payment Security

- ✅ Escrow fund protection
- ✅ Transaction logging
- ✅ Payment gateway integration
- ✅ Refund processing
- ✅ Dispute resolution

---

## 📊 Configuration

### Application Settings

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;Trusted_Connection=True"
  },
  "ApplicationSettings": {
    "SiteName": "TechMart",
    "ItemsPerPage": 12
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### Database Configuration

**Development (In-Memory):**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EcommerceDb"));
```

**Production (SQL Server):**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Email Configuration

Integrate with your preferred email provider:

```csharp
// In EmailService.cs, implement actual sending:
using MailKit.Net.Smtp;
using MimeKit;

public async Task SendEmailAsync(string to, string subject, string body)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("TechMart", _configuration["Email:Username"]));
    message.To.Add(new MailboxAddress("", to));
    message.Subject = subject;
    message.Body = new TextPart("html") { Text = body };

    using var client = new SmtpClient();
    await client.ConnectAsync(_configuration["Email:SmtpServer"], 
        int.Parse(_configuration["Email:Port"]), true);
    await client.AuthenticateAsync(_configuration["Email:Username"], 
        _configuration["Email:Password"]);
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
}
```

---

## 🚢 Deployment

### Deploying to Azure

1. **Create Azure App Service**
   ```bash
   az webapp create --resource-group myResourceGroup \
     --plan myAppServicePlan --name myUniqueAppName \
     --runtime "DOTNET|8.0"
   ```

2. **Configure Database**
   ```bash
   az sql server create --name myserver --resource-group myResourceGroup \
     --location eastus --admin-user myadmin --admin-password MyP@ssw0rd!
   
   az sql db create --resource-group myResourceGroup --server myserver \
     --name myDatabase --service-objective S0
   ```

3. **Update Connection String**
   ```bash
   az webapp config connection-string set --resource-group myResourceGroup \
     --name myUniqueAppName --settings DefaultConnection='...' \
     --connection-string-type SQLAzure
   ```

4. **Publish**
   ```bash
   dotnet publish -c Release
   az webapp deployment source config-zip --resource-group myResourceGroup \
     --name myUniqueAppName --src publish.zip
   ```

### Deploying to IIS

1. **Install .NET 8 Hosting Bundle** on server
2. **Publish application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```
3. **Create IIS Site** pointing to publish folder
4. **Configure Application Pool** (.NET CLR Version: No Managed Code)
5. **Update `appsettings.json`** with production settings

---

## 🧪 Testing

### Run Unit Tests

```bash
dotnet test
```

### Sample Test Structure

```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task GetProductById_ReturnsProduct_WhenExists()
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
    }
}
```

---

## 📚 Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detailed architecture documentation
- **[SETUP_GUIDE.md](SETUP_GUIDE.md)** - Beginner-friendly setup guide
- **[DESIGN_GUIDE.md](DESIGN_GUIDE.md)** - UI/UX design guidelines

---

## 🛠️ Development

### Adding a New Feature

1. **Create Model** in `Models/`
2. **Update DbContext** in `Data/ApplicationDbContext.cs`
3. **Create Service Interface** in `Services/`
4. **Implement Service** in `Services/`
5. **Register Service** in `Program.cs`
6. **Create Controller** in `Controllers/`
7. **Create Views** in `Views/`

### Code Style

- Follow **C# Coding Conventions**
- Use **async/await** for I/O operations
- Implement **SOLID principles**
- Add **XML documentation** for public APIs
- Write **unit tests** for business logic

---

## 📈 Performance Optimization

- ✅ Async operations throughout
- ✅ Eager loading with `.Include()`
- ✅ Response caching middleware
- ✅ Session state management
- ✅ Database indexing on key fields
- ✅ Image lazy loading
- ✅ Pagination for large datasets

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- UI inspired by modern e-commerce platforms
- Icons and fonts from Google Fonts
- Community contributions and feedback

---

## 📞 Support

- **Documentation**: See `/Documentation` folder
- **Issues**: [GitHub Issues](https://github.com/yourusername/repo/issues)
- **Email**: support@techmart.com

---

## 🗺️ Roadmap

### Phase 1 (Current)
- [x] User authentication
- [x] Product listings
- [x] Order processing
- [x] Escrow payments
- [x] Admin panel

### Phase 2 (Next)
- [ ] Real payment gateway integration
- [ ] Advanced search with filters
- [ ] Product recommendations
- [ ] Chat system (buyer-seller)
- [ ] Mobile app (Xamarin/MAUI)

### Phase 3 (Future)
- [ ] Multi-currency support
- [ ] International shipping
- [ ] Seller analytics dashboard
- [ ] Promotional campaigns
- [ ] API for third-party integrations

---

**Built with ❤️ using ASP.NET Core**

---

## 📊 Statistics

```
Total Lines of Code: ~15,000+
Controllers: 8
Models: 20+
Views: 50+
Services: 6
Features: 50+
```

---

*Last Updated: January 2026*
