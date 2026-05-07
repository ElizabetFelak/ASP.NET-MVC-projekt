# Setup skripte za PokemonCollector - Lab3
# Pokreće Docker SQL Server, primjenjuje migracije i pokreće aplikaciju

param(
    [switch]$SkipDocker,
    [switch]$SkipMigration,
    [switch]$RunApp
)

$ErrorActionPreference = "Stop"

Write-Host "PokemonCollector Lab3 Setup" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan

# Provjera trenutne lokacije
$projectRoot = Split-Path -Parent (Get-Item $PSCommandPath).FullName
$webProjectPath = Join-Path (Join-Path $projectRoot "lab-2") "PokemonCollector.Web"

if (-not (Test-Path $webProjectPath)) {
    Write-Host "❌ Nije pronađen web projekt na: $webProjectPath" -ForegroundColor Red
    exit 1
}

# ============================================
# 1. DOCKER SETUP
# ============================================
if (-not $SkipDocker) {
    Write-Host "`nPokretanje Docker SQL Server..." -ForegroundColor Yellow
    
    # Provjera Docker dostupnosti
    try {
        docker --version | Out-Null
    }
    catch {
        Write-Host "Docker nije instaliran. Instalacija je obavezna za automatsku setup." -ForegroundColor Yellow
        Write-Host "   Preuzmi Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Gray
        $response = Read-Host "   Nastavi bez Docker-a? (y/n)"
        if ($response -ne 'y') {
            exit 1
        }
        $SkipDocker = $true
    }
}

if (-not $SkipDocker) {
    Set-Location $projectRoot
    
    # Provjera je li container već pokrenut
    $containerStatus = docker ps -a --filter "name=pokemoncollector-sqlserver" --format "{{.State}}"
    
    if ($containerStatus -eq "running") {
        Write-Host "✅ SQL Server container je već pokrenut" -ForegroundColor Green
    }
    else {
        Write-Host "   Pokretanje Docker Compose..." -ForegroundColor Gray
        docker compose up -d
        
        Write-Host "   Cekanje da se SQL Server pokrene (15 sekundi)..." -ForegroundColor Gray
        Start-Sleep -Seconds 15
        
        Write-Host "SQL Server pokrenut na localhost:1433" -ForegroundColor Green
    }
}

# ============================================
# 2. ENTITY FRAMEWORK MIGRATION
# ============================================
if (-not $SkipMigration) {
    Write-Host "`nPrimjena migracija na bazu..." -ForegroundColor Yellow
    Set-Location $webProjectPath
    
    try {
        Write-Host "   Primjena 'Initial' migracije..." -ForegroundColor Gray
        dotnet ef database update --verbose
        Write-Host "✅ Migracije primijenjene uspješno" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Greška pri primjeni migracija:" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        exit 1
    }
}

# ============================================
# 3. BUILD & RUN
# ============================================
Write-Host "`nGradnja projekta..." -ForegroundColor Yellow
Set-Location $webProjectPath

    try {
    dotnet build
    Write-Host "Projekt uspjesno izgraden" -ForegroundColor Green
}
catch {
    Write-Host "❌ Greška pri gradnji projekta:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# ============================================
# 4. POKRETANJE APLIKACIJE (opciono)
# ============================================
if ($RunApp) {
    Write-Host "`nPokretanje aplikacije..." -ForegroundColor Yellow
    dotnet run
}
else {
    Write-Host "`nSetup je zavrsen!" -ForegroundColor Green
    Write-Host "   Za pokretanje aplikacije koristi:" -ForegroundColor Gray
    Write-Host "   cd '$webProjectPath'" -ForegroundColor Cyan
    Write-Host "   dotnet run" -ForegroundColor Cyan
    Write-Host "`n   Ili pokreni Setup skriptu s -RunApp zastavicom:" -ForegroundColor Gray
    Write-Host "   .\setup.ps1 -RunApp" -ForegroundColor Cyan
}

Write-Host "`nKorisni linkovi:" -ForegroundColor Gray
Write-Host "   - Aplikacija: http://localhost:5000" -ForegroundColor Gray
Write-Host "   - HTTPS: https://localhost:5001" -ForegroundColor Gray
Write-Host "   - SQL Server: localhost:1433" -ForegroundColor Gray
Write-Host "   - Docker Compose status: docker compose ps" -ForegroundColor Gray
Write-Host "   - Zaustavljanje Docker-a: docker compose down" -ForegroundColor Gray
