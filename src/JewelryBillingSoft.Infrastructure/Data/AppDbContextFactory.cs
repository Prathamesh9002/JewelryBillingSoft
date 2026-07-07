using JewelryBillingSoft.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JewelryBillingSoft.Infrastructure;

/// <summary>
/// Design-time factory used by EF Core CLI tools (dotnet ef migrations add, dotnet ef database update)
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Try to load from appsettings.json in the UI project
        var uiProjectPath = Path.Combine(
            Directory.GetCurrentDirectory(), "..", "JewelryBillingSoft.UI");

        IConfigurationRoot config;

        if (Directory.Exists(uiProjectPath) &&
            File.Exists(Path.Combine(uiProjectPath, "appsettings.json")))
        {
            config = new ConfigurationBuilder()
                .SetBasePath(uiProjectPath)
                .AddJsonFile("appsettings.json")
                .Build();
        }
        else
        {
            // Fallback to default SQL Server Express connection
            config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Server=.\\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;"
                })
                .Build();
        }

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? "Server=.\\SQLEXPRESS;Database=JewelryBillingSoftDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}

