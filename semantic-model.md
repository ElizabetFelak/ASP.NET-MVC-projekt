# Semantički Model Baze Podataka - PokemonCollector

## Pregled Strukture

Projekt koristi **Entity Framework Core** s SQL Server bazom. Baza sadrži 7 glavnih entiteta s релацијама koje omogućavaju upravljanje Pokemon kartama, kolekcijama, tradenjem i wish-listom.

---

## Tablica: User

**Opis:** Predstavlja korisnika aplikacije

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ (Primary Key) |
| `Username` | `string` | Jedinstveno korisničko ime |
| `Email` | `string` | Email adresa |
| `RegistrationDate` | `DateTime` | Datum registracije korisnika |
| `Budget` | `decimal` | Proračun korisnika za kupnju |
| `PhoneNumber` | `string` | Telefonski broj |
| `Address` | `string` | Kućna adresa |

**Relacije:**
- 1-N s `Collection` — Jedan korisnik ima više kolekcija
- 1-N s `Trade` (kao Sender) — Korisnik može slati tradove
- 1-N s `Trade` (kao Receiver) — Korisnik može primati tradove
- 1-N s `Wishlist` — Korisnik ima više stavki na wish-listi

---

## Tablica: CardSet

**Opis:** Predstavlja set (izdanje) Pokemon karti

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `SetName` | `string` | Naziv seta (npr. "Base Set") |
| `ReleaseDate` | `DateTime` | Datum izdavanja |
| `TotalCards` | `int` | Ukupan broj karti u setu |
| `Publisher` | `string` | Izdavač (npr. "The Pokémon Company") |
| `SetSymbol` | `string` | Symbol seta |
| `SetCode` | `string` | Kod seta (npr. "BS") |

**Relacije:**
- 1-N s `PokemonCard` — Jedan set sadrži više karti

---

## Tablica: PokemonCard

**Opis:** Predstavlja pojedinu Pokemon karticu

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `CardName` | `string` | Naziv kartice |
| `PokemonNumber` | `int` | Pokédex broj |
| `Type` | `PokemonType` (enum) | Tip Pokemona (Water, Fire, Grass, itd.) |
| `Rarity` | `CardRarity` (enum) | Rijedakost (Common, Rare, UltraRare, itd.) |
| `MarketPrice` | `decimal` | Trenutna tržišna cijena |
| `CardSetId` | `int` | Strani ključ → `CardSet.Id` |
| `CreatedDate` | `DateTime` | Datum kreiranja kartice |

**Relacije:**
- N-1 s `CardSet` — Kartija pripada jednom setu
- 1-N s `CardInstance` — Kartija može imati više instanci
- 1-N s `Wishlist` — Kartija može biti na više wish-listi

---

## Tablica: Collection

**Opis:** Predstavlja korisnikovu kolekciju karti

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `UserId` | `int` | Strani ključ → `User.Id` |
| `CollectionName` | `string` | Naziv kolekcije |
| `CreatedDate` | `DateTime` | Datum kreiranja |
| `CollectionValue` | `decimal` | Ukupna vrijednost kolekcije |
| `Description` | `string` | Opis kolekcije |
| `IsPublic` | `bool` | Vidljiva li kolekcija javno |

**Relacije:**
- N-1 s `User` — Kolekcija pripada jednom korisniku
- 1-N s `CardInstance` — Kolekcija sadrži više instanci karti

---

## Tablica: CardInstance

**Opis:** Predstavlja konkretnu instancu kartice u kolekciji

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `CollectionId` | `int` | Strani ključ → `Collection.Id` |
| `PokemonCardId` | `int` | Strani ključ → `PokemonCard.Id` |
| `Condition` | `CardCondition` (enum) | Stanje kartice (Poor, Fair, Good, NearMint, Mint) |
| `Quantity` | `int` | Broj kopija ove kartice |
| `AcquisitionDate` | `DateTime` | Datum nabave |
| `CurrentValue` | `decimal` | Trenutna vrijednost |

**Relacije:**
- N-1 s `Collection` — Instanca pripada kolekciji (Cascade delete)
- N-1 s `PokemonCard` — Instanca je verzija specifične kartice
- 1-N s `Trade` — Instanca može biti stavka u tradeu

---

## Tablica: Trade

**Opis:** Predstavlja razmjenu karti između korisnika

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `SenderId` | `int` | Strani ključ → `User.Id` (pošiljač) |
| `ReceiverId` | `int` | Strani ključ → `User.Id` (primatelj) |
| `CardInstanceId` | `int` | Strani ključ → `CardInstance.Id` |
| `TradeDate` | `DateTime` | Datum tradea |
| `TransactionAmount` | `decimal` | Iznos transakcije |
| `TradeStatus` | `string` | Status (Pending, Completed, Cancelled) |

**Relacije:**
- N-1 s `User` (kao Sender) — Trade šalje jedan korisnik
- N-1 s `User` (kao Receiver) — Trade prima jedan korisnik
- N-1 s `CardInstance` — Trade sadrži jednu instancu kartice

---

## Tablica: Wishlist

**Opis:** Predstavlja stavku na korisnikovoj wish-listi

| Svojstvo | Tip | Opis |
| --- | --- | --- |
| `Id` | `int` | Primarni ključ |
| `UserId` | `int` | Strani ključ → `User.Id` |
| `PokemonCardId` | `int` | Strani ključ → `PokemonCard.Id` |
| `AddedDate` | `DateTime` | Datum dodavanja na listu |
| `Priority` | `int` | Prioritet (1 = najviši) |
| `MaxPrice` | `decimal` | Maksimalna cijena koju korisnik želi platiti |

