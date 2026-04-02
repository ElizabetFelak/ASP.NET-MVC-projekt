# Objektni Model: Aplikacija za Praćenje Kolekcija Pokemon Karata

## Pregled Strukture

Ovaj projekt implementira aplikaciju za praćenje i upravljanje Personal Pokemon kolekcijama. Aplikacija omogućava korisnicima da vode evidenciju svojih karata, prate njihovu vrijednost i mogu komunicirati s drugim kolekcionarima.

---

## Klase (7 klasa)

### 1. **User** (Kompleksna - 7 svojstava)
Predstavlja korisnika aplikacije.

```csharp
public class User
{
    public int Id { get; set; }                          // ID korisnika
    public string Username { get; set; }                 // Korisničko ime
    public string Email { get; set; }                    // Email adresa
    public DateTime RegistrationDate { get; set; }       // DateTime: Datum registracije
    public decimal Budget { get; set; }                  // Budžet za kupovinu karata
    public string PhoneNumber { get; set; }              // Telefonski broj
    public string Address { get; set; }                  // Fizička adresa
    public List<Collection> Collections { get; set; }    // 1-N: Kolekcije korisnika
}
```

**Karakteristike:**
- Sadrži 7 svojstava (4 osnovnu + 3 dodatna)
- DateTime svojstvo: `RegistrationDate`
- Relacija 1-N s `Collection` klasom

---

### 2. **CardSet** (Kompleksna - 7 svojstava)
Predstavlja set/kolekciju izdanih Pokemon karata.

```csharp
public class CardSet
{
    public int Id { get; set; }                          // ID seta
    public string SetName { get; set; }                  // Naziv seta (npr. "Base Set")
    public DateTime ReleaseDate { get; set; }            // DateTime: Datum izdanja
    public int TotalCards { get; set; }                  // Ukupan broj karata u setu
    public string Publisher { get; set; }                // Izdavač
    public string SetSymbol { get; set; }                // Simbol seta
    public string SetCode { get; set; }                  // Šifra seta
    public List<PokemonCard> Cards { get; set; }         // 1-N: Kartice u setu
}
```

**Karakteristike:**
- Sadrži 7 svojstava
- DateTime svojstvo: `ReleaseDate`
- Relacija 1-N s `PokemonCard` klasom

---

### 3. **PokemonCard** (Kompleksna - 8 svojstava)
Predstavlja pojedinačnu Pokemon karticu.

```csharp
public class PokemonCard
{
    public int Id { get; set; }                          // ID kartice
    public string CardName { get; set; }                 // Naziv kartice
    public int PokemonNumber { get; set; }               // Broj Pokemona
    public PokemonType Type { get; set; }                // Enum: Tip Pokemona
    public CardRarity Rarity { get; set; }               // Enum: Redakšće
    public decimal MarketPrice { get; set; }             // Tržišna vrijednost
    public int CardSetId { get; set; }                   // ID seta kojem pripada
    public DateTime CreatedDate { get; set; }            // DateTime: Datum kreiranja
    public CardSet CardSet { get; set; }                 // Veza s CardSet
    public List<CardInstance> CardInstances { get; set; } // 1-N: Instance kartice
}
```

**Karakteristike:**
- Sadrži 8 svojstava (najviše!)
- DateTime svojstvo: `CreatedDate`
- Enum svojstva: `Type` (PokemonType), `Rarity` (CardRarity)
- Relacija 1-N s `CardInstance` klasom

---

### 4. **Collection** (Kompleksna - 7 svojstava)
Predstavlja korisničku kolekciju karata.

```csharp
public class Collection
{
    public int Id { get; set; }                          // ID kolekcije
    public int UserId { get; set; }                      // ID vlasnika (FK)
    public string CollectionName { get; set; }           // Naziv kolekcije
    public DateTime CreatedDate { get; set; }            // DateTime: Datum kreiranja
    public decimal CollectionValue { get; set; }         // Ukupna vrijednost
    public string Description { get; set; }              // Opis kolekcije
    public bool IsPublic { get; set; }                   // Je li javna
    public User User { get; set; }                       // Veza s User
    public List<CardInstance> CardInstances { get; set; } // N-N: Kartice u kolekciji
}
```

**Karakteristike:**
- Sadrži 7 svojstava
- DateTime svojstvo: `CreatedDate`
- Relacija 1-N s `User` klasom
- Relacija N-N s `PokemonCard` klasom kroz `CardInstance`

---

### 5. **CardInstance** (N-N relacija)
Predstavlja instancu kartice u nekoj kolekciji (omogućava više ista kartice).

```csharp
public class CardInstance
{
    public int Id { get; set; }                          // ID instance
    public int CollectionId { get; set; }                // FK: ID kolekcije
    public int PokemonCardId { get; set; }               // FK: ID kartice
    public CardCondition Condition { get; set; }         // Enum: Stanje kartice
    public int Quantity { get; set; }                    // Količina istih karata
    public DateTime AcquisitionDate { get; set; }        // DateTime: Datum nabavke
    public decimal CurrentValue { get; set; }            // Trenutna vrijednost
    public Collection Collection { get; set; }           // Veza s Collection
    public PokemonCard PokemonCard { get; set; }         // Veza s PokemonCard
}
```

