-- =============================================
-- Jewelry Billing Soft - Database Setup Script
-- SQL Server Express
-- =============================================

USE master;
GO

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'JewelryBillingSoftDb')
BEGIN
    CREATE DATABASE JewelryBillingSoftDb;
END
GO

USE JewelryBillingSoftDb;
GO

-- The application uses Entity Framework Core Migrations
-- Run the application once to auto-create all tables.
-- OR run: dotnet ef database update

-- Manual table creation (backup option)
-- Tables will be created by EF Core on first run.

-- Verify connection
SELECT 'Database ready: ' + DB_NAME() AS [Status];
GO

