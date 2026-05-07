---
name: EF Operations
description: "Use when: adding properties to EF models, creating new migrations, updating database schema, modifying relationships between entities, configuring DbContext. Perfect for Lab 3 Entity Framework tasks."
---

# EF Operations Skill - PokemonCollector Lab 3

## Namjena

Ovaj skill pruža brze workflow-ove za česte Entity Framework operacije:
- Dodavanje novih svojstava na postojeće modele
- Kreiranja novih entiteta
- Provjera i ažuriranje migracija
- Konfiguracija veza između entiteta
- Primjena migracija na bazu

## Primjena

Koristi ovaj skill kada trebašslobodno sa:
- ❌ Kompleksnim C# logikom izvan modela
- ❌ Web kontrolerima i poslovnom logikom
- ✅ Modelima u `Models/PokemonModels.cs`
- ✅ DbContext konfiguraciji
- ✅ Migracijama i bazom podataka

---

## Brzi Workflow-ovi

### 1. Dodavanje Svojstva na Entitet

**Zadatak:** Trebam dodati `Description` svojstvo na `CardSet` model.

**Koraci:**
1. Otvori `lab-2/PokemonCollector.Web/Models/PokemonModels.cs`
2. Nađi klasu `CardSet`
3. Dodaj novo svojstvo:
   ```csharp
   public string Description { get; set; } = string.Empty;
   ```
4. Spremi datoteku
5. Kreiraj migraciju:
   ```powershell
   cd lab-2/PokemonCollector.Web
   dotnet ef migrations add AddDescriptionToCardSet
   ```
6. Primijeni migraciju:
   ```powershell
   dotnet ef database update
   ```

---

### 2. Kreiranje Novog Entiteta

**Zadatak:** Trebam novi entitet `Review` za recenzije karti.

**Koraci:**

1. Dodaj novu klasu u `PokemonModels.cs`:
```csharp
public class Review
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [ForeignKey(nameof(PokemonCard))]
    public int PokemonCardId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 zvjezdica
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigacijska svojstva
    public virtual User? User { get; set; }
    public virtual PokemonCard? PokemonCard { get; set; }
}
```

2. Dodaj DbSet u `PokemonCollectorDbContext.cs`:
```csharp
public DbSet<Review> Reviews { get; set; } = null!;
```

3. Dodaj kolekciju na `User`:
```csharp
public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
```

4. Dodaj kolekciju na `PokemonCard`:
```csharp
public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
```

5. Kreiraj i primijeni migraciju:
```powershell
dotnet ef migrations add AddReviewEntity
dotnet ef database update
```

---

### 3. Promjena Tipa Svojstva

**Primjer:** Trebam promijeniti `Budget` sa `decimal` na `float`

1. Otvori model
2. Promijeni svojstvo
3. Kreiraj migraciju s opisnim imenom
4. Primijeni migraciju

---

### 4. Dodavanje Nova Veza Između Entiteta

**Primjer:** Trebam dodati vezu između `PokemonCard` i `User` (npr. za "favorited cards")

1. Dodaj foreign key na jednom od entiteta
2. Dodaj navigacijska svojstva na oba
3. Konfigurira vezu u `DbContext.OnModelCreating()`
4. Kreiraj migraciju
5. Primijeni migraciju

---

## Naredbe za Migracije

```powershell
# Lokacija: c:\Users\lolno\source\repos\ASP.NET-MVC-projekt\lab-2\PokemonCollector.Web

# Kreiraj novu migraciju s opisom
dotnet ef migrations add DescriptiveName

# Prikaži pending migracije (koje nisu primijenjene)
dotnet ef migrations list

# Ukloni zadnju migraciju (ako još nije primljena)
dotnet ef migrations remove

# Primijeni sve pending migracije
dotnet ef database update

# Skini migracije do specifične migracije
dotnet ef database update InitialMigration

# Skini sve migracije
dotnet ef database update 0

# Generiraj SQL skriptu bez primjene
dotnet ef migrations script

# Prikaži trenutno stanje baze
dotnet ef dbcontext info
```

