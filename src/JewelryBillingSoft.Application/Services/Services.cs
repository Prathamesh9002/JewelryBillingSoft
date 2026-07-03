using JewelryBillingSoft.Application.DTOs;
using JewelryBillingSoft.Application.Interfaces;
using JewelryBillingSoft.Domain.Entities;
using JewelryBillingSoft.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JewelryBillingSoft.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<IEnumerable<CustomerDto>> SearchAsync(string searchTerm)
    {
        var customers = await _unitOfWork.Customers.SearchAsync(searchTerm);
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        // Generate customer code
        var allCustomers = await _unitOfWork.Customers.GetAllAsync();
        var code = $"CUST{(allCustomers.Count() + 1):D4}";

        var customer = new Customer
        {
            CustomerCode = code,
            Name = dto.Name,
            Mobile = dto.Mobile,
            AlternateMobile = dto.AlternateMobile,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            PinCode = dto.PinCode,
            PanNumber = dto.PanNumber,
            AadharNumber = dto.AadharNumber
        };

        var created = await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Customer created: {Name} ({Code})", created.Name, created.CustomerCode);
        return MapToDto(created);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer with id {id} not found");

        customer.Name = dto.Name;
        customer.Mobile = dto.Mobile;
        customer.AlternateMobile = dto.AlternateMobile;
        customer.Email = dto.Email;
        customer.Address = dto.Address;
        customer.City = dto.City;
        customer.State = dto.State;
        customer.PinCode = dto.PinCode;
        customer.PanNumber = dto.PanNumber;
        customer.AadharNumber = dto.AadharNumber;
        customer.UpdatedAt = DateTime.Now;

        var updated = await _unitOfWork.Customers.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Customers.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<InvoiceSummaryDto>> GetPurchaseHistoryAsync(int customerId)
    {
        var invoices = await _unitOfWork.Invoices.GetByCustomerAsync(customerId);
        return invoices.Select(i => new InvoiceSummaryDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            CustomerName = i.Customer?.Name ?? string.Empty,
            CustomerMobile = i.Customer?.Mobile ?? string.Empty,
            TotalAmount = i.TotalAmount,
            PaidAmount = i.PaidAmount,
            PendingAmount = i.PendingAmount,
            Status = i.Status
        });
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        CustomerCode = c.CustomerCode,
        Name = c.Name,
        Mobile = c.Mobile,
        AlternateMobile = c.AlternateMobile,
        Email = c.Email,
        Address = c.Address,
        City = c.City,
        State = c.State,
        LoyaltyPoints = c.LoyaltyPoints,
        CreatedAt = c.CreatedAt
    };
}

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IUnitOfWork unitOfWork, ILogger<InventoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByMetalTypeAsync(MetalType metalType)
    {
        var products = await _unitOfWork.Products.GetByMetalTypeAsync(metalType);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var code = $"{dto.MetalType.ToString()[0]}{(allProducts.Count() + 1):D4}";

        var product = new Product
        {
            ItemCode = code,
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            MetalType = dto.MetalType,
            Purity = dto.Purity,
            GrossWeight = dto.GrossWeight,
            StoneWeight = dto.StoneWeight,
            NetWeight = dto.NetWeight,
            PurchasePrice = dto.PurchasePrice,
            SellingPrice = dto.SellingPrice,
            MakingCharges = dto.MakingCharges,
            WastagePercentage = dto.WastagePercentage,
            StockQuantity = dto.StockQuantity,
            BarcodeNumber = dto.BarcodeNumber,
            HSNCode = dto.HSNCode,
            GSTPercentage = dto.GSTPercentage
        };

        var created = await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id {id} not found");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.MetalType = dto.MetalType;
        product.Purity = dto.Purity;
        product.GrossWeight = dto.GrossWeight;
        product.StoneWeight = dto.StoneWeight;
        product.NetWeight = dto.NetWeight;
        product.PurchasePrice = dto.PurchasePrice;
        product.SellingPrice = dto.SellingPrice;
        product.MakingCharges = dto.MakingCharges;
        product.WastagePercentage = dto.WastagePercentage;
        product.StockQuantity = dto.StockQuantity;
        product.BarcodeNumber = dto.BarcodeNumber;
        product.HSNCode = dto.HSNCode;
        product.GSTPercentage = dto.GSTPercentage;
        product.UpdatedAt = DateTime.Now;

        var updated = await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Products.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 5)
    {
        var products = await _unitOfWork.Products.GetLowStockProductsAsync(threshold);
        return products.Select(MapToDto);
    }

    public async Task<StockSummaryDto> GetStockSummaryAsync()
    {
        var products = (await _unitOfWork.Products.GetAllAsync()).ToList();
        return new StockSummaryDto
        {
            TotalItems = products.Count,
            GoldItems = products.Count(p => p.MetalType == MetalType.Gold),
            SilverItems = products.Count(p => p.MetalType == MetalType.Silver),
            DiamondItems = products.Count(p => p.MetalType == MetalType.Diamond),
            TotalValue = products.Sum(p => p.SellingPrice * p.StockQuantity)
        };
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        ItemCode = p.ItemCode,
        Name = p.Name,
        Description = p.Description,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        MetalType = p.MetalType,
        Purity = p.Purity,
        GrossWeight = p.GrossWeight,
        StoneWeight = p.StoneWeight,
        NetWeight = p.NetWeight,
        PurchasePrice = p.PurchasePrice,
        SellingPrice = p.SellingPrice,
        MakingCharges = p.MakingCharges,
        WastagePercentage = p.WastagePercentage,
        StockQuantity = p.StockQuantity,
        StockStatus = p.StockStatus,
        BarcodeNumber = p.BarcodeNumber,
        HSNCode = p.HSNCode,
        GSTPercentage = p.GSTPercentage
    };
}

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BillingService> _logger;

    public BillingService(IUnitOfWork unitOfWork, ILogger<BillingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<InvoiceDto?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        var invoice = await _unitOfWork.Invoices.GetByInvoiceNumberAsync(invoiceNumber);
        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<IEnumerable<InvoiceSummaryDto>> GetAllAsync()
    {
        var invoices = await _unitOfWork.Invoices.GetAllAsync();
        return invoices.Select(MapToSummaryDto);
    }

    public async Task<IEnumerable<InvoiceSummaryDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var invoices = await _unitOfWork.Invoices.GetByDateRangeAsync(from, to);
        return invoices.Select(MapToSummaryDto);
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
    {
        var invoiceNumber = await _unitOfWork.Invoices.GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = dto.InvoiceDate,
            CustomerId = dto.CustomerId,
            AdvanceReceived = dto.AdvanceReceived,
            Notes = dto.Notes,
            Status = InvoiceStatus.Draft
        };

        foreach (var itemDto in dto.Items)
        {
            var item = await CreateInvoiceItemFromDto(itemDto);
            invoice.InvoiceItems.Add(item);
        }

        CalculateInvoiceTotals(invoice);

        var created = await _unitOfWork.Invoices.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(created);
    }

    public async Task<InvoiceDto> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice {id} not found");

        if (invoice.Status == InvoiceStatus.Completed)
            throw new InvalidOperationException("Cannot update a completed invoice");

        invoice.InvoiceDate = dto.InvoiceDate;
        invoice.CustomerId = dto.CustomerId;
        invoice.AdvanceReceived = dto.AdvanceReceived;
        invoice.Notes = dto.Notes;
        invoice.InvoiceItems.Clear();

        foreach (var itemDto in dto.Items)
        {
            var item = await CreateInvoiceItemFromDto(itemDto);
            invoice.InvoiceItems.Add(item);
        }

        CalculateInvoiceTotals(invoice);
        invoice.UpdatedAt = DateTime.Now;

        var updated = await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task<InvoiceDto> FinalizeInvoiceAsync(int id, List<PaymentDto> payments)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice {id} not found");

        var totalPaid = payments.Sum(p => p.Amount) + invoice.AdvanceReceived;

        foreach (var paymentDto in payments)
        {
            invoice.Payments.Add(new Payment
            {
                PaymentMethod = paymentDto.PaymentMethod,
                Amount = paymentDto.Amount,
                TransactionReference = paymentDto.TransactionReference,
                PaymentDate = DateTime.Now,
                Notes = paymentDto.Notes
            });
        }

        invoice.PaidAmount = totalPaid;
        invoice.PendingAmount = Math.Max(0, invoice.TotalAmount - totalPaid);
        invoice.Status = invoice.PendingAmount == 0 ? InvoiceStatus.Completed : InvoiceStatus.PartiallyPaid;
        invoice.UpdatedAt = DateTime.Now;

        // Update stock
        foreach (var item in invoice.InvoiceItems)
        {
            await _unitOfWork.Products.UpdateStockAsync(item.ProductId, -item.Quantity, StockStatus.Sold);
        }

        var updated = await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task<InvoiceDto> AddPaymentAsync(int invoiceId, PaymentDto paymentDto)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId)
            ?? throw new KeyNotFoundException($"Invoice {invoiceId} not found");

        invoice.Payments.Add(new Payment
        {
            PaymentMethod = paymentDto.PaymentMethod,
            Amount = paymentDto.Amount,
            TransactionReference = paymentDto.TransactionReference,
            PaymentDate = DateTime.Now,
            Notes = paymentDto.Notes
        });

        invoice.PaidAmount += paymentDto.Amount;
        invoice.PendingAmount = Math.Max(0, invoice.TotalAmount - invoice.PaidAmount);

        if (invoice.PendingAmount == 0)
            invoice.Status = InvoiceStatus.Completed;

        var updated = await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task CancelInvoiceAsync(int id, string reason)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice {id} not found");

        invoice.Status = InvoiceStatus.Cancelled;
        invoice.Notes = $"Cancelled: {reason}";
        invoice.UpdatedAt = DateTime.Now;

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<InvoiceDto> ProcessReturnAsync(int id, decimal refundAmount)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice {id} not found");

        invoice.RefundAmount += refundAmount;
        invoice.Payments.Add(new Payment
        {
            PaymentMethod = PaymentMethod.Cash,
            Amount = refundAmount,
            PaymentDate = DateTime.Now,
            Notes = "Refund",
            IsRefund = true
        });

        var updated = await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(updated);
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(int id)
    {
        // PDF generation is handled by InvoicePrinter in Infrastructure
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    public Task<decimal> CalculateItemTotalAsync(InvoiceItemDto item)
    {
        var metalValue = item.NetWeight * item.RatePerGram;
        var wastageAmount = metalValue * (item.WastagePercentage / 100);
        var subTotal = (metalValue + wastageAmount + item.MakingCharges) * item.Quantity;
        var discountAmount = subTotal * (item.DiscountPercentage / 100);
        var taxableAmount = subTotal - discountAmount;
        var gstAmount = taxableAmount * (item.GSTPercentage / 100);
        var total = taxableAmount + gstAmount;
        return Task.FromResult(total);
    }

    private static async Task<InvoiceItem> CreateInvoiceItemFromDto(CreateInvoiceItemDto dto)
    {
        var metalValue = dto.NetWeight * dto.RatePerGram;
        var wastageAmount = metalValue * (dto.WastagePercentage / 100);
        var subTotal = (metalValue + wastageAmount + dto.MakingCharges) * dto.Quantity;
        var discountAmount = subTotal * (dto.DiscountPercentage / 100);
        var taxableAmount = subTotal - discountAmount;
        var gstAmount = taxableAmount * (dto.GSTPercentage / 100);

        await Task.CompletedTask;
        return new InvoiceItem
        {
            ProductId = dto.ProductId,
            ItemDescription = dto.ItemDescription,
            Quantity = dto.Quantity,
            GrossWeight = dto.GrossWeight,
            StoneWeight = dto.StoneWeight,
            NetWeight = dto.NetWeight,
            Purity = dto.Purity,
            RatePerGram = dto.RatePerGram,
            MakingCharges = dto.MakingCharges,
            WastagePercentage = dto.WastagePercentage,
            WastageAmount = wastageAmount,
            SubTotal = subTotal,
            DiscountPercentage = dto.DiscountPercentage,
            DiscountAmount = discountAmount,
            GSTPercentage = dto.GSTPercentage,
            GSTAmount = gstAmount,
            TotalAmount = taxableAmount + gstAmount,
            HSNCode = dto.HSNCode
        };
    }

    private static void CalculateInvoiceTotals(Invoice invoice)
    {
        invoice.SubTotal = invoice.InvoiceItems.Sum(i => i.SubTotal);
        invoice.TotalDiscount = invoice.InvoiceItems.Sum(i => i.DiscountAmount);
        invoice.TotalGST = invoice.InvoiceItems.Sum(i => i.GSTAmount);
        invoice.TotalMakingCharges = invoice.InvoiceItems.Sum(i => i.MakingCharges * i.Quantity);
        invoice.TotalAmount = invoice.InvoiceItems.Sum(i => i.TotalAmount);
        invoice.PendingAmount = Math.Max(0, invoice.TotalAmount - invoice.AdvanceReceived - invoice.PaidAmount);
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        CustomerId = i.CustomerId,
        CustomerName = i.Customer?.Name ?? string.Empty,
        CustomerMobile = i.Customer?.Mobile ?? string.Empty,
        CreatedByUserId = i.CreatedByUserId,
        CreatedByUserName = i.CreatedByUser?.FullName ?? string.Empty,
        SubTotal = i.SubTotal,
        TotalDiscount = i.TotalDiscount,
        TotalGST = i.TotalGST,
        TotalMakingCharges = i.TotalMakingCharges,
        TotalAmount = i.TotalAmount,
        AdvanceReceived = i.AdvanceReceived,
        PaidAmount = i.PaidAmount,
        PendingAmount = i.PendingAmount,
        RefundAmount = i.RefundAmount,
        Status = i.Status,
        Notes = i.Notes,
        Items = i.InvoiceItems.Select(item => new InvoiceItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ItemDescription = item.ItemDescription,
            Quantity = item.Quantity,
            GrossWeight = item.GrossWeight,
            StoneWeight = item.StoneWeight,
            NetWeight = item.NetWeight,
            Purity = item.Purity,
            RatePerGram = item.RatePerGram,
            MakingCharges = item.MakingCharges,
            WastagePercentage = item.WastagePercentage,
            WastageAmount = item.WastageAmount,
            SubTotal = item.SubTotal,
            DiscountPercentage = item.DiscountPercentage,
            DiscountAmount = item.DiscountAmount,
            GSTPercentage = item.GSTPercentage,
            GSTAmount = item.GSTAmount,
            TotalAmount = item.TotalAmount,
            HSNCode = item.HSNCode
        }).ToList(),
        Payments = i.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount,
            TransactionReference = p.TransactionReference,
            PaymentDate = p.PaymentDate,
            Notes = p.Notes,
            IsRefund = p.IsRefund
        }).ToList()
    };

    private static InvoiceSummaryDto MapToSummaryDto(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        CustomerName = i.Customer?.Name ?? string.Empty,
        CustomerMobile = i.Customer?.Mobile ?? string.Empty,
        TotalAmount = i.TotalAmount,
        PaidAmount = i.PaidAmount,
        PendingAmount = i.PendingAmount,
        Status = i.Status
    };
}

