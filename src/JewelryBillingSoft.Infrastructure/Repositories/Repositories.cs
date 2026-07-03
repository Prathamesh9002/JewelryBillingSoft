using JewelryBillingSoft.Application.Interfaces;
using JewelryBillingSoft.Domain.Entities;
using JewelryBillingSoft.Domain.Enums;
using JewelryBillingSoft.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace JewelryBillingSoft.Infrastructure.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    public virtual async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.Where(x => !x.IsDeleted).ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.Now;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
        return entity;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
        }
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _dbSet.AnyAsync(x => x.Id == id && !x.IsDeleted);
}

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _dbSet.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToListAsync();

    public async Task<Customer?> GetByMobileAsync(string mobile) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Mobile == mobile && !x.IsDeleted);

    public async Task<Customer?> GetByCodeAsync(string code) =>
        await _dbSet.FirstOrDefaultAsync(x => x.CustomerCode == code && !x.IsDeleted);

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm) =>
        await _dbSet.Where(x => !x.IsDeleted &&
            (x.Name.Contains(searchTerm) || x.Mobile.Contains(searchTerm) ||
             x.CustomerCode.Contains(searchTerm) || (x.Email != null && x.Email.Contains(searchTerm))))
            .ToListAsync();

    public async Task<IEnumerable<Invoice>> GetCustomerInvoicesAsync(int customerId) =>
        await _context.Invoices
            .Include(i => i.InvoiceItems)
            .Include(i => i.Payments)
            .Where(i => i.CustomerId == customerId && !i.IsDeleted)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
}

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Product>> GetAllAsync() =>
        await _dbSet.Include(p => p.Category)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();

    public override async Task<Product?> GetByIdAsync(int id) =>
        await _dbSet.Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    public async Task<Product?> GetByItemCodeAsync(string itemCode) =>
        await _dbSet.Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.ItemCode == itemCode && !x.IsDeleted);

    public async Task<Product?> GetByBarcodeAsync(string barcode) =>
        await _dbSet.FirstOrDefaultAsync(x => x.BarcodeNumber == barcode && !x.IsDeleted);

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm) =>
        await _dbSet.Include(p => p.Category)
            .Where(x => !x.IsDeleted &&
                (x.Name.Contains(searchTerm) || x.ItemCode.Contains(searchTerm) ||
                 (x.BarcodeNumber != null && x.BarcodeNumber.Contains(searchTerm))))
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetByMetalTypeAsync(MetalType metalType) =>
        await _dbSet.Include(p => p.Category)
            .Where(x => x.MetalType == metalType && !x.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5) =>
        await _dbSet.Where(x => x.StockQuantity <= threshold && !x.IsDeleted).ToListAsync();

    public async Task UpdateStockAsync(int productId, int quantity, StockStatus status)
    {
        var product = await GetByIdAsync(productId);
        if (product != null)
        {
            product.StockQuantity += quantity;
            product.StockStatus = product.StockQuantity <= 0 ? StockStatus.Sold : status;
            product.UpdatedAt = DateTime.Now;
        }
    }
}

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext context) : base(context) { }

    public override async Task<Invoice?> GetByIdAsync(int id) =>
        await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Include(i => i.InvoiceItems).ThenInclude(ii => ii.Product)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    public override async Task<IEnumerable<Invoice>> GetAllAsync() =>
        await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Payments)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync();

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber) =>
        await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.InvoiceItems).ThenInclude(ii => ii.Product)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber && !x.IsDeleted);

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to) =>
        await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.InvoiceItems)
            .Include(i => i.Payments)
            .Where(x => !x.IsDeleted && x.InvoiceDate >= from && x.InvoiceDate <= to)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync();

    public async Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId) =>
        await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Payments)
            .Where(x => !x.IsDeleted && x.CustomerId == customerId)
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync();

    public async Task<IEnumerable<Invoice>> GetPendingInvoicesAsync() =>
        await _dbSet
            .Include(i => i.Customer)
            .Where(x => !x.IsDeleted && x.PendingAmount > 0 && x.Status != InvoiceStatus.Cancelled)
            .OrderBy(x => x.InvoiceDate)
            .ToListAsync();

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var settings = await _context.ShopSettings.FirstOrDefaultAsync();
        var prefix = settings?.InvoicePrefix ?? "INV";
        var counter = settings?.InvoiceCounter ?? 1;
        var invoiceNumber = $"{prefix}{counter:D6}";

        if (settings != null)
        {
            settings.InvoiceCounter++;
            _context.Entry(settings).State = EntityState.Modified;
        }

        return invoiceNumber;
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(x => !x.IsDeleted && x.InvoiceDate >= from && x.InvoiceDate <= to
                        && x.Status != InvoiceStatus.Cancelled)
            .SumAsync(x => x.TotalAmount);
    }
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Username == username && !x.IsDeleted && x.IsActive);

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null) return false;
        var hash = HashPassword(password);
        return user.PasswordHash == hash;
    }

    public async Task ChangePasswordAsync(int userId, string newPasswordHash)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.Now;
        }
        await Task.CompletedTask;
    }

    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "JewelrySalt2024"));
        return Convert.ToBase64String(bytes);
    }
}

public class LicenseRepository : BaseRepository<LicenseInfo>, ILicenseRepository
{
    public LicenseRepository(AppDbContext context) : base(context) { }

    public async Task<LicenseInfo?> GetActiveLicenseAsync() =>
        await _dbSet.Where(x => x.IsActive).OrderByDescending(x => x.ExpiryDate).FirstOrDefaultAsync();

    public async Task<bool> IsLicenseValidAsync()
    {
        var license = await GetActiveLicenseAsync();
        return license != null && license.IsActive && license.ExpiryDate >= DateTime.Today;
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public ICustomerRepository Customers { get; }
    public IProductRepository Products { get; }
    public IInvoiceRepository Invoices { get; }
    public IUserRepository Users { get; }
    public ILicenseRepository Licenses { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Customers = new CustomerRepository(context);
        Products = new ProductRepository(context);
        Invoices = new InvoiceRepository(context);
        Users = new UserRepository(context);
        Licenses = new LicenseRepository(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync() =>
        _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

