using JewelryBillingSoft.Application.DTOs;
using JewelryBillingSoft.Domain.Enums;

namespace JewelryBillingSoft.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<IEnumerable<CustomerDto>> SearchAsync(string searchTerm);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<InvoiceSummaryDto>> GetPurchaseHistoryAsync(int customerId);
}

public interface IInventoryService
{
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<ProductDto>> GetByMetalTypeAsync(MetalType metalType);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 5);
    Task<StockSummaryDto> GetStockSummaryAsync();
}

public interface IBillingService
{
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task<InvoiceDto?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<IEnumerable<InvoiceSummaryDto>> GetAllAsync();
    Task<IEnumerable<InvoiceSummaryDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto);
    Task<InvoiceDto> FinalizeInvoiceAsync(int id, List<PaymentDto> payments);
    Task<InvoiceDto> AddPaymentAsync(int invoiceId, PaymentDto payment);
    Task CancelInvoiceAsync(int id, string reason);
    Task<InvoiceDto> ProcessReturnAsync(int id, decimal refundAmount);
    Task<byte[]> GenerateInvoicePdfAsync(int id);
    Task<decimal> CalculateItemTotalAsync(InvoiceItemDto item);
}

public interface IReportService
{
    Task<DailySalesReportDto> GetDailySalesReportAsync(DateTime date);
    Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(int year, int month);
    Task<StockReportDto> GetStockReportAsync();
    Task<CustomerPurchaseReportDto> GetCustomerPurchaseReportAsync(int? customerId, DateTime? from, DateTime? to);
    Task<TaxSummaryReportDto> GetTaxSummaryReportAsync(DateTime from, DateTime to);
    Task<ProfitLossReportDto> GetProfitLossReportAsync(DateTime from, DateTime to);
    Task<IEnumerable<PendingPaymentDto>> GetPendingPaymentsReportAsync();
    Task<byte[]> ExportReportToPdfAsync(string reportType, object parameters);
}

public interface ILicenseService
{
    Task<LicenseStatusDto> GetLicenseStatusAsync();
    Task<bool> IsLicenseValidAsync();
    Task<LicenseActivationResultDto> ActivateLicenseAsync(string licenseKey, string shopName, string email);
    Task<LicenseActivationResultDto> RenewLicenseAsync(string newLicenseKey);
    Task<int> GetRemainingDaysAsync();
    string GenerateMachineId();
}

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<UserDto> GetCurrentUserAsync();
    bool IsAuthenticated { get; }
    UserDto? CurrentUser { get; }
}

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string? entityId = null,
        object? oldValues = null, object? newValues = null);
    Task<IEnumerable<AuditLogDto>> GetLogsAsync(DateTime? from = null, DateTime? to = null, string? action = null);
}

