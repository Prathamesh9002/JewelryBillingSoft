using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JewelryBillingSoft.Application.Interfaces;
using System.Windows;

namespace JewelryBillingSoft.UI.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string _statusMessage = string.Empty;
    private bool _hasError;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    protected async Task ExecuteAsync(Func<Task> action, string? busyMessage = null)
    {
        try
        {
            IsBusy = true;
            HasError = false;
            StatusMessage = busyMessage ?? "Loading...";
            await action();
            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = ex.Message;
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected void ShowSuccess(string message)
    {
        StatusMessage = message;
        HasError = false;
        MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    protected bool Confirm(string message, string title = "Confirm")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ILicenseService _licenseService;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _rememberMe;

    public event EventHandler<bool>? LoginCompleted;

    public LoginViewModel(IAuthService authService, ILicenseService licenseService)
    {
        _authService = authService;
        _licenseService = licenseService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Please enter username and password";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _authService.LoginAsync(Username, Password);
            if (result.Success)
                LoginCompleted?.Invoke(this, true);
            else
                ErrorMessage = result.Message;
        });
    }
}

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string _currentView = "Dashboard";
    [ObservableProperty] private string _currentUserName = string.Empty;
    [ObservableProperty] private string _currentUserRole = string.Empty;
    [ObservableProperty] private string _shopName = "Jewelry Billing Soft";
    [ObservableProperty] private bool _isSidebarExpanded = true;
    [ObservableProperty] private BaseViewModel? _currentViewModel;

    private readonly DashboardViewModel _dashboardViewModel;
    private readonly BillingViewModel _billingViewModel;
    private readonly CustomerViewModel _customerViewModel;
    private readonly InventoryViewModel _inventoryViewModel;
    private readonly ReportsViewModel _reportsViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    public MainViewModel(
        IAuthService authService,
        DashboardViewModel dashboardViewModel,
        BillingViewModel billingViewModel,
        CustomerViewModel customerViewModel,
        InventoryViewModel inventoryViewModel,
        ReportsViewModel reportsViewModel,
        SettingsViewModel settingsViewModel)
    {
        _authService = authService;
        _dashboardViewModel = dashboardViewModel;
        _billingViewModel = billingViewModel;
        _customerViewModel = customerViewModel;
        _inventoryViewModel = inventoryViewModel;
        _reportsViewModel = reportsViewModel;
        _settingsViewModel = settingsViewModel;

        var user = _authService.CurrentUser;
        if (user != null)
        {
            CurrentUserName = user.FullName;
            CurrentUserRole = user.RoleName;
        }

        NavigateToDashboard();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = "Dashboard";
        CurrentViewModel = _dashboardViewModel;
        _ = _dashboardViewModel.LoadAsync();
    }

    [RelayCommand]
    private void NavigateToBilling()
    {
        CurrentView = "Billing";
        CurrentViewModel = _billingViewModel;
        _ = _billingViewModel.LoadAsync();
    }

    [RelayCommand]
    private void NavigateToCustomers()
    {
        CurrentView = "Customers";
        CurrentViewModel = _customerViewModel;
        _ = _customerViewModel.LoadAsync();
    }

    [RelayCommand]
    private void NavigateToInventory()
    {
        CurrentView = "Inventory";
        CurrentViewModel = _inventoryViewModel;
        _ = _inventoryViewModel.LoadAsync();
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        CurrentView = "Reports";
        CurrentViewModel = _reportsViewModel;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = "Settings";
        CurrentViewModel = _settingsViewModel;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (Confirm("Are you sure you want to logout?"))
        {
            await _authService.LogoutAsync();
            var loginWindow = App.Services.GetService(typeof(Views.LoginWindow)) as Views.LoginWindow;
            loginWindow?.Show();
            Application.Current.Windows.OfType<Views.MainWindow>().FirstOrDefault()?.Close();
        }
    }

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarExpanded = !IsSidebarExpanded;
}

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IBillingService _billingService;
    private readonly ICustomerService _customerService;
    private readonly IInventoryService _inventoryService;
    private readonly IReportService _reportService;

    [ObservableProperty] private decimal _todaysSales;
    [ObservableProperty] private decimal _monthlySales;
    [ObservableProperty] private int _totalCustomers;
    [ObservableProperty] private int _totalProducts;
    [ObservableProperty] private decimal _pendingAmount;
    [ObservableProperty] private int _lowStockItems;
    [ObservableProperty] private int _todaysInvoices;
    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.InvoiceSummaryDto> _recentInvoices = new();

    public DashboardViewModel(
        IBillingService billingService,
        ICustomerService customerService,
        IInventoryService inventoryService,
        IReportService reportService)
    {
        _billingService = billingService;
        _customerService = customerService;
        _inventoryService = inventoryService;
        _reportService = reportService;
    }

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var dailyReport = await _reportService.GetDailySalesReportAsync(today);
            var monthlyReport = await _reportService.GetMonthlySalesReportAsync(today.Year, today.Month);
            var customers = await _customerService.GetAllAsync();
            var stockSummary = await _inventoryService.GetStockSummaryAsync();
            var lowStock = await _inventoryService.GetLowStockAsync();
            var pendingPayments = await _reportService.GetPendingPaymentsReportAsync();
            var recentInvoices = await _billingService.GetByDateRangeAsync(today.AddDays(-7), today);

            TodaysSales = dailyReport.TotalSales;
            TodaysInvoices = dailyReport.TotalInvoices;
            MonthlySales = monthlyReport.TotalSales;
            TotalCustomers = customers.Count();
            TotalProducts = stockSummary.TotalItems;
            LowStockItems = lowStock.Count();
            PendingAmount = pendingPayments.Sum(p => p.PendingAmount);

            RecentInvoices.Clear();
            foreach (var invoice in recentInvoices.Take(10))
                RecentInvoices.Add(invoice);
        });
    }
}

