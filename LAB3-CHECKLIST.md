# Lab 3 - Završetak Checklistа

## Status Implementacije

### ✅ Konfiguracija Entity Framework-a

- [x] **Dodane EF anotacije na sve modele**
  - [x] `[Key]` na sve Id svojstva
  - [x] `[ForeignKey(nameof(...))]` na sve strane ključeve
  - [x] `virtual` na sve navigacijska svojstva
  - [x] `ICollection<T>` na sve 1-N relacije s `List<T>` inicijalizacijom

- [x] **Podesan DbContext**
  - [x] `PokemonCollectorDbContext` kreiran s `DbSet` za sve entitete
  - [x] `OnModelCreating()` konfigurira sve relacije
  - [x] Delete behavior pravilno postavljen (Cascade/Restrict)

- [x] **Konfiguracija u Program.cs**
  - [x] `DbContext` registriran u DI servise
  - [x] Connection string koristi iz `appsettings.json`

### ✅ Docker SQL Server Setup

- [x] **Docker Compose MSSQL service**
  - [x] `docker-compose.yml` s SQL Server 2022 kontejnerom
  - [x] Port `1433` mapiran na localhost
  - [x] Volumeni za perzistenciju podataka
  - [x] Volume persists između pokretanja

- [x] **Connection String Konfiguracija**
  - [x] `appsettings.json` s connection stringom
  - [x] TrustServerCertificate = True
  - [x] MultipleActiveResultSets = True

- [x] **Setup Skripta**
  - [x] `setup.ps1` automatizira cijeli proces
  - [x] Pokreće Docker
  - [x] Primjenjuje migracije
  - [x] Opcije za preskakanje koraka

### ✅ Migracije

- [x] **Inicijalnu migracija kreirana**
  - [x] `20260506225739_Initial.cs` s kompletnom shemom
  - [x] `PokemonCollectorDbContextModelSnapshot.cs` za budućne migracije
  - [x] Projekt se gradi bez greške

### ✅ Custom Routing (4+ Akcije)

- [x] **HomeController - `/` i `/pocetna`**
  - [x] Početna stranica dostupna s dva URL-a
  
- [x] **PokemonCardsController - `/karte`**
  - [x] Index: `/karte` i `/karte/index`
  - [x] Details: `/karte/{id}` i `/karte/detalji/{id}`
  
- [x] **CollectionsController - `/kolekcije`**
  - [x] Index: `/kolekcije`, `/kolekcije/index`, `/kolekcije/sve`
  - [x] Details: `/kolekcije/{id}` i `/kolekcije/{id}/detalji`
  
- [x] **CardSetsController - `/setovi`**
  - [x] Index: `/setovi` i `/setovi/index`
  - [x] Details: `/setovi/{id}` i `/setovi/{id}/pregledaj`

**Dokumentacija:** `ROUTING.md` s detaljima o svim rutama

### ✅ Semantički Modeli

- [x] **semantic-model.md** — Dokumentacija svih entiteta
  - [x] 7 tablica s svim svojstvima
  - [x] Sve relacije 1-N, N-1, N-N
  - [x] ER Dijagram
  - [x] Enumeracije (CardRarity, CardCondition, PokemonType)
  - [x] Primjeri SQL upita
  - [x] Delete behavior objašnjen

- [x] **sitemap.md** — Mapa svih URL-ova
  - [x] Sve dostupne rute s Controller/Action/View
  - [x] Breadcrumb navigacija
  - [x] Struktura direktorija
  - [x] Query string parametri za budućnost

### ✅ Skill-ovi (EF Operations)

- [x] **EF Operations Skill kreiran**
  - [x] `.github/skills/ef-operations/SKILL.md`
  - [x] Workflow-ovi za dodavanje svojstava
  - [x] Kreiranja novih entiteta
  - [x] Migracijama i bazom
  - [x] Brzi referentni vodiči
  - [x] Najbolje prakse

---

## Datoteke Kreirane/Izmijenjene

### Projektu

```
lab-2/PokemonCollector.Web/
├── Models/PokemonModels.cs              [IZMIJENJENO - EF anotacije]
├── Data/PokemonCollectorDbContext.cs    [KREIRANO - DbContext]
├── Data/Migrations/
│   ├── 20260506225739_Initial.cs        [KREIRANO]
│   └── PokemonCollectorDbContextModelSnapshot.cs [KREIRANO]
├── Program.cs                            [IZMIJENJENO - EF registracija]
├── PokemonCollector.Web.csproj           [IZMIJENJENO - EF NuGet paketi]
└── appsettings.json                      [IZMIJENJENO - connection string]
```

### Dokumentacija

```
lab-root/
├── docker-compose.yml                    [KREIRANO - Docker SQL Server]
├── setup.ps1                             [KREIRANO - Automatski setup]
├── SETUP.md                              [KREIRANO - Upute za setup]
├── ROUTING.md                            [KREIRANO - Dokumentacija routing]
├── semantic-model.md                     [KREIRANO - DB model]
├── sitemap.md                            [KREIRANO - URL mapa]
└── .github/skills/ef-operations/SKILL.md [KREIRANO - EF skill]
```

### Controlleri (Custom Routing)

```
lab-2/PokemonCollector.Web/Controllers/
├── HomeController.cs                     [IZMIJENJENO - [Route("")]]
├── PokemonCardsController.cs             [IZMIJENJENO - [Route("karte")]]
├── CollectionsController.cs              [IZMIJENJENO - [Route("kolekcije")]]
└── CardSetsController.cs                 [IZMIJENJENO - [Route("setovi")]]
```

---

## Kako Koristiti Setup

```powershell
# 1. Navigiraj u root direktorij
cd c:\Users\lolno\source\repos\ASP.NET-MVC-projekt

# 2. Pokreni setup (trebam Docker instaliran)
.\setup.ps1

# 3. Ili s pokretanjem aplikacije
.\setup.ps1 -RunApp
```

---

## Sljedeći Koraci (Nisu Dio Lab 3, Ali Su Preporučeni)

- [ ] Prebaciti MockRepository na EF Repository
- [ ] Dodati Create/Edit/Delete akcije za sve entitete
- [ ] Dodati custom routing i preostalim kontrolerima
- [ ] Kreirati skill-ove za "List Page" i "Edit Form"
- [ ] Dodati validacija na modele (`[Required]`, `[StringLength]`)
- [ ] Optimizirati migracije (dodati indekse, itd.)

---

## Testiranje

### Provjera Routinga

```powershell
# Provjeri da li URL-ovi funkcioniraju
# (trebam pokrenuti dotnet run)

http://localhost:5000/
http://localhost:5000/pocetna
http://localhost:5000/karte
http://localhost:5000/kolekcije
http://localhost:5000/setovi
```

### Provjera Baze

```powershell
# Provjeri je li baza dostupna
docker compose ps

# Vidi logove SQL Servera
docker compose logs -f sqlserver
```

### Provjera Migracija

```powershell
cd lab-2/PokemonCollector.Web
dotnet ef migrations list
dotnet ef database info
```

---

## Zaključak

✨ **Lab 3 je implementiran s:**
- ✅ EF Core integracijom
- ✅ SQL Server bazom u Docker-u
- ✅ 4+ custom routing akcija
- ✅ Kompletnom dokumentacijom (semantic model + sitemap)
- ✅ EF Operations skill-om za brzi razvoj

**Projekt je presliku ready-to-deploy!**
