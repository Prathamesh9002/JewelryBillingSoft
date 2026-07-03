using JewelryBillingSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JewelryBillingSoft.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<LicenseInfo> LicenseInfos { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<ShopSettings> ShopSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Username).HasMaxLength(50).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.Mobile).HasMaxLength(15);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.CustomerCode).IsUnique();
            e.HasIndex(x => x.Mobile);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Mobile).HasMaxLength(15).IsRequired();
            e.Property(x => x.LoyaltyPoints).HasColumnType("decimal(18,2)");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ItemCode).IsUnique();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.GrossWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.StoneWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.NetWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.PurchasePrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.SellingPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.MakingCharges).HasColumnType("decimal(18,2)");
            e.Property(x => x.WastagePercentage).HasColumnType("decimal(5,2)");
            e.Property(x => x.GSTPercentage).HasColumnType("decimal(5,2)");
            e.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.InvoiceNumber).IsUnique();
            e.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalDiscount).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalGST).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalMakingCharges).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.AdvanceReceived).HasColumnType("decimal(18,2)");
            e.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.PendingAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.RefundAmount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Customer).WithMany(c => c.Invoices).HasForeignKey(x => x.CustomerId);
            e.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // InvoiceItem
        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.GrossWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.StoneWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.NetWeight).HasColumnType("decimal(10,3)");
            e.Property(x => x.RatePerGram).HasColumnType("decimal(18,2)");
            e.Property(x => x.MakingCharges).HasColumnType("decimal(18,2)");
            e.Property(x => x.WastagePercentage).HasColumnType("decimal(5,2)");
            e.Property(x => x.WastageAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountPercentage).HasColumnType("decimal(5,2)");
            e.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.GSTPercentage).HasColumnType("decimal(5,2)");
            e.Property(x => x.GSTAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Invoice).WithMany(i => i.InvoiceItems).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany(p => p.InvoiceItems).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        // Payment
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Invoice).WithMany(i => i.Payments).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        // LicenseInfo
        modelBuilder.Entity<LicenseInfo>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.LicenseKey).HasMaxLength(200).IsRequired();
            e.Property(x => x.ShopName).HasMaxLength(200);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(100);
            e.Property(x => x.EntityName).HasMaxLength(100);
            e.HasOne(x => x.User).WithMany(u => u.AuditLogs).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin user (password: Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "gGcG1J7LFqFiZABa0e2/JwKVh/NxqaXY7cjA1Lne8Bk=", // Admin@123
            FullName = "System Administrator",
            Email = "admin@jewelryshop.com",
            Mobile = "9999999999",
            Role = Domain.Enums.UserRole.Admin,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1)
        });

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Gold Necklace", MetalType = Domain.Enums.MetalType.Gold, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 2, Name = "Gold Ring", MetalType = Domain.Enums.MetalType.Gold, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 3, Name = "Gold Bangles", MetalType = Domain.Enums.MetalType.Gold, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 4, Name = "Gold Earrings", MetalType = Domain.Enums.MetalType.Gold, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 5, Name = "Silver Jewelry", MetalType = Domain.Enums.MetalType.Silver, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 6, Name = "Diamond Jewelry", MetalType = Domain.Enums.MetalType.Diamond, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 7, Name = "Custom Ornaments", MetalType = Domain.Enums.MetalType.Other, CreatedAt = new DateTime(2024, 1, 1) }
        );

        // Seed ShopSettings
        modelBuilder.Entity<ShopSettings>().HasData(new ShopSettings
        {
            Id = 1,
            ShopName = "My Jewelry Shop",
            ShopAddress = "123 Main Street, City - 400001",
            Mobile = "9876543210",
            Email = "info@jewelryshop.com",
            InvoicePrefix = "INV",
            InvoiceCounter = 1,
            CurrencySymbol = "₹",
            DefaultGSTRate = 3,
            TermsAndConditions = "All sales are final. Exchange within 30 days with original bill.",
            CreatedAt = new DateTime(2024, 1, 1)
        });

        // Seed sample products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1, ItemCode = "G0001", Name = "22K Gold Chain", CategoryId = 1,
                MetalType = Domain.Enums.MetalType.Gold, Purity = Domain.Enums.PurityType.Gold22K,
                GrossWeight = 10.5m, StoneWeight = 0, NetWeight = 10.5m,
                PurchasePrice = 55000, SellingPrice = 62000, MakingCharges = 2500,
                WastagePercentage = 2, StockQuantity = 5, GSTPercentage = 3,
                HSNCode = "7113", CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 2, ItemCode = "G0002", Name = "22K Gold Ring", CategoryId = 2,
                MetalType = Domain.Enums.MetalType.Gold, Purity = Domain.Enums.PurityType.Gold22K,
                GrossWeight = 4.2m, StoneWeight = 0, NetWeight = 4.2m,
                PurchasePrice = 22000, SellingPrice = 26000, MakingCharges = 1200,
                WastagePercentage = 1.5m, StockQuantity = 10, GSTPercentage = 3,
                HSNCode = "7113", CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 3, ItemCode = "D0001", Name = "Diamond Solitaire Ring", CategoryId = 6,
                MetalType = Domain.Enums.MetalType.Diamond, Purity = Domain.Enums.PurityType.Diamond,
                GrossWeight = 5.0m, StoneWeight = 1.2m, NetWeight = 3.8m,
                PurchasePrice = 80000, SellingPrice = 120000, MakingCharges = 5000,
                WastagePercentage = 0, StockQuantity = 3, GSTPercentage = 3,
                HSNCode = "7102", CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // Seed sample customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1, CustomerCode = "CUST0001", Name = "Rahul Sharma", Mobile = "9876543210",
                Email = "rahul@example.com", City = "Mumbai", State = "Maharashtra",
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Customer
            {
                Id = 2, CustomerCode = "CUST0002", Name = "Priya Patel", Mobile = "9876543211",
                Email = "priya@example.com", City = "Ahmedabad", State = "Gujarat",
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
            entry.Entity.UpdatedAt = DateTime.Now;

        return base.SaveChangesAsync(cancellationToken);
    }
}