public class LicenseService : ILicenseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LicenseService> _logger;

    public LicenseService(IUnitOfWork unitOfWork, ILogger<LicenseService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<LicenseStatusDto> GetLicenseStatusAsync()
    {
        var license = await _unitOfWork.Licenses.GetActiveLicenseAsync();
        if (license == null)
            return new LicenseStatusDto { IsValid = false };

        return new LicenseStatusDto
        {
            IsValid = license.IsActive && license.ExpiryDate >= DateTime.Today,
            ShopName = license.ShopName,
            LicenseHolder = license.LicenseHolder,
            ActivationDate = license.ActivationDate,
            ExpiryDate = license.ExpiryDate,
            RemainingDays = Math.Max(0, (license.ExpiryDate - DateTime.Today).Days),
            LicenseType = license.LicenseType,
            MaxUsers = license.MaxUsers
        };
    }

    public async Task<bool> IsLicenseValidAsync()
    {
        var status = await GetLicenseStatusAsync();
        return status.IsValid;
    }

    public async Task<LicenseActivationResultDto> ActivateLicenseAsync(string licenseKey, string shopName, string email)
    {
        try
        {
            // Validate license key format (simple check)
            if (string.IsNullOrEmpty(licenseKey) || licenseKey.Length < 16)
                return new LicenseActivationResultDto { Success = false, Message = "Invalid license key format" };

            var license = new LicenseInfo
            {
                LicenseKey = licenseKey,
                ShopName = shopName,
                Email = email,
                LicenseHolder = shopName,
                ActivationDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddYears(1),
                IsActive = true,
                MachineId = GenerateMachineId(),
                LicenseType = "Annual",
                MaxUsers = 5,
                EncryptedData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{licenseKey}|{DateTime.Today:yyyyMMdd}|{shopName}"))
            };

            await _unitOfWork.Licenses.AddAsync(license);
            await _unitOfWork.SaveChangesAsync();

            var status = await GetLicenseStatusAsync();
            return new LicenseActivationResultDto { Success = true, Message = "License activated successfully", LicenseStatus = status };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "License activation failed");
            return new LicenseActivationResultDto { Success = false, Message = ex.Message };
        }
    }

    public async Task<LicenseActivationResultDto> RenewLicenseAsync(string newLicenseKey)
    {
        var existing = await _unitOfWork.Licenses.GetActiveLicenseAsync();
        if (existing == null)
            return new LicenseActivationResultDto { Success = false, Message = "No existing license found" };

        return await ActivateLicenseAsync(newLicenseKey, existing.ShopName, existing.Email);
    }

    public async Task<int> GetRemainingDaysAsync()
    {
        var status = await GetLicenseStatusAsync();
        return status.RemainingDays;
    }

    public string GenerateMachineId()
    {
        var machineName = Environment.MachineName;
        var userName = Environment.UserName;
        var combined = $"{machineName}-{userName}";
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combined));
        return Convert.ToBase64String(bytes)[..16];
    }
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private UserDto? _currentUser;

    public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public bool IsAuthenticated => _currentUser != null;
    public UserDto? CurrentUser => _currentUser;

    public async Task<AuthResultDto> LoginAsync(string username, string password)
    {
        try
        {
            var isValid = await _unitOfWork.Users.ValidatePasswordAsync(username, password);
            if (!isValid)
                return new AuthResultDto { Success = false, Message = "Invalid username or password" };

            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null || !user.IsActive)
                return new AuthResultDto { Success = false, Message = "Account is inactive" };

            _currentUser = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Mobile = user.Mobile,
                Role = user.Role,
                IsActive = user.IsActive
            };

            _logger.LogInformation("User logged in: {Username}", username);
            return new AuthResultDto { Success = true, Message = "Login successful", User = _currentUser };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for {Username}", username);
            return new AuthResultDto { Success = false, Message = "Login failed" };
        }
    }

    public Task LogoutAsync()
    {
        _logger.LogInformation("User logged out: {Username}", _currentUser?.Username);
        _currentUser = null;
        return Task.CompletedTask;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        var isValid = await _unitOfWork.Users.ValidatePasswordAsync(user.Username, currentPassword);
        if (!isValid) return false;

        var newHash = HashPassword(newPassword);
        await _unitOfWork.Users.ChangePasswordAsync(userId, newHash);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<UserDto> GetCurrentUserAsync()
    {
        if (_currentUser == null)
            throw new UnauthorizedAccessException("No user logged in");
        return Task.FromResult(_currentUser);
    }

    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "JewelrySalt2024"));
        return Convert.ToBase64String(bytes);
    }
}

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<DailySalesReportDto> GetDailySalesReportAsync(DateTime date)
    {
        var invoices = (await _unitOfWork.Invoices.GetByDateRangeAsync(date.Date, date.Date.AddDays(1).AddSeconds(-1))).ToList();
        return new DailySalesReportDto
        {
            Date = date,
            TotalInvoices = invoices.Count,
            TotalSales = invoices.Sum(i => i.TotalAmount),
            CashSales = invoices.SelectMany(i => i.Payments).Where(p => p.PaymentMethod == PaymentMethod.Cash && !p.IsRefund).Sum(p => p.Amount),
            CardSales = invoices.SelectMany(i => i.Payments).Where(p => p.PaymentMethod == PaymentMethod.Card && !p.IsRefund).Sum(p => p.Amount),
            UPISales = invoices.SelectMany(i => i.Payments).Where(p => p.PaymentMethod == PaymentMethod.UPI && !p.IsRefund).Sum(p => p.Amount),
            PendingAmount = invoices.Sum(i => i.PendingAmount),
            TotalItemsSold = invoices.SelectMany(i => i.InvoiceItems).Sum(item => item.Quantity)
        };
    }

    public async Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddSeconds(-1);
        var invoices = (await _unitOfWork.Invoices.GetByDateRangeAsync(from, to)).ToList();

        return new MonthlySalesReportDto
        {
            Year = year,
            Month = month,
            TotalSales = invoices.Sum(i => i.TotalAmount),
            TotalInvoices = invoices.Count
        };
    }

    public async Task<StockReportDto> GetStockReportAsync()
    {
        var products = (await _unitOfWork.Products.GetAllAsync()).ToList();
        var lowStock = await _unitOfWork.Products.GetLowStockProductsAsync();

        return new StockReportDto
        {
            TotalProducts = products.Count,
            AvailableProducts = products.Count(p => p.StockStatus == StockStatus.Available),
            SoldProducts = products.Count(p => p.StockStatus == StockStatus.Sold),
            TotalInventoryValue = products.Sum(p => p.SellingPrice * p.StockQuantity),
            LowStockItems = lowStock.Select(p => new ProductDto { Id = p.Id, ItemCode = p.ItemCode, Name = p.Name, StockQuantity = p.StockQuantity }).ToList()
        };
    }

    public async Task<CustomerPurchaseReportDto> GetCustomerPurchaseReportAsync(int? customerId, DateTime? from, DateTime? to)
    {
        var result = new CustomerPurchaseReportDto();
        if (customerId.HasValue)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId.Value);
            if (customer != null)
            {
                result.Customer = new CustomerDto { Id = customer.Id, Name = customer.Name, Mobile = customer.Mobile };
            }
        }
        var invoices = from.HasValue && to.HasValue
            ? await _unitOfWork.Invoices.GetByDateRangeAsync(from.Value, to.Value)
            : await _unitOfWork.Invoices.GetAllAsync();

        if (customerId.HasValue)
            invoices = invoices.Where(i => i.CustomerId == customerId.Value);

        var invoiceList = invoices.ToList();
        result.TotalInvoices = invoiceList.Count;
        result.TotalAmount = invoiceList.Sum(i => i.TotalAmount);
        result.PendingAmount = invoiceList.Sum(i => i.PendingAmount);
        return result;
    }

    public async Task<TaxSummaryReportDto> GetTaxSummaryReportAsync(DateTime from, DateTime to)
    {
        var invoices = (await _unitOfWork.Invoices.GetByDateRangeAsync(from, to)).ToList();
        var totalGST = invoices.Sum(i => i.TotalGST);
        return new TaxSummaryReportDto
        {
            FromDate = from,
            ToDate = to,
            TotalSales = invoices.Sum(i => i.TotalAmount),
            TotalGST = totalGST,
            TotalCGST = totalGST / 2,
            TotalSGST = totalGST / 2
        };
    }

    public async Task<ProfitLossReportDto> GetProfitLossReportAsync(DateTime from, DateTime to)
    {
        var invoices = (await _unitOfWork.Invoices.GetByDateRangeAsync(from, to)).ToList();
        var revenue = invoices.Sum(i => i.TotalAmount);
        var discount = invoices.Sum(i => i.TotalDiscount);
        var makingCharges = invoices.Sum(i => i.TotalMakingCharges);

        return new ProfitLossReportDto
        {
            FromDate = from,
            ToDate = to,
            TotalRevenue = revenue,
            MakingChargesRevenue = makingCharges,
            TotalDiscount = discount,
            NetProfit = revenue - discount
        };
    }

    public async Task<IEnumerable<PendingPaymentDto>> GetPendingPaymentsReportAsync()
    {
        var invoices = await _unitOfWork.Invoices.GetPendingInvoicesAsync();
        return invoices.Select(i => new PendingPaymentDto
        {
            InvoiceId = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            CustomerName = i.Customer?.Name ?? string.Empty,
            CustomerMobile = i.Customer?.Mobile ?? string.Empty,
            TotalAmount = i.TotalAmount,
            PaidAmount = i.PaidAmount,
            PendingAmount = i.PendingAmount,
            DaysOverdue = (DateTime.Today - i.InvoiceDate.Date).Days
        });
    }

    public Task<byte[]> ExportReportToPdfAsync(string reportType, object parameters)
    {
        return Task.FromResult(Array.Empty<byte>());
    }
}