---

## EF Anotacije - Brz Pregled

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Example
{
    [Key]                           // Primarni ključ
    public int Id { get; set; }

    [Required]                      // Obavezno polje
    public string Name { get; set; }

    [StringLength(100)]             // Max 100 znakova
    public string Description { get; set; }

    [Range(1, 100)]                 // Raspon vrijednosti
    public int Rating { get; set; }

    [EmailAddress]                  // Email validacija
    public string Email { get; set; }

    [ForeignKey(nameof(User))]      // Strani ključ
    public int UserId { get; set; }

    [MaxLength(50)]                 // Max duljina
    public string Code { get; set; }

    [NotMapped]                     // Ne mapira se u bazu
    public string TempProperty { get; set; }

    // Navigacijska svojstva - MORA biti virtual!
    public virtual User User { get; set; }
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
```

---

## Česta Greška: Nekompatibilne Migracije

**Greška:** `The model backing the 'PokemonCollectorDbContext' context has changed...`

**Rješenje:** Trebam obrisati zadnju migraciju i ponovno je kreiratiš:
```powershell
dotnet ef migrations remove
dotnet ef migrations add ProperName
dotnet ef database update
```

---

## Provjera Konfiguracije

```csharp
// U Program.cs:
var options = new DbContextOptionsBuilder<PokemonCollectorDbContext>()
    .UseSqlServer("connection-string")
    .Options;

var context = new PokemonCollectorDbContext(options);

// Provjera je li model validan
context.Database.EnsureCreated();  // Kreira bazu ako ne postoji
```

---

## Najbolje Prakse

✅ **Uvijek koristi `virtual` na navigacijskim svojstvima**
✅ **Uvijek koristi `[ForeignKey(nameof(...))]` umjesto stringova**
✅ **Inicijaliziraj `ICollection<T>` sa `new List<T>()`**
✅ **Koristi opisna imena za migracije**: `AddEmailToUser` umjesto `AddColumn`
✅ **Primijeni migracije odmah nakon kreiranja**

❌ **Ne koristi `using` s nullable koncepta** - EF to pravi probleme
❌ **Ne briši migracije nakon što su primijenjene**
❌ **Ne koristi `[Table]` atribut ako nije potrebno**

---

## Povezane Datoteke

- **Modeli:** `lab-2/PokemonCollector.Web/Models/PokemonModels.cs`
- **DbContext:** `lab-2/PokemonCollector.Web/Data/PokemonCollectorDbContext.cs`
- **Migracije:** `lab-2/PokemonCollector.Web/Migrations/`
- **Connection String:** `lab-2/PokemonCollector.Web/appsettings.json`

---

## Когда Trebam Ovaj Skill

- [x] Trebam dodati novo svojstvo na model
- [x] Trebam kreirati novi entitet
- [x] Trebam promijeniti tip svojstva
- [x] Trebam dodati vezu između entiteta
- [x] Trebam kreirira migraciju
- [x] Trebam primiti migraciju
- [x] Trebam provjeriti status migracija
- [x] Trebam obrisati zadnju migraciju

---

## Primjer: Kompletan EF Workflow

**Zadatak:** Trebam dodati `Timestamp` svojstvo na sve entitete.

1. **Dodaj svojstvo u sve klase:**
```csharp
[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
public DateTime LastModified { get; set; } = DateTime.UtcNow;
```

2. **Kreiraj migraciju:**
```powershell
dotnet ef migrations add AddLastModifiedToAllEntities
```

3. **Provjeri migraciju:**
```powershell
dotnet ef migrations list
```

4. **Primijeni:**
```powershell
dotnet ef database update
```

5. **Provjeri da je sve dobro:**
```powershell
dotnet ef dbcontext info
```

---

Ovaj skill osigurava brz i siguran rad s Entity Framework-om bez greške!
