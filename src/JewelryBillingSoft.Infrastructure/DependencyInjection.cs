using JewelryBillingSoft.Application.Interfaces;
using JewelryBillingSoft.Application.Services;
using JewelryBillingSoft.Infrastructure.Data;
using JewelryBillingSoft.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryBillingSoft.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Resolve and expand the SQLite connection string (supports %APPDATA% etc.)
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=jewelry.db";
        var connectionString = Environment.ExpandEnvironmentVariables(rawConnectionString);

        // Ensure the database directory exists
        var dataSource = connectionString
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .FirstOrDefault(p => p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            ?.Substring("Data Source=".Length)
            .Trim();
        if (!string.IsNullOrWhiteSpace(dataSource))
        {
            var dir = Path.GetDirectoryName(dataSource);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);
        }

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories and Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application Services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}

