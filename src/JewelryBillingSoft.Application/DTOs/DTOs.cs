using JewelryBillingSoft.Domain.Enums;

namespace JewelryBillingSoft.Application.DTOs;

// Customer DTOs
public class CustomerDto
{
    public int Id { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? AlternateMobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal LoyaltyPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalPurchaseAmount { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? AlternateMobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? PanNumber { get; set; }
    public string? AadharNumber { get; set; }
}

public class UpdateCustomerDto : CreateCustomerDto
{
    public int Id { get; set; }
}

// Product DTOs
public class ProductDto
{
    public int Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public MetalType MetalType { get; set; }
    public string MetalTypeName => MetalType.ToString();
    public PurityType Purity { get; set; }
    public string PurityName => Purity.ToString();
    public decimal GrossWeight { get; set; }
    public decimal StoneWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal MakingCharges { get; set; }
    public decimal WastagePercentage { get; set; }
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; }
    public string? BarcodeNumber { get; set; }
    public string? HSNCode { get; set; }
    public decimal GSTPercentage { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public MetalType MetalType { get; set; }
    public PurityType Purity { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal StoneWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal MakingCharges { get; set; }
    public decimal WastagePercentage { get; set; }
    public int StockQuantity { get; set; }
    public string? BarcodeNumber { get; set; }
    public string? HSNCode { get; set; }
    public decimal GSTPercentage { get; set; } = 3;
}

public class UpdateProductDto : CreateProductDto
{
    public int Id { get; set; }
}

// Invoice DTOs
public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalGST { get; set; }
    public decimal TotalMakingCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AdvanceReceived { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Notes { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class InvoiceSummaryDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string StatusName => Status.ToString();
}

public class CreateInvoiceDto
{
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public decimal AdvanceReceived { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class UpdateInvoiceDto : CreateInvoiceDto
{
    public int Id { get; set; }
}

public class InvoiceItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal GrossWeight { get; set; }
    public decimal StoneWeight { get; set; }
    public decimal NetWeight { get; set; }
    public string Purity { get; set; } = string.Empty;
    public decimal RatePerGram { get; set; }
    public decimal MakingCharges { get; set; }
    public decimal WastagePercentage { get; set; }
    public decimal WastageAmount { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GSTPercentage { get; set; }
    public decimal GSTAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? HSNCode { get; set; }
}

public class CreateInvoiceItemDto
{
    public int ProductId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal GrossWeight { get; set; }
    public decimal StoneWeight { get; set; }
    public decimal NetWeight { get; set; }
    public string Purity { get; set; } = string.Empty;
    public decimal RatePerGram { get; set; }
    public decimal MakingCharges { get; set; }
    public decimal WastagePercentage { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal GSTPercentage { get; set; } = 3;
    public string? HSNCode { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName => PaymentMethod.ToString();
    public decimal Amount { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }
    public bool IsRefund { get; set; }
}

// Report DTOs
public class DailySalesReportDto
{
    public DateTime Date { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalSales { get; set; }
    public decimal CashSales { get; set; }
    public decimal CardSales { get; set; }
    public decimal UPISales { get; set; }
    public decimal PendingAmount { get; set; }
    public int TotalItemsSold { get; set; }
    public List<InvoiceSummaryDto> Invoices { get; set; } = new();
}

public class MonthlySalesReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public decimal TotalSales { get; set; }
    public int TotalInvoices { get; set; }
    public List<DailySalesReportDto> DailyBreakdown { get; set; } = new();
}

public class StockReportDto
{
    public int TotalProducts { get; set; }
    public int AvailableProducts { get; set; }
    public int SoldProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<ProductDto> LowStockItems { get; set; } = new();
    public List<ProductDto> Products { get; set; } = new();
}

public class StockSummaryDto
{
    public int TotalItems { get; set; }
    public int GoldItems { get; set; }
    public int SilverItems { get; set; }
    public int DiamondItems { get; set; }
    public decimal TotalValue { get; set; }
}

public class CustomerPurchaseReportDto
{
    public CustomerDto? Customer { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public List<InvoiceSummaryDto> Invoices { get; set; } = new();
}

public class TaxSummaryReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTaxableAmount { get; set; }
    public decimal TotalCGST { get; set; }
    public decimal TotalSGST { get; set; }
    public decimal TotalGST { get; set; }
    public List<TaxLineItemDto> LineItems { get; set; } = new();
}

public class TaxLineItemDto
{
    public string HSNCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal CGSTAmount { get; set; }
    public decimal SGSTAmount { get; set; }
    public decimal TotalTax { get; set; }
}

public class ProfitLossReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCostOfGoods { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal GrossProfitPercentage { get; set; }
    public decimal MakingChargesRevenue { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal NetProfit { get; set; }
}

public class PendingPaymentDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerMobile { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public int DaysOverdue { get; set; }
}

// License DTOs
public class LicenseStatusDto
{
    public bool IsValid { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string LicenseHolder { get; set; } = string.Empty;
    public DateTime ActivationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public bool IsExpiringSoon => RemainingDays <= 30;
    public string LicenseType { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
}

public class LicenseActivationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public LicenseStatusDto? LicenseStatus { get; set; }
}

// Auth DTOs
public class AuthResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string? Token { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string RoleName => Role.ToString();
    public bool IsActive { get; set; }
}

// Audit DTOs
public class AuditLogDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}

using JewelryBillingSoft.Domain.Enums;