public partial class CustomerViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;

    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.CustomerDto> _customers = new();
    [ObservableProperty] private Application.DTOs.CustomerDto? _selectedCustomer;
    [ObservableProperty] private string _searchTerm = string.Empty;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private Application.DTOs.CreateCustomerDto _editingCustomer = new();

    public CustomerViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var customers = string.IsNullOrEmpty(SearchTerm)
                ? await _customerService.GetAllAsync()
                : await _customerService.SearchAsync(SearchTerm);

            Customers.Clear();
            foreach (var c in customers)
                Customers.Add(c);
        });
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadAsync();

    [RelayCommand]
    private void AddNew()
    {
        EditingCustomer = new Application.DTOs.CreateCustomerDto();
        SelectedCustomer = null;
        IsEditing = true;
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedCustomer == null) return;
        EditingCustomer = new Application.DTOs.CreateCustomerDto
        {
            Name = SelectedCustomer.Name,
            Mobile = SelectedCustomer.Mobile,
            AlternateMobile = SelectedCustomer.AlternateMobile,
            Email = SelectedCustomer.Email,
            Address = SelectedCustomer.Address,
            City = SelectedCustomer.City,
            State = SelectedCustomer.State
        };
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(EditingCustomer.Name) || string.IsNullOrEmpty(EditingCustomer.Mobile))
        {
            MessageBox.Show("Name and Mobile are required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (SelectedCustomer == null)
                await _customerService.CreateAsync(EditingCustomer);
            else
                await _customerService.UpdateAsync(SelectedCustomer.Id, new Application.DTOs.UpdateCustomerDto
                {
                    Id = SelectedCustomer.Id,
                    Name = EditingCustomer.Name,
                    Mobile = EditingCustomer.Mobile,
                    AlternateMobile = EditingCustomer.AlternateMobile,
                    Email = EditingCustomer.Email,
                    Address = EditingCustomer.Address,
                    City = EditingCustomer.City,
                    State = EditingCustomer.State
                });

            IsEditing = false;
            await LoadAsync();
            ShowSuccess("Customer saved successfully");
        });
    }

    [RelayCommand]
    private void CancelEdit() => IsEditing = false;

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedCustomer == null || !Confirm($"Delete customer '{SelectedCustomer.Name}'?")) return;
        await ExecuteAsync(async () =>
        {
            await _customerService.DeleteAsync(SelectedCustomer.Id);
            await LoadAsync();
        });
    }
}

