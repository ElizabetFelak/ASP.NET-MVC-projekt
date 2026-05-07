# Sitemap (Mapa URL-ova) - PokemonCollector

## Pregled

Dokument opisuje sve dostupne URL-ove u aplikaciji, njihove controllere, akcije i odgovarajuće view-ove.

---

## Početna Stranica

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/` | `HomeController` | `Index()` | `Views/Home/Index.cshtml` | Početna stranica s top kolekcijama, wish-listom i tradenjem |
| `/pocetna` | `HomeController` | `Index()` | `Views/Home/Index.cshtml` | Alternativni URL za početnu stranicu |

**Model:** `HomeIndexViewModel`
- TopCollections (List<Collection>)
- TopWishlistItems (List<Wishlist>)
- LatestTrades (List<Trade>)

---

## Pokemon Kartice - `/karte`

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/karte` | `PokemonCardsController` | `Index()` | `Views/PokemonCards/Index.cshtml` | Popis svih Pokemon karti |
| `/karte/index` | `PokemonCardsController` | `Index()` | `Views/PokemonCards/Index.cshtml` | Alternativni URL |
| `/karte/{id}` | `PokemonCardsController` | `Details(int id)` | `Views/PokemonCards/Details.cshtml` | Detalji specifične kartice |
| `/karte/detalji/{id}` | `PokemonCardsController` | `Details(int id)` | `Views/PokemonCards/Details.cshtml` | Alternativni URL s "detalji" segmentom |

**Model na Index:**
- List<PokemonCard>

**Model na Details:**
- PokemonCard (s ForeignKey-om CardSet)
- CardSet informacije
- CardInstances (ako su dostupne)

**Primjeri:**
- `http://localhost:5000/karte` → Sve kartice
- `http://localhost:5000/karte/42` → Kartice s ID-om 42
- `http://localhost:5000/karte/detalji/42` → Isti kao gornji

---

## Kartični Setovi - `/setovi`

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/setovi` | `CardSetsController` | `Index()` | `Views/CardSets/Index.cshtml` | Popis svih setova karti |
| `/setovi/index` | `CardSetsController` | `Index()` | `Views/CardSets/Index.cshtml` | Alternativni URL |
| `/setovi/{id}` | `CardSetsController` | `Details(int id)` | `Views/CardSets/Details.cshtml` | Detalji specifičnog seta |
| `/setovi/{id}/pregledaj` | `CardSetsController` | `Details(int id)` | `Views/CardSets/Details.cshtml` | Alternativni URL |

**Model na Index:**
- List<CardSet>

**Model na Details:**
- CardSet (s informacijama o izdanju)
- ICollection<PokemonCard> Cards (sve kartice u setu)

**Primjeri:**
- `http://localhost:5000/setovi` → Svi setovi
- `http://localhost:5000/setovi/1` → Set s ID-om 1
- `http://localhost:5000/setovi/1/pregledaj` → Isti kao gornji

---

## Kolekcije - `/kolekcije`

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/kolekcije` | `CollectionsController` | `Index()` | `Views/Collections/Index.cshtml` | Popis svih kolekcija |
| `/kolekcije/index` | `CollectionsController` | `Index()` | `Views/Collections/Index.cshtml` | Alternativni URL |
| `/kolekcije/sve` | `CollectionsController` | `Index()` | `Views/Collections/Index.cshtml` | SEO-friendly URL |
| `/kolekcije/{id}` | `CollectionsController` | `Details(int id)` | `Views/Collections/Details.cshtml` | Detalji specifične kolekcije |
| `/kolekcije/{id}/detalji` | `CollectionsController` | `Details(int id)` | `Views/Collections/Details.cshtml` | Alternativni URL |

**Model na Index:**
- List<Collection>

**Model na Details:**
- Collection (s User informacijama)
- ICollection<CardInstance> CardInstances
- Korisnik koji posjeduje kolekciju
- Ukupna vrijednost

**Primjeri:**
- `http://localhost:5000/kolekcije` → Sve kolekcije
- `http://localhost:5000/kolekcije/sve` → SEO verzija
- `http://localhost:5000/kolekcije/5` → Kolekcija s ID-om 5
- `http://localhost:5000/kolekcije/5/detalji` → Isti kao gornji

---

## Instanci Karti - CardInstances

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/instanci` | `CardInstancesController` | `Index()` | `Views/CardInstances/Index.cshtml` | Popis svih instanci karti |
| `/instanci/{id}` | `CardInstancesController` | `Details(int id)` | `Views/CardInstances/Details.cshtml` | Detalji specifične instance |

*Trenutno koristi default routing. Trebalo bi dodati custom routing ako je potrebno.*

---

## Tradei - Trades

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/tradei` | `TradesController` | `Index()` | `Views/Trades/Index.cshtml` | Popis svih tradea |
| `/tradei/{id}` | `TradesController` | `Details(int id)` | `Views/Trades/Details.cshtml` | Detalji specifičnog tradea |

*Trenutno koristi default routing. Trebalo bi dodati custom routing ako je potrebno.*

---

