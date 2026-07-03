# ============================================================
#  Jewelry Billing Soft — Local Setup Script
#  Run this script once to set up the project on your machine.
#  Usage: .\scripts\setup-local.ps1
# ============================================================

param(
    [string]$SqlInstance = ".\SQLEXPRESS",
    [string]$DatabaseName = "JewelryBillingSoftDb"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "   Jewelry Billing Soft — Local Setup" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# ── Helper Functions ──────────────────────────────────────

function Write-Step($step, $message) {
    Write-Host "[$step] $message" -ForegroundColor Yellow
}

function Write-Success($message) {
    Write-Host "  ✅ $message" -ForegroundColor Green
}

function Write-Fail($message) {
    Write-Host "  ❌ $message" -ForegroundColor Red
}

function Write-Info($message) {
    Write-Host "  ℹ️  $message" -ForegroundColor Cyan
}

function Test-CommandExists($cmd) {
    return $null -ne (Get-Command $cmd -ErrorAction SilentlyContinue)
}

# ── Step 1: Check .NET 8 SDK ──────────────────────────────

Write-Step "1/6" "Checking .NET 8 SDK..."
if (Test-CommandExists "dotnet") {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion -match "^8\.") {
        Write-Success ".NET 8 SDK found: $dotnetVersion"
    } else {
        Write-Fail ".NET 8 SDK not found (current: $dotnetVersion)"
        Write-Info "Download from: https://dotnet.microsoft.com/download/dotnet/8"
        Write-Info "Install .NET 8 SDK and re-run this script."
        exit 1
    }
} else {
    Write-Fail ".NET SDK not found on this machine."
    Write-Info "Download from: https://dotnet.microsoft.com/download/dotnet/8"
    exit 1
}

# ── Step 2: Check SQL Server ─────────────────────────────

Write-Step "2/6" "Checking SQL Server Express..."

$sqlService = Get-Service -Name "MSSQL`$SQLEXPRESS" -ErrorAction SilentlyContinue
if ($null -eq $sqlService) {
    # Try LocalDB
    $localDbCheck = sqllocaldb info 2>$null
    if ($null -ne $localDbCheck) {
        Write-Success "SQL Server LocalDB found. Switching to LocalDB connection."
        $SqlInstance = "(localdb)\MSSQLLocalDB"
    } else {
        Write-Fail "SQL Server Express not found."
        Write-Info "Download SQL Server Express from:"
        Write-Info "https://www.microsoft.com/en-us/sql-server/sql-server-downloads"
        Write-Info ""
        Write-Info "OR install SQL Server LocalDB via Visual Studio Installer."
        exit 1
    }
} else {
    if ($sqlService.Status -ne "Running") {
        Write-Info "Starting SQL Server Express service..."
        Start-Service -Name "MSSQL`$SQLEXPRESS"
        Start-Sleep -Seconds 3
    }
    Write-Success "SQL Server Express is running."
}

# ── Step 3: Update appsettings.json ──────────────────────

Write-Step "3/6" "Configuring database connection..."

$appSettingsPath = Join-Path $ProjectRoot "src\JewelryBillingSoft.UI\appsettings.json"

if (Test-Path $appSettingsPath) {
    $connectionString = "Server=$SqlInstance;Database=$DatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"
    $appSettings = Get-Content $appSettingsPath -Raw | ConvertFrom-Json
    $appSettings.ConnectionStrings.DefaultConnection = $connectionString
    $appSettings | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath -Encoding UTF8
    Write-Success "Connection string updated: $connectionString"
} else {
    Write-Fail "appsettings.json not found at: $appSettingsPath"
    exit 1
}

# ── Step 4: Restore NuGet packages ───────────────────────

Write-Step "4/6" "Restoring NuGet packages..."
Set-Location $ProjectRoot

try {
    dotnet restore JewelryBillingSoft.sln --verbosity quiet
    Write-Success "NuGet packages restored."
} catch {
    Write-Fail "Failed to restore NuGet packages: $_"
    exit 1
}

# ── Step 5: EF Core Migrations ───────────────────────────

Write-Step "5/6" "Setting up database (EF Core migrations)..."

# Install dotnet-ef tool if not present
if (-not (Test-CommandExists "dotnet-ef")) {
    Write-Info "Installing dotnet-ef CLI tool..."
    dotnet tool install --global dotnet-ef --verbosity quiet
    # Refresh PATH
    $env:PATH = [System.Environment]::GetEnvironmentVariable("PATH", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("PATH", "User")
}

$infraProject = Join-Path $ProjectRoot "src\JewelryBillingSoft.Infrastructure"
$uiProject    = Join-Path $ProjectRoot "src\JewelryBillingSoft.UI"

Set-Location $infraProject

# Check if migrations already exist
$migrationsFolder = Join-Path $infraProject "Migrations"
if (-not (Test-Path $migrationsFolder)) {
    Write-Info "Creating initial database migration..."
    dotnet ef migrations add InitialCreate --startup-project $uiProject --verbosity quiet
    Write-Success "Migration created."
} else {
    Write-Info "Migrations folder already exists — skipping migration creation."
}

Write-Info "Applying migrations to database (this creates all tables + seed data)..."
dotnet ef database update --startup-project $uiProject --verbosity quiet
Write-Success "Database '$DatabaseName' created and ready."

# ── Step 6: Build the Solution ────────────────────────────

Write-Step "6/6" "Building the solution..."
Set-Location $ProjectRoot

try {
    dotnet build JewelryBillingSoft.sln --configuration Release --verbosity quiet
    Write-Success "Build succeeded!"
} catch {
    Write-Fail "Build failed: $_"
    Write-Info "Try running: dotnet build JewelryBillingSoft.sln"
    exit 1
}

# ── Done ─────────────────────────────────────────────────

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "   ✅ Setup Complete! Ready to Run." -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "  To run the application:" -ForegroundColor White
Write-Host "    cd src\JewelryBillingSoft.UI" -ForegroundColor Cyan
Write-Host "    dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Or double-click the built .exe at:" -ForegroundColor White
Write-Host "    src\JewelryBillingSoft.UI\bin\Release\net8.0-windows\JewelryBillingSoft.exe" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Default Login Credentials:" -ForegroundColor White
Write-Host "    Username : admin" -ForegroundColor Cyan
Write-Host "    Password : Admin@123" -ForegroundColor Cyan
Write-Host ""
Write-Host "  License Activation (first run):" -ForegroundColor White
Write-Host "    Use any 16+ char key, e.g.: DEMO-DEMO-DEMO-DEMO1" -ForegroundColor Cyan
Write-Host ""