**Relacije:**
- N-1 s `User` — Stavka pripada korisniku (Cascade delete)
- N-1 s `PokemonCard` — Stavka se odnosi na specifičnu karticu

---

## Dijagram Relacija (ER Diagram)

```
                            ┌─────────────┐
                            │    User     │
                            ├─────────────┤
                            │ Id (PK)     │
                            │ Username    │
                            │ Email       │
                            │ Budget      │
                            │ Phone       │
                            │ Address     │
                            │ Collections (nav) │
                            │ Wishlist (nav)    │
                            │ TradesSent (nav)  │
                            │ TradesReceived(nav)│
                            └─────────────┘

┌─────────────────────────┐           ┌──────────────┐
│      Collection         │           │   Wishlist   │
├─────────────────────────┤           ├──────────────┤
│ Id (PK)                 │           │ Id (PK)      │
│ UserId (FK)             │           │ UserId (FK)  │
│ CollectionName          │           │ PokemonCardId│
│ CreatedDate             │           │ AddedDate    │
│ CollectionValue         │           │ Priority     │
│ Description             │           │ MaxPrice     │
│ IsPublic                │           │ (nav: User)  │
│ (nav: User)             │           │ (nav: PokemonCard)
│ (nav: CardInstances)    │           └──────────────┘
└─────────────────────────┘

       ▲                             ▲
       │ 1..*                        │ 1..*
       │                             │
┌─────────────────────────┐    ┌─────────────────────────┐
│      CardInstance       │    │      PokemonCard        │
├─────────────────────────┤    ├─────────────────────────┤
│ Id (PK)                 │    │ Id (PK)                 │
│ CollectionId (FK)       │    │ CardName                │
│ (nav: Collection)       │    │ PokemonNumber           │
│ PokemonCardId (FK)      │    │ Type                    │
│ (nav: PokemonCard)      │    │ Rarity                  │
│ Condition               │    │ MarketPrice             │
│ Quantity                │    │ CardSetId (FK)          │
│ AcquisitionDate         │    │ (nav: CardSet)          │
│ CurrentValue            │    │ (nav: CardInstances)    │
│ (nav: Trades)           │    │ CreatedDate             │
└─────────────────────────┘    └─────────────────────────┘

           ▲                         ▲
           │                         │
          N-1                        N-1
           │                         │
      ┌──────────────┐           ┌──────────────┐
      │    Trade     │           │   CardSet    │
      ├──────────────┤           ├──────────────┤
      │ Id (PK)      │           │ Id (PK)      │
      │ SenderId (FK)│           │ SetName      │
      │ (nav: Sender)│           │ ReleaseDate  │
      │ ReceiverId(FK)│          │ TotalCards   │
      │ (nav: Receiver)│         │ Publisher    │
      │ CardInstanceId(FK)│      │ SetSymbol    │
      │ (nav: CardInstance)│     │ SetCode      │
      │ TradeDate     │          └──────────────┘
      │ TransactionAmount│
      │ TradeStatus   │
      └──────────────┘

``` 

---

## Delete Behavior (Kaskadne Brisanje)

| Relacija | Delete Behavior | Razlog |
| --- | --- | --- |
| Collection → CardInstance | **Cascade** | Brisanjem kolekcije brišu se njene kartice |
| CardInstance → Trade | **Restrict** | Ako je kartija u tradeu, ne može se obrisati |
| PokemonCard → CardInstance | **Restrict** | Originalna kartija se ne briše s instancama |
| Wishlist → User | **Cascade** | Brisanjem korisnika brišu se jeho wish-liste |
| Trade.Sender / Receiver | **Restrict** | Ako korisnik ima tradove, ne može se obrisati |

---

## Enumeracije (Enums)

### CardRarity
```csharp
Common, Uncommon, Rare, UltraRare, SecretRare, Promo
```

### CardCondition
```csharp
Poor, Fair, Good, VeryGood, Excellent, NearMint, Mint
```

### PokemonType
```csharp
Colorless, Fire, Water, Electric, Grass, Fighting, 
Psychic, Dragon, Dark, Steel, Fairy
```

---

## Primjeri Upita (Example Queries)

### Sve kartice u kolekciji korisnika

```sql
SELECT ci.*, pc.CardName, pc.Type, pc.Rarity
FROM CardInstance ci
JOIN PokemonCard pc ON ci.PokemonCardId = pc.Id
WHERE ci.CollectionId = @collectionId
```

### Korisnikova wish-lista s cijenama

```sql
SELECT w.*, pc.CardName, pc.MarketPrice
FROM Wishlist w
JOIN PokemonCard pc ON w.PokemonCardId = pc.Id
WHERE w.UserId = @userId
ORDER BY w.Priority
```

### Nedavni tradei korisnika

```sql
SELECT t.*, 
       u1.Username AS SenderName, 
       u2.Username AS ReceiverName,
       ci.Condition
FROM Trade t
JOIN [User] u1 ON t.SenderId = u1.Id
JOIN [User] u2 ON t.ReceiverId = u2.Id
JOIN CardInstance ci ON t.CardInstanceId = ci.Id
WHERE t.SenderId = @userId OR t.ReceiverId = @userId
ORDER BY t.TradeDate DESC
```

---

## Zaključak

Semantički model omogućava fleksibilnu upravljanje Pokemon kartama sa:
- ✅ Kolekcijama po korisniku
- ✅ Tradenjem između korisnika
- ✅ Wish-listom s prioritetima
- ✅ Praćenjem stanja kartice i vrijednosti
- ✅ Organizacijom po setovima i tipovima
