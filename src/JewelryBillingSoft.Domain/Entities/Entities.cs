using JewelryBillingSoft.Domain.Enums;

namespace JewelryBillingSoft.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

public class Customer : BaseEntity
{
    public string CustomerCode { get; set; } = string.Empty;
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
    public decimal LoyaltyPoints { get; set; } = 0;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MetalType MetalType { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product : BaseEntity
{
    public string ItemCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
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
    public StockStatus StockStatus { get; set; } = StockStatus.Available;
    public string? BarcodeNumber { get; set; }
    public string? HSNCode { get; set; }
    public decimal GSTPercentage { get; set; } = 3;
    public string? ImagePath { get; set; }
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalGST { get; set; }
    public decimal TotalMakingCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AdvanceReceived { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
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
    public decimal UnitPrice { get; set; }
    public string? HSNCode { get; set; }
}

public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
    public bool IsRefund { get; set; } = false;
}

public class LicenseInfo : BaseEntity
{
    public string LicenseKey { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string LicenseHolder { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime ActivationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? MachineId { get; set; }
    public int MaxUsers { get; set; } = 3;
    public string LicenseType { get; set; } = "Annual";
    public string EncryptedData { get; set; } = string.Empty;
}

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IPAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class ShopSettings : BaseEntity
{
    public string ShopName { get; set; } = string.Empty;
    public string? ShopAddress { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? GSTNumber { get; set; }
    public string? PanNumber { get; set; }
    public string? LogoPath { get; set; }
    public string? InvoicePrefix { get; set; } = "INV";
    public int InvoiceCounter { get; set; } = 1;
    public string CurrencySymbol { get; set; } = "₹";
    public decimal DefaultGSTRate { get; set; } = 3;
    public string? BankDetails { get; set; }
    public string? TermsAndConditions { get; set; }
}