public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;

    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.ProductDto> _products = new();
    [ObservableProperty] private Application.DTOs.ProductDto? _selectedProduct;
    [ObservableProperty] private string _searchTerm = string.Empty;
    [ObservableProperty] private bool _isEditing;
    [ObservableProperty] private Application.DTOs.CreateProductDto _editingProduct = new();
    [ObservableProperty] private Application.DTOs.StockSummaryDto _stockSummary = new();
    [ObservableProperty] private string _filterMetalType = "All";

    public IEnumerable<string> MetalTypes => new[] { "All", "Gold", "Silver", "Diamond", "Platinum", "Other" };
    public IEnumerable<string> PurityOptions => new[] { "Gold24K", "Gold22K", "Gold18K", "Gold14K", "Silver999", "Silver925", "Diamond", "Other" };

    public InventoryViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            IEnumerable<Application.DTOs.ProductDto> products;

            if (!string.IsNullOrEmpty(SearchTerm))
                products = await _inventoryService.SearchAsync(SearchTerm);
            else if (FilterMetalType != "All" && Enum.TryParse<Domain.Enums.MetalType>(FilterMetalType, out var metalType))
                products = await _inventoryService.GetByMetalTypeAsync(metalType);
            else
                products = await _inventoryService.GetAllAsync();

            Products.Clear();
            foreach (var p in products)
                Products.Add(p);

            StockSummary = await _inventoryService.GetStockSummaryAsync();
        });
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadAsync();

    [RelayCommand]
    private void AddNew()
    {
        EditingProduct = new Application.DTOs.CreateProductDto();
        SelectedProduct = null;
        IsEditing = true;
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedProduct == null) return;
        EditingProduct = new Application.DTOs.CreateProductDto
        {
            Name = SelectedProduct.Name,
            Description = SelectedProduct.Description,
            CategoryId = SelectedProduct.CategoryId,
            MetalType = SelectedProduct.MetalType,
            Purity = SelectedProduct.Purity,
            GrossWeight = SelectedProduct.GrossWeight,
            StoneWeight = SelectedProduct.StoneWeight,
            NetWeight = SelectedProduct.NetWeight,
            PurchasePrice = SelectedProduct.PurchasePrice,
            SellingPrice = SelectedProduct.SellingPrice,
            MakingCharges = SelectedProduct.MakingCharges,
            WastagePercentage = SelectedProduct.WastagePercentage,
            StockQuantity = SelectedProduct.StockQuantity,
            BarcodeNumber = SelectedProduct.BarcodeNumber,
            HSNCode = SelectedProduct.HSNCode,
            GSTPercentage = SelectedProduct.GSTPercentage
        };
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(EditingProduct.Name))
        {
            MessageBox.Show("Product name is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (SelectedProduct == null)
                await _inventoryService.CreateAsync(EditingProduct);
            else
                await _inventoryService.UpdateAsync(SelectedProduct.Id, new Application.DTOs.UpdateProductDto
                {
                    Id = SelectedProduct.Id,
                    Name = EditingProduct.Name,
                    Description = EditingProduct.Description,
                    CategoryId = EditingProduct.CategoryId,
                    MetalType = EditingProduct.MetalType,
                    Purity = EditingProduct.Purity,
                    GrossWeight = EditingProduct.GrossWeight,
                    StoneWeight = EditingProduct.StoneWeight,
                    NetWeight = EditingProduct.NetWeight,
                    PurchasePrice = EditingProduct.PurchasePrice,
                    SellingPrice = EditingProduct.SellingPrice,
                    MakingCharges = EditingProduct.MakingCharges,
                    WastagePercentage = EditingProduct.WastagePercentage,
                    StockQuantity = EditingProduct.StockQuantity,
                    BarcodeNumber = EditingProduct.BarcodeNumber,
                    HSNCode = EditingProduct.HSNCode,
                    GSTPercentage = EditingProduct.GSTPercentage
                });

            IsEditing = false;
            await LoadAsync();
            ShowSuccess("Product saved successfully");
        });
    }

    [RelayCommand]
    private void CancelEdit() => IsEditing = false;

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedProduct == null || !Confirm($"Delete product '{SelectedProduct.Name}'?")) return;
        await ExecuteAsync(async () =>
        {
            await _inventoryService.DeleteAsync(SelectedProduct.Id);
            await LoadAsync();
        });
    }
}

