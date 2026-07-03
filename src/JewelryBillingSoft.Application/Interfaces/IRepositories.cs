using JewelryBillingSoft.Domain.Entities;
using JewelryBillingSoft.Domain.Enums;

namespace JewelryBillingSoft.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByMobileAsync(string mobile);
    Task<Customer?> GetByCodeAsync(string code);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
    Task<IEnumerable<Invoice>> GetCustomerInvoicesAsync(int customerId);
}

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByItemCodeAsync(string itemCode);
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<IEnumerable<Product>> GetByMetalTypeAsync(MetalType metalType);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5);
    Task UpdateStockAsync(int productId, int quantity, StockStatus status);
}

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId);
    Task<IEnumerable<Invoice>> GetPendingInvoicesAsync();
    Task<string> GenerateInvoiceNumberAsync();
    Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ValidatePasswordAsync(string username, string password);
    Task ChangePasswordAsync(int userId, string newPasswordHash);
}

public interface ILicenseRepository : IRepository<LicenseInfo>
{
    Task<LicenseInfo?> GetActiveLicenseAsync();
    Task<bool> IsLicenseValidAsync();
}

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    IProductRepository Products { get; }
    IInvoiceRepository Invoices { get; }
    IUserRepository Users { get; }
    ILicenseRepository Licenses { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

