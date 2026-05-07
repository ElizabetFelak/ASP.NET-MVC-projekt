# Prilagodba Usmjeravanja (Routing) - Custom Routes

## Pregled

Umjesto default convention-based routinga (`/Controller/Action/{id}`), projekt koristi **attribute-based routing** za semantičke i korisničke URL-ove.

## Implementirani Custom Routingi

### 1. HomeController - Početna Stranica

**Lokacija:** `lab-2/PokemonCollector.Web/Controllers/HomeController.cs`

```csharp
[Route("")]
[Route("pocetna")]
public class HomeController : AppControllerBase
{
    [Route("")]
    [Route("pocetna")]
    public IActionResult Index()
```

| URL | Akcija | Opis |
| --- | --- | --- |
| `/` | `HomeController.Index()` | Početna stranica (root) |
| `/pocetna` | `HomeController.Index()` | Alternativni URL za početnu stranicu |
| `http://localhost:5000/` | Početak | Prikazuje top kolekcije, wish-listu i nedavne tradove |

**Primjer:** 
- Default: `http://localhost:5000/Home/Index`
- Custom: `http://localhost:5000/` ili `http://localhost:5000/pocetna`

---

### 2. PokemonCardsController - Listing i Detalji Karti

**Lokacija:** `lab-2/PokemonCollector.Web/Controllers/PokemonCardsController.cs`

```csharp
[Route("karte")]
public class PokemonCardsController : AppControllerBase
{
    [Route("")]
    [Route("index")]
    public IActionResult Index()
    
    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public IActionResult Details(int id)
```

| URL | Akcija | Opis |
| --- | --- | --- |
| `/karte` | `PokemonCardsController.Index()` | Popis svih Pokemon karti |
| `/karte/index` | `PokemonCardsController.Index()` | Alternativni URL |
| `/karte/5` | `PokemonCardsController.Details(5)` | Detalji karte s ID-om 5 |
| `/karte/detalji/5` | `PokemonCardsController.Details(5)` | Detalji s eksplicitnim "detalji" segmentom |

**Primjer:** 
- Default: `http://localhost:5000/PokemonCards/Index`
- Default: `http://localhost:5000/PokemonCards/Details/5`
- Custom: `http://localhost:5000/karte`
- Custom: `http://localhost:5000/karte/5` ili `http://localhost:5000/karte/detalji/5`

---

### 3. CollectionsController - Kolekcije Korisnika

**Lokacija:** `lab-2/PokemonCollector.Web/Controllers/CollectionsController.cs`

```csharp
[Route("kolekcije")]
public class CollectionsController : AppControllerBase
{
    [Route("")]
    [Route("index")]
    [Route("sve")]
    public IActionResult Index()
    
    [Route("{id:int}")]
    [Route("{id:int}/detalji")]
    public IActionResult Details(int id)
```

| URL | Akcija | Opis |
| --- | --- | --- |
| `/kolekcije` | `CollectionsController.Index()` | Popis svih kolekcija |
| `/kolekcije/index` | `CollectionsController.Index()` | Alternativni URL |
| `/kolekcije/sve` | `CollectionsController.Index()` | SEO-friendly URL |
| `/kolekcije/3` | `CollectionsController.Details(3)` | Detalji kolekcije s ID-om 3 |
| `/kolekcije/3/detalji` | `CollectionsController.Details(3)` | Detalji s eksplicitnim segmentom |

**Primjer:** 
- Default: `http://localhost:5000/Collections/Index`
- Default: `http://localhost:5000/Collections/Details/3`
- Custom: `http://localhost:5000/kolekcije`
- Custom: `http://localhost:5000/kolekcije/sve`
- Custom: `http://localhost:5000/kolekcije/3` ili `http://localhost:5000/kolekcije/3/detalji`

---

### 4. CardSetsController - Setovi Karti

**Lokacija:** `lab-2/PokemonCollector.Web/Controllers/CardSetsController.cs`

```csharp
[Route("setovi")]
public class CardSetsController : AppControllerBase
{
    [Route("")]
    [Route("index")]
    public IActionResult Index()
    
    [Route("{id:int}")]
    [Route("{id:int}/pregledaj")]
    public IActionResult Details(int id)
```

