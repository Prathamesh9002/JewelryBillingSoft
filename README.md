# 💎 Jewelry Billing Soft

A professional, commercial-grade billing software for jewelry shops — built with .NET 8, WPF, and SQL Server Express.

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![Architecture](https://img.shields.io/badge/Architecture-MVVM-green)
![Database](https://img.shields.io/badge/Database-SQL%20Server%20Express-red)

---

## ✨ Features

### 🧾 Billing & Invoice Management
- Create professional jewelry invoices with full GST support
- Handle gold, silver, diamond, and custom ornaments
- Calculate: gross weight, stone weight, net weight, purity, rate per gram, making charges, wastage %, GST, discounts
- Track advance payments and balance due
- Print / export invoices as PDF

### 📦 Inventory Management
- Add, edit, delete, and track jewelry items
- Item code, category, metal type, purity, weight, price, stock
- Track sold, returned, and available stock
- Low stock alerts
- Search and filter by metal type

### 👥 Customer Management
- Customer profiles: name, mobile, address, email, PAN, Aadhaar
- Quick search during billing
- Full purchase history per customer
- Loyalty points tracking

### 💳 Payment Tracking
- Cash, Card, UPI, and mixed payments
- Partial payments and pending balance tracking
- Refund/return processing
- Daily and monthly sales summaries

### 📊 Reports & Analytics
- Daily Sales Report
- Monthly Sales Report
- Stock Report
- Customer Purchase Report
- Tax Summary (CGST + SGST)
- Profit & Loss Overview
- Pending Payments Report

### 🔑 Licensing System
- 1-year license validity
- Secure local activation
- Expiry check on startup
- Renewal support
- Machine-bound activation
- 30-day expiry warning

### 🔒 Security
- Role-based access: Admin, Manager, Cashier
- Password hashing (SHA-256)
- Audit logging
- Input validation

---

## 🏗️ Architecture

```
JewelryBillingSoft/
├── src/
│   ├── JewelryBillingSoft.Domain/          # Entities, Enums
│   ├── JewelryBillingSoft.Application/     # Services, Interfaces, DTOs
│   ├── JewelryBillingSoft.Infrastructure/  # EF Core, Repositories, PDF, DI
│   └── JewelryBillingSoft.UI/              # WPF MVVM UI
├── database/
│   └── setup.sql
└── docs/
```

**Layers:**
- **Domain**: Entities (User, Customer, Product, Invoice, Payment, LicenseInfo, AuditLog)
- **Application**: Business logic services (BillingService, CustomerService, InventoryService, ReportService, LicenseService, AuthService)
- **Infrastructure**: EF Core DbContext, Repository pattern, Unit of Work, PDF printing (PdfSharpCore), DI setup
- **UI**: WPF with MVVM via CommunityToolkit.Mvvm, Material Design UI

---

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or named instance)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/prajput_RSCP/JewelryBillingSoft.git
   cd JewelryBillingSoft
   ```

2. **Configure connection string**
   
   Edit `src/JewelryBillingSoft.UI/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Install dependencies and run EF migrations**
   ```bash
   cd src/JewelryBillingSoft.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../JewelryBillingSoft.UI
   dotnet ef database update --startup-project ../JewelryBillingSoft.UI
   ```

4. **Build and Run**
   ```bash
   cd src/JewelryBillingSoft.UI
   dotnet run
   ```

### First Launch
1. **License Activation**: Use any 16+ character key (e.g., `DEMO-DEMO-DEMO-DEMO`)
2. **Login**: `admin` / `Admin@123`
3. Start billing!

---

## 📱 Screens

| Screen | Description |
|--------|-------------|
| 🔐 Login | Secure login with role-based access |
| 📊 Dashboard | Sales summary, stats, recent invoices |
| 🧾 Billing | Create invoices, add items, process payments |
| 👥 Customers | Manage customer database |
| 📦 Inventory | Track jewelry stock |
| 📈 Reports | Generate business reports |
| ⚙️ Settings | Shop configuration |

---

## 🛡️ Default Credentials

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Administrator |

> ⚠️ Change the default password after first login!

---

## 🔧 Technology Stack

| Component | Technology |
|-----------|-----------|
| UI | WPF (.NET 8) |
| Architecture | MVVM (CommunityToolkit.Mvvm) |
| Database | SQL Server Express |
| ORM | Entity Framework Core 8 |
| UI Theme | Material Design in XAML |
| PDF | PdfSharpCore |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |

---

## 📦 Deployment

1. **Build Release**
   ```bash
   dotnet publish src/JewelryBillingSoft.UI -c Release -r win-x64 --self-contained
   ```

2. **Create installer** using [Inno Setup](https://jrsoftware.org/isinfo.php) or [NSIS](https://nsis.sourceforge.io/)

3. **Database**: App auto-creates the database on first run via EF Core migrations

---

## 📄 License

This software is proprietary. Requires a valid license key for use.

---

## 📞 Support

For license keys and support, contact: support@jewelrysoft.com

