# 📖 Jewelry Billing Soft — User Manual

**Version:** 1.0.0  
**Platform:** Windows Desktop  
**Last Updated:** July 2026

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [System Requirements](#2-system-requirements)
3. [Installation & Setup](#3-installation--setup)
4. [License Activation](#4-license-activation)
5. [Login & User Roles](#5-login--user-roles)
6. [Dashboard](#6-dashboard)
7. [Billing & Invoice Management](#7-billing--invoice-management)
8. [Customer Management](#8-customer-management)
9. [Inventory Management](#9-inventory-management)
10. [Payments & Pending Dues](#10-payments--pending-dues)
11. [Reports & Analytics](#11-reports--analytics)
12. [Settings](#12-settings)
13. [License Renewal](#13-license-renewal)
14. [Keyboard Shortcuts](#14-keyboard-shortcuts)
15. [Troubleshooting](#15-troubleshooting)
16. [FAQs](#16-faqs)
17. [Support](#17-support)

---

## 1. Introduction

**Jewelry Billing Soft** is a professional, offline-first billing and management software designed specifically for jewelry shops. It helps shop owners and staff to:

- Create and print GST-compliant invoices
- Manage jewelry inventory (gold, silver, diamond)
- Maintain customer records and purchase history
- Accept cash, card, and UPI payments
- Generate daily, monthly, and tax reports
- Manage the business securely with role-based access

The software runs completely **offline** on Windows — no internet connection required for daily operations.

---

## 2. System Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| OS | Windows 10 (64-bit) | Windows 11 (64-bit) |
| RAM | 4 GB | 8 GB |
| Disk Space | 500 MB | 2 GB |
| Processor | Intel Core i3 / AMD equivalent | Intel Core i5 or higher |
| Display | 1280 × 720 | 1920 × 1080 |
| .NET Runtime | .NET 8 Desktop Runtime | .NET 8 Desktop Runtime |
| Database | SQL Server Express 2019+ | SQL Server Express 2022 |
| Printer | Any Windows-compatible printer | Laser printer (for invoices) |

---

## 3. Installation & Setup

### Step 1 — Install Prerequisites

1. **Install .NET 8 Desktop Runtime**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8
   - Run the installer and follow the prompts

2. **Install SQL Server Express**
   - Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Choose **"Express"** edition (free)
   - During setup, note the **instance name** (usually `.\SQLEXPRESS`)

### Step 2 — Configure the Application

Open the file `appsettings.json` in the application folder and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> **Note:** If your SQL Server instance has a different name, replace `SQLEXPRESS` with your instance name.

### Step 3 — First Launch

1. Double-click `JewelryBillingSoft.exe` to launch the application
2. The app will **automatically create the database** on first run
3. The **License Activation** screen will appear (see Section 4)

---

## 4. License Activation

When you launch the software for the first time (or after license expiry), the **License Activation** screen appears.

### How to Activate

| Field | Description |
|-------|-------------|
| **Shop Name** | Your jewelry shop's official name |
| **Email** | Your registered email address |
| **License Key** | The 16-character key provided at purchase |

**Steps:**
1. Enter your **Shop Name**
2. Enter your **Email** address
3. Enter the **License Key** (format: `XXXX-XXXX-XXXX-XXXX`)
4. Click **"🔑 Activate License"**
5. On success, you will be redirected to the Login screen

> **Demo/Trial:** Use key `DEMO-DEMO-DEMO-DEMO1` to activate a trial license.

### License Validity
- License is valid for **1 year** from the activation date
- You will receive a **warning 30 days before expiry**
- After expiry, the software will prompt for renewal before allowing access

---

## 5. Login & User Roles

### Logging In

1. Launch the application
2. Enter your **Username** and **Password**
3. Click **"Sign In"**

### Default Admin Credentials

| Username | Password |
|----------|----------|
| `admin` | `Admin@123` |

> ⚠️ **Important:** Change the default password immediately after first login via **Settings → Change Password**.

### User Roles

| Role | Permissions |
|------|------------|
| **Admin** | Full access — billing, inventory, customers, reports, settings, user management |
| **Manager** | Billing, inventory, customers, reports (no settings/user management) |
| **Cashier** | Billing and customer lookup only |

---

## 6. Dashboard

The **Dashboard** is the home screen shown after login. It provides a quick overview of the business.

### Dashboard Cards

| Card | What It Shows |
|------|--------------|
| **Today's Sales** | Total invoice amount for today |
| **Monthly Sales** | Total sales for the current month |
| **Total Customers** | Number of registered customers |
| **Pending Amount** | Total unpaid balance across all invoices |
| **Total Products** | Number of products in inventory |
| **Low Stock Alert** | Number of items with stock ≤ 5 units |

### Recent Invoices Table

The bottom section shows the **last 7 days of invoices** with:
- Invoice number, date, customer name
- Total amount, paid amount, pending balance
- Status (Completed / Partially Paid / Cancelled)

### Navigation Sidebar

Use the **left sidebar** to navigate between modules:

| Icon | Module |
|------|--------|
| 📊 | Dashboard |
| 🧾 | Billing |
| 👥 | Customers |
| 📦 | Inventory |
| 📈 | Reports |
| ⚙️ | Settings |
| 🚪 | Logout |

> **Tip:** Click the **☰ (hamburger)** button at the top of the sidebar to collapse or expand it.

---

## 7. Billing & Invoice Management

The Billing module is the core of the application. Use it to create, manage, and print invoices.

### 7.1 Creating a New Invoice

1. Click **"🧾 Billing"** in the sidebar
2. Click **"+ New Invoice"** button (top right)
3. The **New Invoice form** opens on the right panel

#### Step-by-Step Invoice Creation

**Step 1 — Select Customer**
- Click the **Customer** dropdown
- Type to search or scroll to find the customer
- Select the customer from the list
- If the customer is new, go to **Customers** module first to add them

**Step 2 — Add Items**

For each jewelry item:

| Field | Description | Example |
|-------|-------------|---------|
| **Product** | Select from inventory dropdown | Gold Ring 22K |
| **Qty** | Number of pieces | 1 |
| **Net Wt (g)** | Net weight in grams | 4.25 |
| **Rate/g (₹)** | Current gold/silver rate per gram | 6200 |
| **Making (₹)** | Making charges | 1500 |
| **Wastage %** | Wastage percentage | 2 |
| **Discount %** | Discount on the item | 0 |

After filling in the fields, click **"+ Add Item"**.

> **Auto-fill:** When you select a product, its stored weight, making charges, and wastage % are pre-filled automatically. You can override them.

**How the Amount is Calculated:**
```
Metal Value    = Net Weight × Rate per Gram
Wastage Amount = Metal Value × (Wastage% / 100)
Sub Total      = (Metal Value + Wastage + Making Charges) × Quantity
Discount       = Sub Total × (Discount% / 100)
Taxable Amount = Sub Total - Discount
GST Amount     = Taxable Amount × (GST% / 100)
Total          = Taxable Amount + GST Amount
```

**Step 3 — Review Items**
- The **Invoice Items** table shows all added items
- Click **✕** next to any item to remove it
- The **Grand Total** updates automatically

**Step 4 — Process Payment**

| Payment Field | Description |
|---------------|-------------|
| **Cash (₹)** | Amount paid in cash |
| **Card (₹)** | Amount paid by debit/credit card |
| **UPI (₹)** | Amount paid via UPI |
| **UPI Reference** | Transaction ID from UPI app |

> **Partial Payment:** You can leave the total payment less than the invoice amount — the pending balance will be tracked automatically.

**Step 5 — Save Invoice**
- Click **"✅ Save & Print"** to finalize and generate the PDF invoice
- The invoice PDF will open automatically for printing

### 7.2 Viewing & Searching Invoices

- Use the **date range pickers** (From / To) at the top and click **"🔍 Filter"**
- The invoice list shows all invoices for the selected period
- **Double-click** any invoice to view its full details on the right panel

### 7.3 Printing an Invoice

1. Select an invoice from the list (double-click)
2. Click the **"🖨️ Print"** button on the invoice detail panel
3. The PDF invoice opens in your default PDF viewer
4. Use the viewer's print function (Ctrl+P)

**The printed invoice includes:**
- Shop name, address, and GST number
- Customer details
- Item-wise breakup (weight, purity, rate, making charges, GST)
- Payment summary
- Pending balance (if any)
- Terms & conditions
- Authorized signature line

### 7.4 Invoice Statuses

| Status | Meaning |
|--------|---------|
| **Draft** | Invoice created but not finalized |
| **Completed** | Fully paid |
| **Partially Paid** | Payment received but balance pending |
| **Cancelled** | Invoice cancelled |

---

## 8. Customer Management

### 8.1 Adding a New Customer

1. Click **"👥 Customers"** in the sidebar
2. Click **"+ Add Customer"**
3. Fill in the details:

| Field | Required | Description |
|-------|----------|-------------|
| Name | ✅ Yes | Full name of the customer |
| Mobile | ✅ Yes | Primary mobile number |
| Alternate Mobile | No | Secondary contact number |
| Email | No | Email address |
| Address | No | Full street address |
| City | No | City name |
| State | No | State name |
| PAN Number | No | PAN card number (for high-value purchases) |
| Aadhaar Number | No | Aadhaar card number |

4. Click **"💾 Save"**

A unique **Customer Code** (e.g., `CUST0001`) is automatically generated.

### 8.2 Searching Customers

- Type in the **search bar** at the top (searches by name, mobile, or customer code)
- Click **"🔍 Search"** or press Enter

### 8.3 Editing a Customer

1. Click on the customer in the list to select them
2. Click **"✏️ Edit"**
3. Modify the required fields
4. Click **"💾 Save"**

### 8.4 Deleting a Customer

1. Select the customer from the list
2. Click **"🗑️ Delete"**
3. Confirm the deletion in the popup dialog

> ⚠️ **Note:** Customers with existing invoices cannot be deleted. You can deactivate them instead.

### 8.5 Customer Purchase History

Click on any customer to see their:
- Total number of invoices
- Total purchase amount
- All previous invoices with dates and amounts

---

## 9. Inventory Management

### 9.1 Adding a New Product / Jewelry Item

1. Click **"📦 Inventory"** in the sidebar
2. Click **"+ Add Product"**
3. Fill in the product details:

| Field | Description | Example |
|-------|-------------|---------|
| **Name** | Product name | Gold Necklace 22K |
| **Metal Type** | Gold / Silver / Diamond / Platinum | Gold |
| **Purity** | Purity type | Gold22K |
| **Gross Weight** | Total weight including stones (grams) | 15.50 |
| **Stone Weight** | Weight of stones (grams) | 1.20 |
| **Net Weight** | Metal weight = Gross - Stone (grams) | 14.30 |
| **Purchase Price** | Cost price (₹) | 88000 |
| **Selling Price** | MRP / selling price (₹) | 98000 |
| **Making Charges** | Making charges (₹) | 3500 |
| **Wastage %** | Wastage percentage | 2.0 |
| **Stock Quantity** | Number of pieces in stock | 3 |
| **GST %** | GST rate (default: 3% for jewelry) | 3 |
| **HSN Code** | HSN code for GST filing | 7113 |
| **Barcode** | Barcode number (optional) | 8901234567890 |

4. Click **"💾 Save"**

A unique **Item Code** is automatically generated (e.g., `G0001` for Gold, `S0001` for Silver, `D0001` for Diamond).

### 9.2 Stock Summary (Top Cards)

| Card | Description |
|------|-------------|
| **Total Items** | All products in the system |
| **Gold Items** | Count of gold jewelry |
| **Diamond Items** | Count of diamond jewelry |
| **Total Value** | Total inventory value at selling price |

### 9.3 Filtering Inventory

- **Search Box:** Type item name, code, or barcode
- **Metal Type Filter:** Filter by Gold / Silver / Diamond / All
- Click **"🔍 Search"** to apply filters

### 9.4 Editing Product Details

1. Click on the product in the grid
2. Click **"✏️ Edit"**
3. Update the fields
4. Click **"💾 Save"**

### 9.5 Stock Status

| Status | Meaning |
|--------|---------|
| **Available** | In stock and ready to sell |
| **Sold** | Sold out (stock = 0) |
| **Reserved** | Reserved for a customer |
| **Returned** | Returned item |

> **Auto-Update:** When an invoice is finalized, stock quantity is automatically reduced.

### 9.6 Low Stock Alert

Items with **stock quantity ≤ 5** appear in the **Low Stock Alert** on the Dashboard. Restock these items promptly.

---

## 10. Payments & Pending Dues

### Recording a Payment on an Existing Invoice

1. Go to **Billing**
2. Double-click the invoice with a pending balance
3. Click **"+ Add Payment"** (if visible) or create a new invoice with the balance
4. Select payment method and enter amount
5. Save

### Payment Methods Supported

| Method | When to Use |
|--------|------------|
| **Cash** | Physical currency |
| **Card** | Debit or credit card (swipe/tap) |
| **UPI** | GPay, PhonePe, Paytm, etc. |
| **Mixed** | Combination of above |

> **Tip:** For UPI payments, always enter the **Transaction Reference ID** for reconciliation.

---

## 11. Reports & Analytics

### Accessing Reports

1. Click **"📈 Reports"** in the sidebar
2. Select the **Report Type** from the dropdown
3. Set the **date range** (From / To)
4. Click **"📊 Generate"**

### Available Reports

#### 📅 Daily Sales Report
- **Purpose:** End-of-day summary
- **Shows:** Total invoices, total sales, cash/card/UPI breakup, pending amount, items sold
- **Use When:** Daily cash closing

#### 📆 Monthly Sales Report
- **Purpose:** Monthly business overview
- **Shows:** Total monthly revenue, invoice count, day-wise breakdown
- **Use When:** Monthly review, GST filing

#### 📦 Stock Report
- **Purpose:** Current inventory status
- **Shows:** Total products, available/sold counts, total inventory value, low-stock items
- **Use When:** Stock auditing, reorder planning

#### 👥 Customer Purchase Report
- **Purpose:** Customer-wise sales analysis
- **Shows:** Per-customer invoice history, total spend, pending dues
- **Use When:** Identifying top customers, follow-ups

#### 🧾 Tax Summary Report (GST Report)
- **Purpose:** GST filing assistance
- **Shows:** HSN-wise taxable amount, CGST, SGST, total tax collected
- **Use When:** Monthly/quarterly GST return filing

#### 📊 Profit & Loss Report
- **Purpose:** Business profitability analysis
- **Shows:** Total revenue, cost of goods, making charges revenue, discounts, net profit
- **Use When:** Business performance review

#### ⏳ Pending Payments Report
- **Purpose:** Follow up on outstanding dues
- **Shows:** All invoices with pending balances, days overdue, customer contact
- **Use When:** Daily collection follow-ups

---

## 12. Settings

### Shop Details

1. Go to **⚙️ Settings**
2. Update your shop information:
   - **Shop Name** (appears on invoices)
   - **Address** (appears on invoices)
   - **Mobile** (appears on invoices)
   - **Email**
   - **GST Number** (required for GST-compliant invoices)
3. Click **"💾 Save Settings"**

### Changing Your Password

1. Go to **⚙️ Settings → Change Password**
2. Enter **Current Password**
3. Enter **New Password** (minimum 8 characters)
4. Enter **Confirm Password**
5. Click **"🔒 Change Password"**

> **Password Requirements:**
> - Minimum 8 characters
> - Use a mix of letters, numbers, and symbols for security

---

## 13. License Renewal

### Checking License Status

1. Go to **⚙️ Settings** (Admin only)
2. The license expiry date and remaining days are displayed

### Renewing Before Expiry

1. Purchase a new license key from your vendor
2. Go to **Settings → License**
3. Enter the new license key
4. Click **"Renew License"**

### Renewing After Expiry

1. The software will show the **License Activation** screen on startup
2. Enter your new license key along with shop name and email
3. Click **"🔑 Activate License"**

> **Important:** Keep your license key safe. Contact support if you lose it.

---

## 14. Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Alt + D` | Go to Dashboard |
| `Alt + B` | Go to Billing |
| `Alt + C` | Go to Customers |
| `Alt + I` | Go to Inventory |
| `Alt + R` | Go to Reports |
| `Ctrl + N` | New Invoice (in Billing screen) |
| `Ctrl + P` | Print current invoice |
| `Ctrl + F` | Focus search bar |
| `Escape` | Cancel current form / close panel |
| `F5` | Refresh current view |

---

## 15. Troubleshooting

### ❌ "Database initialization failed" on startup

**Cause:** SQL Server Express is not running or connection string is wrong.

**Fix:**
1. Open **Services** (press `Win + R`, type `services.msc`)
2. Find **SQL Server (SQLEXPRESS)** — make sure it is **Running**
3. If stopped, right-click → **Start**
4. If still failing, verify the connection string in `appsettings.json`

---

### ❌ Application won't open / crashes immediately

**Cause:** .NET 8 Runtime is not installed.

**Fix:**
1. Download .NET 8 Desktop Runtime from https://dotnet.microsoft.com/download/dotnet/8
2. Install and restart the application

---

### ❌ Invoice PDF doesn't open

**Cause:** No default PDF viewer is set on the computer.

**Fix:**
1. Install a free PDF viewer such as **Adobe Acrobat Reader** or **Foxit Reader**
2. Set it as the default program for `.pdf` files

---

### ❌ Login fails with correct credentials

**Cause:** Possible database sync issue or corrupted password.

**Fix (Admin only):**
1. Open **SQL Server Management Studio (SSMS)**
2. Run the following query to reset the admin password:
```sql
USE JewelryBillingSoftDb;
UPDATE Users 
SET PasswordHash = 'gGcG1J7LFqFiZABa0e2/JwKVh/NxqaXY7cjA1Lne8Bk=' 
WHERE Username = 'admin';
```
3. This resets the password back to `Admin@123`

---

### ❌ Printer not working / Invoice not printing

**Fix:**
1. Ensure the printer is connected and set as the **default printer**
2. Try opening a saved PDF from the temp folder manually and printing
3. Check Windows printer settings: **Control Panel → Devices and Printers**

---

### ❌ Software is slow or freezing

**Fix:**
1. Ensure your PC meets the minimum requirements
2. Check available RAM (close other programs)
3. Run **SQL Server Maintenance** — shrink the database if it's grown very large
4. Restart the application

---

## 16. FAQs

**Q: Can I use this software on multiple computers?**  
A: The license is tied to one machine. Contact your vendor for multi-machine licensing.

**Q: Can two staff members use it at the same time?**  
A: If installed on a server and accessed via network share, multiple users can connect. Contact support for network setup guidance.

**Q: How do I take a backup of my data?**  
A: Use **SQL Server Management Studio (SSMS)** to back up the `JewelryBillingSoftDb` database. Set up scheduled backups in SSMS for automatic daily backups.

**Q: Can I import existing customer/product data?**  
A: Direct import from Excel is not supported in v1.0. Contact support for assisted data migration.

**Q: What GST rate applies to jewelry?**  
A: As per Indian GST rules, the standard rate for jewelry is **3%** (CGST 1.5% + SGST 1.5%). Verify with your CA for the latest rates.

**Q: Can I customize the invoice format / add my logo?**  
A: In v1.0, the invoice template uses your shop name from Settings. Logo support will be added in v1.1.

**Q: How do I handle returns?**  
A: Open the original invoice, use the **"Process Return"** option to record the refund amount. The system will update the payment records accordingly.

**Q: Is internet required for daily use?**  
A: No. The software works 100% offline. Internet is only needed for the initial license activation.

**Q: How do I add new staff/users?**  
A: Admin can add new users via **Settings → User Management** (available in v1.1). Currently, contact your system admin to add users via database.

---

## 17. Support

### Contact Information

| Channel | Details |
|---------|---------|
| 📧 Email | support@jewelrysoft.com |
| 📞 Phone | +91-XXXXXXXXXX (Mon–Sat, 10 AM–6 PM IST) |
| 🌐 Website | www.jewelrysoft.com |

### Before Contacting Support, Please Have Ready:
1. Your **License Key**
2. Your **Windows version** (Win+R → `winver`)
3. A **screenshot** of the error message (if any)
4. The log file located at: `%APPDATA%\JewelryBillingSoft\logs\`

### Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | July 2024 | Initial release — Billing, Inventory, Customers, Reports, Licensing |

---

*© 2024 JewelrySoft. All rights reserved. This manual is for registered users only.*