public partial class BillingViewModel : BaseViewModel
{
    private readonly IBillingService _billingService;
    private readonly ICustomerService _customerService;
    private readonly IInventoryService _inventoryService;

    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.InvoiceSummaryDto> _invoices = new();
    [ObservableProperty] private Application.DTOs.InvoiceDto? _currentInvoice;
    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.InvoiceItemDto> _currentItems = new();
    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.CustomerDto> _customers = new();
    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<Application.DTOs.ProductDto> _products = new();
    [ObservableProperty] private Application.DTOs.CustomerDto? _selectedCustomer;
    [ObservableProperty] private Application.DTOs.ProductDto? _selectedProduct;
    [ObservableProperty] private bool _isCreatingInvoice;
    [ObservableProperty] private string _searchTerm = string.Empty;
    [ObservableProperty] private DateTime _fromDate = DateTime.Today.AddDays(-30);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // New item fields
    [ObservableProperty] private int _itemQuantity = 1;
    [ObservableProperty] private decimal _itemGrossWeight;
    [ObservableProperty] private decimal _itemStoneWeight;
    [ObservableProperty] private decimal _itemNetWeight;
    [ObservableProperty] private decimal _itemRatePerGram;
    [ObservableProperty] private decimal _itemMakingCharges;
    [ObservableProperty] private decimal _itemWastagePercentage;
    [ObservableProperty] private decimal _itemDiscountPercentage;
    [ObservableProperty] private decimal _itemGstPercentage = 3;
    [ObservableProperty] private decimal _invoiceAdvance;
    [ObservableProperty] private decimal _calculatedTotal;

    // Payment
    [ObservableProperty] private decimal _cashAmount;
    [ObservableProperty] private decimal _cardAmount;
    [ObservableProperty] private decimal _upiAmount;
    [ObservableProperty] private string _upiReference = string.Empty;

    public BillingViewModel(IBillingService billingService, ICustomerService customerService, IInventoryService inventoryService)
    {
        _billingService = billingService;
        _customerService = customerService;
        _inventoryService = inventoryService;
    }

    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var invoices = await _billingService.GetByDateRangeAsync(FromDate, ToDate);
            Invoices.Clear();
            foreach (var i in invoices)
                Invoices.Add(i);

            var customers = await _customerService.GetAllAsync();
            Customers.Clear();
            foreach (var c in customers)
                Customers.Add(c);

