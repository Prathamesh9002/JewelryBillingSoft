using JewelryBillingSoft.Application.Interfaces;
using JewelryBillingSoft.Infrastructure;
using JewelryBillingSoft.UI.ViewModels;
using JewelryBillingSoft.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using WpfApp = System.Windows.Application;

namespace JewelryBillingSoft.UI;

public partial class App : WpfApp
{
    private ServiceProvider? _serviceProvider;

    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        // Initialize database
        try
        {
            await DependencyInjection.InitializeDatabaseAsync(_serviceProvider);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database initialization failed:\n{ex.Message}\n\nPlease check that the application data directory is writable.",
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        // Check license
        var licenseService = _serviceProvider.GetRequiredService<ILicenseService>();
        var isValid = await licenseService.IsLicenseValidAsync();

        if (!isValid)
        {
            var licenseWindow = _serviceProvider.GetRequiredService<LicenseActivationWindow>();
            licenseWindow.Show();
        }
        else
        {
            // Check if license is expiring soon
            var remainingDays = await licenseService.GetRemainingDaysAsync();
            if (remainingDays <= 30)
            {
                MessageBox.Show(
                    $"Your license will expire in {remainingDays} days. Please renew to continue using the software.",
                    "License Expiry Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddInfrastructure(configuration);
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<BillingViewModel>();
        services.AddTransient<CustomerViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LicenseViewModel>();

        // Register Windows
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<LicenseActivationWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

