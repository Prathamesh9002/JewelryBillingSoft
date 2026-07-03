# Local Setup Guide — Jewelry Billing Soft

## ✅ Prerequisites Checklist

Before starting, make sure you have these installed on your Windows PC:

| Tool | Download Link | Check if Installed |
|------|--------------|-------------------|
| **.NET 8 SDK** | https://dotnet.microsoft.com/download/dotnet/8 | Run: `dotnet --version` |
| **SQL Server Express** | https://www.microsoft.com/en-us/sql-server/sql-server-downloads | Check Services |
| **Git** | https://git-scm.com/download/win | Run: `git --version` |
| **Visual Studio 2022** *(optional)* | https://visualstudio.microsoft.com/ | For IDE development |

---

## 🚀 Quick Setup (Automated Script)

> **Easiest way** — just run the PowerShell setup script:

```powershell
# Open PowerShell as Administrator and run:
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\setup-local.ps1
```

---

## 🔧 Manual Step-by-Step Setup

### Step 1 — Clone the Repository

Open **PowerShell** or **Command Prompt** and run:

```bash
git clone https://github.com/Prathamesh9002/JewelryBillingSoft.git
cd JewelryBillingSoft
```

---

### Step 2 — Verify .NET 8 SDK

```powershell
dotnet --version
# Should show: 8.x.x
```

If not installed, download from: https://dotnet.microsoft.com/download/dotnet/8  
Choose: **.NET 8 SDK (Windows x64)**

---

### Step 3 — Set Up SQL Server Express

#### Option A — Use SQL Server Express (Recommended)
1. Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Click **"Download now"** under **Express** (free edition)
3. Run the installer → Choose **"Basic"** installation
4. Note the **connection string** shown at the end (e.g., `Server=localhost\SQLEXPRESS`)

#### Option B — Use SQL Server LocalDB (Developer shortcut)
LocalDB comes with Visual Studio. Check if it's available:
```powershell
sqllocaldb info
# If available, use this connection string:
# Server=(localdb)\MSSQLLocalDB;Database=JewelryBillingSoftDb;Trusted_Connection=True;
```

---

### Step 4 — Configure Connection String

Open `src/JewelryBillingSoft.UI/appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Common connection string formats:**

| SQL Server Setup | Connection String |
|-----------------|------------------|
| SQL Server Express (default) | `Server=.\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server LocalDB | `Server=(localdb)\MSSQLLocalDB;Database=JewelryBillingSoftDb;Trusted_Connection=True;` |
| Named instance | `Server=MYPC\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server with login | `Server=.\SQLEXPRESS;Database=JewelryBillingSoftDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;` |

---

### Step 5 — Restore NuGet Packages

```powershell
cd C:\JewelryBillingSoft
dotnet restore JewelryBillingSoft.sln
```

---

### Step 6 — Install EF Core Tools & Run Migrations

```powershell
# Install EF Core CLI tools (one-time)
dotnet tool install --global dotnet-ef

# Verify
dotnet ef --version

# Create & apply database migrations
cd src\JewelryBillingSoft.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\JewelryBillingSoft.UI

# Apply migrations (creates the database and all tables)
dotnet ef database update --startup-project ..\JewelryBillingSoft.UI
```

---

### Step 7 — Build the Project

```powershell
cd C:\JewelryBillingSoft
dotnet build JewelryBillingSoft.sln
```

Expected output: `Build succeeded.`

---

### Step 8 — Run the Application

```powershell
cd src\JewelryBillingSoft.UI
dotnet run
```

The app will launch. On first run:
1. **License Activation screen** → Enter any 16+ character key (e.g., `DEMO-DEMO-DEMO-DEMO1`) + shop name
2. **Login screen** → Username: `admin`, Password: `Admin@123`
3. **Dashboard** opens 🎉

---

## 🖥️ Using Visual Studio 2022 (Alternative to CLI)

1. Open **Visual Studio 2022**
2. Click **"Open a project or solution"**
3. Browse to `C:\JewelryBillingSoft\JewelryBillingSoft.sln`
4. Wait for NuGet packages to restore automatically
5. Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
6. Run:
   ```
   Update-Database
   ```
7. Set **JewelryBillingSoft.UI** as the Startup Project (right-click → Set as Startup Project)
8. Press **F5** to run

---

## ✅ Verify Everything Works

After running the app:

1. ✅ License activation screen appears
2. ✅ Enter shop name + any license key → click Activate
3. ✅ Login screen appears → `admin` / `Admin@123`
4. ✅ Dashboard loads with sample data
5. ✅ Navigation works (Billing, Customers, Inventory, Reports)

---

## 🐛 Common Errors & Fixes

### ❌ `No .NET SDKs were found`
**Fix:** Install .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8

---

### ❌ `A network-related or instance-specific error` (DB connection)
**Fix:** SQL Server Express is not running.
```powershell
# Start SQL Server Express service
Start-Service -Name "MSSQL`$SQLEXPRESS"
```
Or open **Services** (Win+R → `services.msc`) → Find **SQL Server (SQLEXPRESS)** → Start

---

### ❌ `dotnet-ef: command not found`
**Fix:**
```powershell
dotnet tool install --global dotnet-ef
# Then restart your terminal
```

---

### ❌ `Could not execute because the specified command or file was not found` (migrations)
**Fix:** Make sure you are in the correct folder:
```powershell
cd C:\JewelryBillingSoft\src\JewelryBillingSoft.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\JewelryBillingSoft.UI
```

---

### ❌ Build errors / missing packages
**Fix:**
```powershell
cd C:\JewelryBillingSoft
dotnet restore
dotnet build
```

---

### ❌ `Login failed` in app
The seed data sets admin password to `Admin@123`. If it doesn't work, reset via SQL:
```sql
USE JewelryBillingSoftDb;
UPDATE Users SET PasswordHash = 'gGcG1J7LFqFiZABa0e2/JwKVh/NxqaXY7cjA1Lne8Bk=' WHERE Username = 'admin';
```

---

## 📁 Project Structure Reference

```
JewelryBillingSoft/
├── JewelryBillingSoft.sln          ← Open this in Visual Studio
├── src/
│   ├── JewelryBillingSoft.Domain/          ← Entities & Enums
│   ├── JewelryBillingSoft.Application/     ← Business Logic & Services
│   ├── JewelryBillingSoft.Infrastructure/  ← Database, Repositories
│   └── JewelryBillingSoft.UI/              ← WPF App (Entry Point)
│       └── appsettings.json                ← ⚙️ Edit DB connection here
├── database/
│   └── setup.sql
└── docs/
    └── USER_MANUAL.md
```

---

## 🔗 Useful Links

- 📦 .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8
- 🗄️ SQL Server Express: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- 🛠️ Visual Studio 2022: https://visualstudio.microsoft.com/
- 📖 User Manual: [docs/USER_MANUAL.md](docs/USER_MANUAL.md)
- 💻 Repository: https://github.com/Prathamesh9002/JewelryBillingSoft