            var products = await _inventoryService.GetAllAsync();
            Products.Clear();
            foreach (var p in products)
                Products.Add(p);
        });
    }

    [RelayCommand]
    private void NewInvoice()
    {
        CurrentItems.Clear();
        SelectedCustomer = null;
        InvoiceAdvance = 0;
        IsCreatingInvoice = true;
    }

    [RelayCommand]
    private void AddItemToInvoice()
    {
        if (SelectedProduct == null) return;
        var item = new Application.DTOs.InvoiceItemDto
        {
            ProductId = SelectedProduct.Id,
            ItemDescription = SelectedProduct.Name,
            Quantity = ItemQuantity,
            GrossWeight = ItemGrossWeight > 0 ? ItemGrossWeight : SelectedProduct.GrossWeight,
            StoneWeight = ItemStoneWeight > 0 ? ItemStoneWeight : SelectedProduct.StoneWeight,
            NetWeight = ItemNetWeight > 0 ? ItemNetWeight : SelectedProduct.NetWeight,
            Purity = SelectedProduct.PurityName,
            RatePerGram = ItemRatePerGram > 0 ? ItemRatePerGram : SelectedProduct.SellingPrice / SelectedProduct.NetWeight,
            MakingCharges = ItemMakingCharges > 0 ? ItemMakingCharges : SelectedProduct.MakingCharges,
            WastagePercentage = ItemWastagePercentage > 0 ? ItemWastagePercentage : SelectedProduct.WastagePercentage,
            DiscountPercentage = ItemDiscountPercentage,
            GSTPercentage = ItemGstPercentage,
            HSNCode = SelectedProduct.HSNCode
        };

        // Calculate totals
        var metalValue = item.NetWeight * item.RatePerGram;
        var wastage = metalValue * (item.WastagePercentage / 100);
        var sub = (metalValue + wastage + item.MakingCharges) * item.Quantity;
        var discount = sub * (item.DiscountPercentage / 100);
        var taxable = sub - discount;
        var gst = taxable * (item.GSTPercentage / 100);

        item.WastageAmount = wastage;
        item.SubTotal = sub;
        item.DiscountAmount = discount;
        item.GSTAmount = gst;
        item.TotalAmount = taxable + gst;

        CurrentItems.Add(item);
        RecalculateTotal();
    }

    [RelayCommand]
    private void RemoveItem(Application.DTOs.InvoiceItemDto item)
    {
        CurrentItems.Remove(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        CalculatedTotal = CurrentItems.Sum(i => i.TotalAmount);
    }

    [RelayCommand]
    private async Task FinalizeInvoiceAsync()
    {
        if (SelectedCustomer == null || !CurrentItems.Any())
        {
            MessageBox.Show("Please select a customer and add at least one item", "Validation",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await ExecuteAsync(async () =>
        {
            var createDto = new Application.DTOs.CreateInvoiceDto
            {
                CustomerId = SelectedCustomer.Id,
                AdvanceReceived = InvoiceAdvance,
                Items = CurrentItems.Select(i => new Application.DTOs.CreateInvoiceItemDto
                {
                    ProductId = i.ProductId,
                    ItemDescription = i.ItemDescription,
                    Quantity = i.Quantity,
                    GrossWeight = i.GrossWeight,
                    StoneWeight = i.StoneWeight,
                    NetWeight = i.NetWeight,
                    Purity = i.Purity,
                    RatePerGram = i.RatePerGram,
                    MakingCharges = i.MakingCharges,
                    WastagePercentage = i.WastagePercentage,
                    DiscountPercentage = i.DiscountPercentage,
                    GSTPercentage = i.GSTPercentage,
                    HSNCode = i.HSNCode
                }).ToList()
            };

            var invoice = await _billingService.CreateInvoiceAsync(createDto);

            var payments = new List<Application.DTOs.PaymentDto>();
            if (CashAmount > 0)
                payments.Add(new Application.DTOs.PaymentDto { PaymentMethod = Domain.Enums.PaymentMethod.Cash, Amount = CashAmount });
            if (CardAmount > 0)
                payments.Add(new Application.DTOs.PaymentDto { PaymentMethod = Domain.Enums.PaymentMethod.Card, Amount = CardAmount });
            if (UpiAmount > 0)
                payments.Add(new Application.DTOs.PaymentDto { PaymentMethod = Domain.Enums.PaymentMethod.UPI, Amount = UpiAmount, TransactionReference = UpiReference });

            if (payments.Any())
                await _billingService.FinalizeInvoiceAsync(invoice.Id, payments);

            CurrentInvoice = await _billingService.GetByIdAsync(invoice.Id);
            IsCreatingInvoice = false;
            await LoadAsync();
            ShowSuccess($"Invoice {invoice.InvoiceNumber} created successfully");
        });
    }

    [RelayCommand]
    private void CancelInvoice()
    {
        CurrentItems.Clear();
        IsCreatingInvoice = false;
    }

    [RelayCommand]
    private async Task ViewInvoiceAsync(Application.DTOs.InvoiceSummaryDto summary)
    {
        CurrentInvoice = await _billingService.GetByIdAsync(summary.Id);
    }

    [RelayCommand]
    private async Task PrintInvoiceAsync()
    {
        if (CurrentInvoice == null) return;
        await ExecuteAsync(async () =>
        {
            var pdfBytes = await _billingService.GenerateInvoicePdfAsync(CurrentInvoice.Id);
            var tempFile = Path.Combine(Path.GetTempPath(), $"Invoice_{CurrentInvoice.InvoiceNumber}.pdf");
            await File.WriteAllBytesAsync(tempFile, pdfBytes);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true });
        });
    }
}

