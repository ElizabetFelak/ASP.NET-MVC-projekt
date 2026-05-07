# PokemonCollector - Lab 3 Setup Upute

## Brzi Start

```powershell
# Iz roditeljskog direktorija projekta:
.\setup.ps1

# Ili s pokretanjem aplikacije:
.\setup.ps1 -RunApp
```

## Što skripte radi?

Setup skripta automatski:
1. ✅ Pokreće Docker SQL Server container
2. ✅ Čeka da se SQL Server inicijalizira (~15 sekundi)
3. ✅ Primjenjuje EF migracije (kreira tablice u bazi)
4. ✅ Gradi projekt

## Dostupne Opcije

```powershell
# Preskočiti Docker setup (ako je već pokrenut)
.\setup.ps1 -SkipDocker

# Preskočiti migracije (ako su već primijenjene)
.\setup.ps1 -SkipMigration

# Odmah pokrenuti aplikaciju nakon setup-a
.\setup.ps1 -RunApp

# Kombinacije:
.\setup.ps1 -SkipDocker -SkipMigration -RunApp
```

## Ručni Koraci (Ako Skriptu Nećeš Koristiti)

### 1. Pokreni SQL Server u Docker-u
```powershell
cd c:\Users\lolno\source\repos\ASP.NET-MVC-projekt
docker compose up -d
```

### 2. Primijeni Migracije
```powershell
cd c:\Users\lolno\source\repos\ASP.NET-MVC-projekt\lab-2\PokemonCollector.Web
dotnet ef database update
```

### 3. Pokreni Aplikaciju
```powershell
dotnet run
```

## Connection String

```
Server=localhost,1433;
Database=PokemonCollectorDb;
User Id=sa;
Password=YourStrong(!)Password123;
TrustServerCertificate=True;
Encrypt=False;
MultipleActiveResultSets=True
```

## Korisni Docker Komandi

```powershell
# Status SQL Server containera
docker compose ps

# Zaustavljanje SQL Server-a
docker compose down

# Prikaz SQL Server logova
docker compose logs -f sqlserver

# Ponovno pokretanje bez brisanja podataka
docker compose restart
```

## Provjera je li SQL Server Spreman

```powershell
# Koristi SQL Server Management Studio ili:
sqlcmd -S localhost,1433 -U sa -P "YourStrong(!)Password123" -Q "SELECT @@VERSION"
```

## Greške i Rješenja

### "docker: The term 'docker' is not recognized"
- **Rješenje:** Instaliraj Docker Desktop s https://www.docker.com/products/docker-desktop

### "Cannot open server requested by the login"
- **Rješenje:** Pričekaj 30 sekundi da se SQL Server u potpunosti inicijalizira, pa ponovi migraciju

### "Migrations applied but table is empty"
- **Rješenje:** To je normalno! Baza je prazna. Mock podaci se koriste iz `MockDataFactory.cs`

## Dodatne Migracije

Ako dodaš novo svojstvo u model:

```powershell
# Kreiraj novu migraciju
dotnet ef migrations add AddNewProperty

# Primijeni je na bazu
dotnet ef database update
```

## Struktura Direktorija

```
lab-2/PokemonCollector.Web/
├── Data/
│   ├── PokemonCollectorDbContext.cs    # DbContext
│   ├── IPokemonRepository.cs           # Repository interface
│   └── MockPokemonRepository.cs        # Mock implementacija
├── Migrations/
│   ├── 20260506225739_Initial.cs       # Inicijalna migracija
│   └── PokemonCollectorDbContextModelSnapshot.cs
├── Models/
│   └── PokemonModels.cs                # Svi entiteti (User, Card, Collection...)
└── Program.cs                          # EF registracija
```

## Sljedeći Koraci (Lab3 Zadaci)

- [ ] Prebaciti app s mock repository na EF repository
- [ ] Podesiti 4+ custom routing akcije
- [ ] Kreirati `semantic-model.md` (modeli i veze)
- [ ] Kreirati `sitemap.md` (URL struktura)
- [ ] Konfigurirati skill-ove (EF, List stranica, Edit forma)