## Korisnici - Users

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/korisnici` | `UsersController` | `Index()` | `Views/Users/Index.cshtml` | Popis svih korisnika |
| `/korisnici/{id}` | `UsersController` | `Details(int id)` | `Views/Users/Details.cshtml` | Profil korisnika |

*Trenutno koristi default routing. Trebalo bi dodati custom routing ako je potrebno.*

---

## Wish-lista - Wishlists

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/wishlist` | `WishlistsController` | `Index()` | `Views/Wishlists/Index.cshtml` | Popis svih wish-listi |
| `/wishlist/{id}` | `WishlistsController` | `Details(int id)` | `Views/Wishlists/Details.cshtml` | Detalji specifične stavke |

*Trenutno koristi default routing. Trebalo bi dodati custom routing ako je potrebno.*

---

## Error Handling

| URL | Controller | Action | View | Opis |
| --- | --- | --- | --- | --- |
| `/Home/Error` | `HomeController` | `Error()` | `Views/Home/Error.cshtml` | Greška (404, 500, itd.) |
| Bilo koja nepostojeća ruta | Fallback | — | `Views/Home/Error.cshtml` | Ako ruta nije pronađena |

---

## Shared Layout & Partials

| View | Lokacija | Opis |
| --- | --- | --- |
| `_Layout.cshtml` | `Views/Shared/` | Glavna stranica (header, footer, navbar) |
| `_Breadcrumbs.cshtml` | `Views/Shared/` | Breadcrumb navigacija |
| `_ViewStart.cshtml` | `Views/` | Automatski koristi `_Layout.cshtml` |
| `_ViewImports.cshtml` | `Views/` | Globalni using statements i tag helperi |

---

## Strukturiranje Direktorija

```
Views/
├── Home/
│   ├── Index.cshtml                 (Početna stranica)
│   ├── Error.cshtml                 (Error stranica)
│   └── Privacy.cshtml               (Privacy policy)
├── PokemonCards/
│   ├── Index.cshtml                 (Popis karti)
│   └── Details.cshtml               (Detalji kartice)
├── CardSets/
│   ├── Index.cshtml                 (Popis setova)
│   └── Details.cshtml               (Detalji seta)
├── Collections/
│   ├── Index.cshtml                 (Popis kolekcija)
│   └── Details.cshtml               (Detalji kolekcije)
├── CardInstances/
│   ├── Index.cshtml                 (Popis instanci)
│   └── Details.cshtml               (Detalji instance)
├── Trades/
│   ├── Index.cshtml                 (Popis tradea)
│   └── Details.cshtml               (Detalji tradea)
├── Users/
│   ├── Index.cshtml                 (Popis korisnika)
│   └── Details.cshtml               (Profil korisnika)
├── Wishlists/
│   ├── Index.cshtml                 (Popis wish-lista)
│   └── Details.cshtml               (Detalji stavke)
└── Shared/
    ├── _Layout.cshtml               (Master stranica)
    ├── _Breadcrumbs.cshtml          (Breadcrumb navigacija)
    ├── _Layout.cshtml.css           (CSS za layout)
    ├── _ViewStart.cshtml
    └── _ViewImports.cshtml
```

---

## Breadcrumb Navigacija

Svaka stranica osim početne ima breadcrumb navigaciju:

| Stranica | Breadcrumb |
| --- | --- |
| `/karte` | Home > Pokemon Cards |
| `/karte/5` | Home > Pokemon Cards > [Kartice Ime] |
| `/kolekcije/3` | Home > Collections > [Naziv Kolekcije] |
| `/setovi/1` | Home > Card Sets > [Naziv Seta] |

---

## Query String Parametri

Trenutno nije korišteno, ali sljedeće mogu biti dodane:

```
/karte?sort=rarity&filter=rare&page=1
/kolekcije?userId=5&sort=value&page=2
/setovi?year=2024&publisher=pokemon
```

---

## Provjera Dostupnih Ruta

Tijekom razvoja, može se provjeriti sve dostupne rute:

```powershell
# U PowerShell-u
curl http://localhost:5000/karte
curl http://localhost:5000/kolekcije
curl http://localhost:5000/setovi
```

---

## Napomene o Rutama

- ✅ **Custom Routing** — `/karte`, `/setovi`, `/kolekcije` koriste [Route] atribute
- ✅ **Semantic URLs** — URL-ovi su čitljivi i SEO-friendly
- ⚠️ **Alternativni URL-ovi** — Većina akcija ima više mogućih URL-ova (fleksibilnost)
- ❌ **Default Routing** — CardInstances, Trades, Users, Wishlists još koriste convention-based
- 📝 **Buduća Optimizacija** — Trebalo bi dodati custom routing i ostalim kontrolerima

---

## Zaključak

Sitemap dokumentira sve dostupne URL-ove i njihove dijelove. Custom routing na główne stranice čini navigaciju jasnijom i boljom za SEO.

Trebalo bi nastaviti s custom routingom i za:
- `/instanci` (CardInstances)
- `/tradei` (Trades)
- `/korisnici` (Users)
- `/wishlist` (Wishlists)