public partial class ReportsViewModel : BaseViewModel
{
    private readonly IReportService _reportService;

    [ObservableProperty] private DateTime _reportFromDate = DateTime.Today.AddDays(-30);
    [ObservableProperty] private DateTime _reportToDate = DateTime.Today;
    [ObservableProperty] private string _selectedReportType = "Daily Sales";
    [ObservableProperty] private object? _reportData;

    public IEnumerable<string> ReportTypes => new[]
    {
        "Daily Sales", "Monthly Sales", "Stock Report", "Pending Payments", "Tax Summary", "Profit & Loss"
    };

    public ReportsViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [RelayCommand]
    private async Task GenerateReportAsync()
    {
        await ExecuteAsync(async () =>
        {
            ReportData = SelectedReportType switch
            {
                "Daily Sales" => await _reportService.GetDailySalesReportAsync(ReportFromDate),
                "Monthly Sales" => await _reportService.GetMonthlySalesReportAsync(ReportFromDate.Year, ReportFromDate.Month),
                "Stock Report" => await _reportService.GetStockReportAsync(),
                "Pending Payments" => await _reportService.GetPendingPaymentsReportAsync(),
                "Tax Summary" => await _reportService.GetTaxSummaryReportAsync(ReportFromDate, ReportToDate),
                "Profit & Loss" => await _reportService.GetProfitLossReportAsync(ReportFromDate, ReportToDate),
                _ => null
            };
        });
    }
}

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty] private string _shopName = "My Jewelry Shop";
    [ObservableProperty] private string _shopAddress = string.Empty;
    [ObservableProperty] private string _mobile = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _gstNumber = string.Empty;
    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;

    [RelayCommand]
    private void SaveSettings()
    {
        ShowSuccess("Settings saved successfully");
    }

    [RelayCommand]
    private void ChangePassword()
    {
        if (NewPassword != ConfirmPassword)
        {
            MessageBox.Show("Passwords do not match", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        ShowSuccess("Password changed successfully");
    }
}

public partial class LicenseViewModel : BaseViewModel
{
    private readonly ILicenseService _licenseService;

    [ObservableProperty] private string _licenseKey = string.Empty;
    [ObservableProperty] private string _shopName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private Application.DTOs.LicenseStatusDto? _licenseStatus;
    [ObservableProperty] private string _activationMessage = string.Empty;

    public event EventHandler? ActivationSucceeded;

    public LicenseViewModel(ILicenseService licenseService)
    {
        _licenseService = licenseService;
        _ = LoadLicenseStatusAsync();
    }

    public async Task LoadLicenseStatusAsync()
    {
        LicenseStatus = await _licenseService.GetLicenseStatusAsync();
    }

    [RelayCommand]
    private async Task ActivateAsync()
    {
        if (string.IsNullOrEmpty(LicenseKey) || string.IsNullOrEmpty(ShopName))
        {
            ActivationMessage = "License key and shop name are required";
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _licenseService.ActivateLicenseAsync(LicenseKey, ShopName, Email);
            ActivationMessage = result.Message;
            if (result.Success)
            {
                LicenseStatus = result.LicenseStatus;
                ActivationSucceeded?.Invoke(this, EventArgs.Empty);
            }
        });
    }
}

