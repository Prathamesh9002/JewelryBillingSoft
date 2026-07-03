namespace JewelryBillingSoft.Domain.Enums;

public enum MetalType
{
    Gold,
    Silver,
    Diamond,
    Platinum,
    Other
}

public enum PaymentMethod
{
    Cash,
    Card,
    UPI,
    Mixed,
    Pending
}

public enum UserRole
{
    Admin,
    Manager,
    Cashier
}

public enum InvoiceStatus
{
    Draft,
    Completed,
    Cancelled,
    PartiallyPaid
}

public enum StockStatus
{
    Available,
    Sold,
    Reserved,
    Returned
}

public enum PurityType
{
    Gold24K,
    Gold22K,
    Gold18K,
    Gold14K,
    Silver999,
    Silver925,
    Diamond,
    Other
}