**Karakteristike:**
- N-N relacija između `Collection` i `PokemonCard`
- DateTime svojstvo: `AcquisitionDate`
- Enum svojstvo: `Condition` (CardCondition)

---

### 6. **Trade** (Transakcija)
Predstavlja transakciju/handel između korisnika.

```csharp
public class Trade
{
    public int Id { get; set; }                          // ID transakcije
    public int SenderId { get; set; }                    // FK: Pošiljatelj
    public int ReceiverId { get; set; }                  // FK: Primatelj
    public int CardInstanceId { get; set; }              // FK: Kartica koja se traži
    public DateTime TradeDate { get; set; }              // DateTime: Datum transakcije
    public decimal TransactionAmount { get; set; }       // Iznos transakcije
    public string TradeStatus { get; set; }              // Status ("Completed", "Pending")
    public User Sender { get; set; }                     // Veza - Pošiljatelj
    public User Receiver { get; set; }                   // Veza - Primatelj
    public CardInstance CardInstance { get; set; }       // Veza - Kartica
}
```

**Karakteristike:**
- DateTime svojstvo: `TradeDate`
- Veze s `User` (dva foreign key-a)

---

### 7. **Wishlist**
Predstavlja listu željenih karata.

```csharp
public class Wishlist
{
    public int Id { get; set; }                          // ID stavke
    public int UserId { get; set; }                      // FK: ID korisnika
    public int PokemonCardId { get; set; }               // FK: ID željene kartice
    public DateTime AddedDate { get; set; }              // DateTime: Datum dodavanja
    public int Priority { get; set; }                    // Prioritet (1-10)
    public decimal MaxPrice { get; set; }                // Maksimalna cijena
    public User User { get; set; }                       // Veza s User
    public PokemonCard PokemonCard { get; set; }         // Veza s PokemonCard
}
```

---

## Enumi (3 enum-a)

### 1. **PokemonType** (vlastiti enum)
Predstavlja tipu Pokemona.
```csharp
public enum PokemonType
{
    Normal, Fire, Water, Electric, Grass,
    Fighting, Psychic,
    Dragon, Dark, Steel, Fairy
}
```

### 2. **CardRarity** (vlastiti enum)
Predstavlja redakšće kartice.
```csharp
public enum CardRarity
{
    Common, Uncommon, Rare, UltraRare, SecretRare
}
```

### 3. **CardCondition** (vlastiti enum)
Predstavlja stanje kartice.
```csharp
public enum CardCondition
{
    Poor, Fair, Good, VeryGood, Excellent, Mint
}
```

---

## Relacije

### 1-N Relacije:
- **User** → **Collection** (jedna kolekcija mora pripadati korisniku)
- **CardSet** → **PokemonCard** (jedna kartica pripada samo jednom setu)
- **PokemonCard** → **CardInstance** (jedna kartica može imati više instanci)

### N-N Relacije:
- **Collection** ↔ **PokemonCard** (kroz **CardInstance**)
  - Omogućava da ista kartica bude u više kolekcija
  - Omogućava da jedna kolekcija sadrži više karata

---

## Test Podaci

Aplikacija je popunjena s:
- **3 korisnika** (PokemonMaster, CardCollector92, RareFinder)
- **3 seta karata** (Base Set, Jungle, Fossil)
- **8 Pokemon karata** raznih tipova i redakšća
- **3 kolekcije** s razičitim kartama

---

## LINQ Upiti Uključeni

Program demonstrira sljedeće LINQ operacije:

1. **Where** - Filtriranje karata po tipu
2. **OrderBy/OrderByDescending** - Sortiranje
3. **Take** - Ograničenje broja rezultata
4. **GroupBy** - Grupiranjeponedjelnika
5. **Select** - Projekcija
6. **FirstOrDefault** - Pronalaženje prvog elementa
7. **Sum/Average** - Agregatne funkcije
8. **ToList** - Konverzija u listu

---

## Sažetak

✅ **7 klasa** - User, CardSet, PokemonCard, Collection, CardInstance, Trade, Wishlist
✅ **4 kompleksne klase** - User (7), CardSet (7), PokemonCard (8), Collection (7)
✅ **3 enuma** - PokemonType, CardRarity, CardCondition
✅ **DateTime svojstva** - RegistrationDate, ReleaseDate, CreatedDate, AcquisitionDate, TradeDate, AddedDate
✅ **1-N relacije** - User→Collection, CardSet→PokemonCard, PokemonCard→CardInstance
✅ **N-N relacije** - Collection↔PokemonCard (kroz CardInstance)
✅ **3+ glavna objekta** - 3 korisnika s detaljno popunjenim podacima
✅ **Smislena svojstva** - Sva svojstva su relevantna za aplikaciju