| URL | Akcija | Opis |
| --- | --- | --- |
| `/setovi` | `CardSetsController.Index()` | Popis svih setova karti |
| `/setovi/index` | `CardSetsController.Index()` | Alternativni URL |
| `/setovi/1` | `CardSetsController.Details(1)` | Detalji seta s ID-om 1 |
| `/setovi/1/pregledaj` | `CardSetsController.Details(1)` | Detalji s "pregledaj" segmentom |

**Primjer:** 
- Default: `http://localhost:5000/CardSets/Index`
- Default: `http://localhost:5000/CardSets/Details/1`
- Custom: `http://localhost:5000/setovi`
- Custom: `http://localhost:5000/setovi/1` ili `http://localhost:5000/setovi/1/pregledaj`

---

## Napredne Tehnike Korištene

### 1. **Multiple Routes na Jednoj Akciji**
```csharp
[Route("")]
[Route("index")]
[Route("sve")]
public IActionResult Index()
```
Ista akcija može biti dostupna s više URL-ova. Korisnika s `/kolekcije`, `/kolekcije/index` ili `/kolekcije/sve` će vesti na istu stranicu.

### 2. **Route Constraint - `{id:int}`**
```csharp
[Route("{id:int}")]
public IActionResult Details(int id)
```
Osigurava da `{id}` bude **cijeli broj**. Ako URL sadrži `/karte/abc`, neće se matchati ova ruta.

### 3. **Controller-level Route**
```csharp
[Route("karte")]
public class PokemonCardsController
```
Definiše prefiks koji će se koristiti za sve akcije u controlleru. Sve akcije u `PokemonCardsController` će počinjati s `/karte`.

### 4. **Kombiniranje Controller i Action Routinga**
```csharp
[Route("karte")]
public class PokemonCardsController
{
    [Route("detalji/{id:int}")]
    public IActionResult Details(int id)
    // Rezultira s: /karte/detalji/{id}
}
```

## Testiranje Routinga

### U Developmentu
Tijekom razvoja, ASP.NET Core automatski prikazuje sve dostupne routinge. Dodatno možeš koristiti:

```csharp
// U Program.cs
app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
    var endpoints = endpointSources.SelectMany(es => es.Endpoints).ToList();
    return endpoints;
});
```

### Stvarni Testovi

1. **Pokreni aplikaciju:**
```powershell
cd lab-2/PokemonCollector.Web
dotnet run
```

2. **Provjeri URL-ove u Browseru:**
   - `http://localhost:5000/`
   - `http://localhost:5000/karte`
   - `http://localhost:5000/kolekcije/5`
   - `http://localhost:5000/setovi/1/pregledaj`

3. **Ako se URL ne pronađe:**
   - Trebaš vidjeti `404 Not Found` stranicu
   - Provjeri je li [Route] atribut ispravno napisan

## Prednosti Custom Routinga

✅ **SEO-Friendly URLs** — `/kolekcije/5` je bolje od `/Collections/Details/5`  
✅ **Lokalizacija** — `/kolekcije` umjesto `/Collections` za hrvatske korisnike  
✅ **Čitljivost** — URL jasno pokazuje što radi  
✅ **Fleksibilnost** — Ista akcija dostupna s više URL-ova  
✅ **Keširanje** — Lakše cachirati specifične rute  

## Dodatne Rute (Budući Dev)

Ako trebašdodati novu custom rutu:

```csharp
[Route("api/karte")]
[Route("public/cards")]
public IActionResult GetCardsApi()
{
    // API verzija - dostupna s dvije različite rute
}
```

## Debugging Routinga

Ako ruta ne radi kako je očekivano:

1. **Provjeri [Route] atribut** — Je li naveden točno?
2. **Provjeri controller namn** — Trebalo bi End s "Controller"
3. **Provjerite Build** — Ponekad trebate `dotnet build` nakon izmjene
4. **Testiraje slučajno zaustavljanje** — Ponekad trebate restartuanje `dotnet run`

---

## Zaključak

Projekt sada koristi **4+ custom route-a** umjesto convention-based routinga, čineći URL-ove semantičkim i boljima za SEO. Svaka glavna akcija ima alternativne URL-ove za fleksibilnost.
