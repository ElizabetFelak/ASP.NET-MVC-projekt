using System;
using System.Collections.Generic;
using System.Linq;
using PokemonCollectorApp.Model;

namespace PokemonCollectorApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("=== Pokemon Card Collector Application ===\n");

            // Kreiranje podataka
            var users = InitializeUsers();
            var cardSets = InitializeCardSets();
            var pokemonCards = InitializePokemonCards(cardSets);
            var collections = InitializeCollections(users, pokemonCards);
            var trades = InitializeTrades(users);
            var wishlists = InitializeWishlists(users, pokemonCards);

            WireObjectGraph(users, cardSets, pokemonCards, collections, trades, wishlists);

            // Ispis osnovnih podataka
            System.Console.WriteLine("--- KORISNICI ---");
            DisplayUsers(users);

            System.Console.WriteLine("\n--- KOLEKCIJE ---");
            DisplayCollections(collections);

            System.Console.WriteLine("\n--- POKEMON KARTICE ---");
            DisplayPokemonCards(pokemonCards);

            System.Console.WriteLine("\n--- TRADE ZAPISI ---");
            DisplayTrades(trades);

            System.Console.WriteLine("\n--- WISHLIST ---");
            DisplayWishlists(wishlists);

            // LINQ Upiti
            System.Console.WriteLine("\n\n=== LINQ UPITI ===\n");

            // Upit 1: Javne kolekcije poredane po vrijednosti
            System.Console.WriteLine("UPIT 1: Javne kolekcije poredane po vrijednosti:");
            var publicCollections = collections
                .Where(c => c.IsPublic)
                .OrderByDescending(c => c.CollectionValue)
                .ToList();
            foreach (var collection in publicCollections)
            {
                System.Console.WriteLine($"  - {collection.CollectionName} | vlasnik: {collection.User.Username} | vrijednost: {collection.CollectionValue}€");
            }

            // Upit 2: Kartice koje si korisnik može priuštiti
            var mainUser = users.First(u => u.Username == "PokemonMaster");
            System.Console.WriteLine($"\nUPIT 2: Kartice koje može kupiti {mainUser.Username} (budžet {mainUser.Budget}€):");
            var affordableCards = pokemonCards
                .Where(p => p.MarketPrice <= mainUser.Budget)
                .OrderBy(p => p.MarketPrice)
                .ToList();
            foreach (var card in affordableCards)
            {
                System.Console.WriteLine($"  - {card.CardName} | {card.MarketPrice}€ | {card.Rarity}");
            }

            // Upit 3: Rijetke kartice sortirane po cijeni
            System.Console.WriteLine("\nUPIT 3: Rijetke kartice sortirane po cijeni:");
            var rareCards = pokemonCards
                .Where(p => p.Rarity == CardRarity.Rare || 
                           p.Rarity == CardRarity.VeryRare ||
                           p.Rarity == CardRarity.UltraRare ||
                           p.Rarity == CardRarity.SecretRare ||
                           p.Rarity == CardRarity.Promo)
                .OrderByDescending(p => p.MarketPrice)
                .ToList();
            foreach (var card in rareCards)
            {
                System.Console.WriteLine($"  - {card.CardName} | {card.MarketPrice}€ | {card.Rarity} | set: {card.CardSet.SetName}");
            }

            // Upit 4: Najtraženije kartice iz wishlisti
            System.Console.WriteLine("\nUPIT 4: Najtraženije kartice iz wishlisti:");
            var mostWantedCards = wishlists
                .GroupBy(w => w.PokemonCard)
                .Select(g => new
                {
                    CardName = g.Key.CardName,
                    SetName = g.Key.CardSet.SetName,
                    WantCount = g.Count(),
                    MaxPrice = g.Max(w => w.MaxPrice)
                })
                .OrderByDescending(x => x.WantCount)
                .ToList();
            foreach (var item in mostWantedCards)
            {
                System.Console.WriteLine($"  - {item.CardName} ({item.SetName}) | želi je {item.WantCount} korisnika | max cijena: {item.MaxPrice}€");
            }

            // Upit 5: Statistika po korisniku
            System.Console.WriteLine("\nUPIT 5: Ukupna vrijednost kolekcija po korisniku:");
            var userCollectionStats = users
                .Select(u => new
                {
                    u.Username,
                    CollectionCount = u.Collections.Count,
                    TotalValue = u.Collections.Sum(c => c.CollectionValue),
                    CardCount = u.Collections.SelectMany(c => c.CardInstances).Count()
                })
                .OrderByDescending(x => x.TotalValue)
                .ToList();
            foreach (var item in userCollectionStats)
            {
                System.Console.WriteLine($"  - {item.Username} | kolekcije: {item.CollectionCount} | karata: {item.CardCount} | ukupno: {item.TotalValue}€");
            }

            // Upit 6: Prosjek cijene po tipu kartice
            System.Console.WriteLine("\nUPIT 6: Prosječna cijena po tipu kartice:");
            var avgPriceByType = pokemonCards
                .GroupBy(p => p.Type)
                .Select(g => new { Type = g.Key, AvgPrice = g.Average(p => p.MarketPrice) })
                .OrderByDescending(x => x.AvgPrice)
                .ToList();
            foreach (var item in avgPriceByType)
            {
                System.Console.WriteLine($"  - {item.Type}: {item.AvgPrice:F2}€");
            }

            // Upit 7: Top 3 najskuplje kartice koje su i u wishlistama
            System.Console.WriteLine("\nUPIT 7: Top 3 najskuplje kartice iz wishlisti:");
            var topExpensiveCards = pokemonCards
                .Where(card => wishlists.Any(w => w.PokemonCardId == card.Id))
                .OrderByDescending(p => p.MarketPrice)
                .Take(3)
                .ToList();
            foreach (var card in topExpensiveCards)
            {
                System.Console.WriteLine($"  - {card.CardName} | {card.MarketPrice}€ | željena u kolekciji");
            }

            // Upit 8: Kartice koje nedostaju u određenoj kolekciji
            System.Console.WriteLine("\nUPIT 8: Kartice koje nedostaju u kolekciji 'Budget Collector's Corner':");
            var targetCollection = collections.First(c => c.CollectionName == "Budget Collector's Corner");
            var missingCards = pokemonCards
                .Where(card => !targetCollection.CardInstances.Any(ci => ci.PokemonCardId == card.Id))
                .OrderByDescending(card => card.MarketPrice)
                .ToList();
            foreach (var card in missingCards.Take(5))
            {
                System.Console.WriteLine($"  - {card.CardName} | {card.CardSet.SetName} | {card.MarketPrice}€");
            }

            // Upit 9: Usporedba setova prema broju rijetkih kartica
            System.Console.WriteLine("\nUPIT 9: Setovi rangirani po broju rijetkih kartica:");
            var setStats = cardSets
                .Select(set => new
                {
                    set.SetName,
                    TotalCards = set.Cards.Count,
                    RareCards = set.Cards.Count(card => card.Rarity == CardRarity.Rare || card.Rarity == CardRarity.VeryRare || card.Rarity == CardRarity.UltraRare || card.Rarity == CardRarity.SecretRare)
                })
                .OrderByDescending(x => x.RareCards)
                .ThenByDescending(x => x.TotalCards)
                .ToList();
            foreach (var item in setStats)
            {
                System.Console.WriteLine($"  - {item.SetName} | ukupno: {item.TotalCards} | rijetke: {item.RareCards}");
            }

            // Upit 10: Najveća kolekcija po vrijednosti
            System.Console.WriteLine("\nUPIT 10: Korisnik s najvećom kolekcijom po vrijednosti:");
            var topCollector = collections
                .OrderByDescending(c => c.CollectionValue)
                .FirstOrDefault();
            if (topCollector != null)
            {
                System.Console.WriteLine($"  - {topCollector.User.Username}: {topCollector.CollectionValue}€ ({topCollector.CardInstances.Count} karata)");
            }

            System.Console.ReadKey();
        }

        static List<User> InitializeUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "PokemonMaster",
                    Email = "master@pokemon.com",
                    RegistrationDate = new DateTime(2020, 1, 15),
                    Budget = 5000m,
                    PhoneNumber = "+385-1-1234567",
                    Address = "Zagreb, Hrvatska"
                },
                new User
                {
                    Id = 2,
                    Username = "CardCollector92",
                    Email = "collector@example.com",
                    RegistrationDate = new DateTime(2021, 6, 22),
                    Budget = 2500m,
                    PhoneNumber = "+385-1-2345678",
                    Address = "Split, Hrvatska"
                },
                new User
                {
                    Id = 3,
                    Username = "RareFinder",
                    Email = "rarefindr@email.com",
                    RegistrationDate = new DateTime(2022, 3, 10),
                    Budget = 1500m,
                    PhoneNumber = "+385-1-3456789",
                    Address = "Rijeka, Hrvatska"
                }
            };

            users[0].Collections = new List<Collection>();
            users[1].Collections = new List<Collection>();
            users[2].Collections = new List<Collection>();

            return users;
        }

        static List<CardSet> InitializeCardSets()
        {
            return new List<CardSet>
            {
                new CardSet
                {
                    Id = 1,
                    SetName = "Base Set",
                    ReleaseDate = new DateTime(1999, 1, 9),
                    TotalCards = 102,
                    Publisher = "The Pokémon Company International",
                    SetSymbol = "Circle",
                    SetCode = "RS"
                },
                new CardSet
                {
                    Id = 2,
                    SetName = "Jungle",
                    ReleaseDate = new DateTime(1999, 6, 16),
                    TotalCards = 64,
                    Publisher = "The Pokémon Company International",
                    SetSymbol = "Leaf",
                    SetCode = "JU"
                },
                new CardSet
                {
                    Id = 3,
                    SetName = "Fossil",
                    ReleaseDate = new DateTime(1999, 10, 10),
                    TotalCards = 62,
                    Publisher = "The Pokémon Company International",
                    SetSymbol = "Skull",
                    SetCode = "FO"
                }
            };
        }

        static List<PokemonCard> InitializePokemonCards(List<CardSet> sets)
        {
            var cards = new List<PokemonCard>
            {
                // Base Set kartice
                new PokemonCard
                {
                    Id = 1,
                    CardName = "Charizard",
                    PokemonNumber = 6,
                    Type = PokemonType.Fire,
                    Rarity = CardRarity.UltraRare,
                    MarketPrice = 15000m,
                    CardSetId = 1,
                    CreatedDate = new DateTime(1999, 1, 15),
                    CardSet = sets[0]
                },
                new PokemonCard
                {
                    Id = 2,
                    CardName = "Blastoise",
                    PokemonNumber = 9,
                    Type = PokemonType.Water,
                    Rarity = CardRarity.Rare,
                    MarketPrice = 8500m,
                    CardSetId = 1,
                    CreatedDate = new DateTime(1999, 1, 15),
                    CardSet = sets[0]
                },
                new PokemonCard
                {
                    Id = 3,
                    CardName = "Venusaur",
                    PokemonNumber = 3,
                    Type = PokemonType.Grass,
                    Rarity = CardRarity.Rare,
                    MarketPrice = 7200m,
                    CardSetId = 1,
                    CreatedDate = new DateTime(1999, 1, 15),
                    CardSet = sets[0]
                },
                new PokemonCard
                {
                    Id = 4,
                    CardName = "Pikachu",
                    PokemonNumber = 25,
                    Type = PokemonType.Electric,
                    Rarity = CardRarity.Uncommon,
                    MarketPrice = 2500m,
                    CardSetId = 1,
                    CreatedDate = new DateTime(1999, 1, 15),
                    CardSet = sets[0]
                },
                // Jungle kartice
                new PokemonCard
                {
                    Id = 5,
                    CardName = "Dragonite",
                    PokemonNumber = 149,
                    Type = PokemonType.Dragon,
                    Rarity = CardRarity.VeryRare,
                    MarketPrice = 5500m,
                    CardSetId = 2,
                    CreatedDate = new DateTime(1999, 6, 20),
                    CardSet = sets[1]
                },
                new PokemonCard
                {
                    Id = 6,
                    CardName = "Gengar",
                    PokemonNumber = 94,
                    Type = PokemonType.Ghost,
                    Rarity = CardRarity.Rare,
                    MarketPrice = 4800m,
                    CardSetId = 2,
                    CreatedDate = new DateTime(1999, 6, 20),
                    CardSet = sets[1]
                },
                new PokemonCard
                {
                    Id = 7,
                    CardName = "Machamp",
                    PokemonNumber = 68,
                    Type = PokemonType.Fighting,
                    Rarity = CardRarity.Rare,
                    MarketPrice = 3200m,
                    CardSetId = 2,
                    CreatedDate = new DateTime(1999, 6, 20),
                    CardSet = sets[1]
                },
                // Fossil kartice
                new PokemonCard
                {
                    Id = 8,
                    CardName = "Aerodactyl",
                    PokemonNumber = 142,
                    Type = PokemonType.Rock,
                    Rarity = CardRarity.VeryRare,
                    MarketPrice = 6200m,
                    CardSetId = 3,
                    CreatedDate = new DateTime(1999, 10, 15),
                    CardSet = sets[2]
                }
            };

            return cards;
        }

        static List<Collection> InitializeCollections(List<User> users, List<PokemonCard> cards)
        {
            var collections = new List<Collection>();

            // Kolekcija 1
            var collection1 = new Collection
            {
                Id = 1,
                UserId = 1,
                CollectionName = "First Generation Classics",
                CreatedDate = new DateTime(2020, 2, 1),
                Description = "Moja kolekcija originalnih Pokemon karata iz prve generacije",
                IsPublic = true,
                User = users[0],
                CardInstances = new List<CardInstance>()
            };

            collection1.CardInstances.Add(new CardInstance
            {
                Id = 1,
                CollectionId = 1,
                PokemonCardId = 1,
                Condition = CardCondition.VeryGood,
                Quantity = 1,
                AcquisitionDate = new DateTime(2020, 3, 15),
                CurrentValue = 14500m,
                Collection = collection1,
                PokemonCard = cards[0]
            });

            collection1.CardInstances.Add(new CardInstance
            {
                Id = 2,
                CollectionId = 1,
                PokemonCardId = 2,
                Condition = CardCondition.Excellent,
                Quantity = 1,
                AcquisitionDate = new DateTime(2020, 5, 20),
                CurrentValue = 8200m,
                Collection = collection1,
                PokemonCard = cards[1]
            });

            collection1.CardInstances.Add(new CardInstance
            {
                Id = 3,
                CollectionId = 1,
                PokemonCardId = 4,
                Condition = CardCondition.Good,
                Quantity = 2,
                AcquisitionDate = new DateTime(2020, 4, 10),
                CurrentValue = 4800m,
                Collection = collection1,
                PokemonCard = cards[3]
            });

            collection1.CollectionValue = collection1.CardInstances.Sum(ci => ci.CurrentValue);

            // Kolekcija 2
            var collection2 = new Collection
            {
                Id = 2,
                UserId = 2,
                CollectionName = "Rare Dragons and Spooks",
                CreatedDate = new DateTime(2021, 7, 1),
                Description = "Specijalizirana kolekcija rijetkih zmajeva i duhova",
                IsPublic = true,
                User = users[1],
                CardInstances = new List<CardInstance>()
            };

            collection2.CardInstances.Add(new CardInstance
            {
                Id = 4,
                CollectionId = 2,
                PokemonCardId = 5,
                Condition = CardCondition.Excellent,
                Quantity = 1,
                AcquisitionDate = new DateTime(2021, 8, 5),
                CurrentValue = 5300m,
                Collection = collection2,
                PokemonCard = cards[4]
            });

            collection2.CardInstances.Add(new CardInstance
            {
                Id = 5,
                CollectionId = 2,
                PokemonCardId = 6,
                Condition = CardCondition.VeryGood,
                Quantity = 1,
                AcquisitionDate = new DateTime(2021, 9, 12),
                CurrentValue = 4600m,
                Collection = collection2,
                PokemonCard = cards[5]
            });

            collection2.CardInstances.Add(new CardInstance
            {
                Id = 6,
                CollectionId = 2,
                PokemonCardId = 8,
                Condition = CardCondition.Good,
                Quantity = 1,
                AcquisitionDate = new DateTime(2021, 10, 3),
                CurrentValue = 6000m,
                Collection = collection2,
                PokemonCard = cards[7]
            });

            collection2.CardInstances.Add(new CardInstance
            {
                Id = 7,
                CollectionId = 2,
                PokemonCardId = 3,
                Condition = CardCondition.Fair,
                Quantity = 1,
                AcquisitionDate = new DateTime(2021, 11, 15),
                CurrentValue = 6800m,
                Collection = collection2,
                PokemonCard = cards[2]
            });

            collection2.CollectionValue = collection2.CardInstances.Sum(ci => ci.CurrentValue);

            // Kolekcija 3
            var collection3 = new Collection
            {
                Id = 3,
                UserId = 3,
                CollectionName = "Budget Collector's Corner",
                CreatedDate = new DateTime(2022, 4, 1),
                Description = "Kolekcija dostupnijih karata za početnike",
                IsPublic = false,
                User = users[2],
                CardInstances = new List<CardInstance>()
            };

            collection3.CardInstances.Add(new CardInstance
            {
                Id = 8,
                CollectionId = 3,
                PokemonCardId = 7,
                Condition = CardCondition.Good,
                Quantity = 2,
                AcquisitionDate = new DateTime(2022, 5, 20),
                CurrentValue = 6200m,
                Collection = collection3,
                PokemonCard = cards[6]
            });

            collection3.CardInstances.Add(new CardInstance
            {
                Id = 9,
                CollectionId = 3,
                PokemonCardId = 4,
                Condition = CardCondition.Excellent,
                Quantity = 1,
                AcquisitionDate = new DateTime(2022, 6, 10),
                CurrentValue = 2400m,
                Collection = collection3,
                PokemonCard = cards[3]
            });

            collection3.CollectionValue = collection3.CardInstances.Sum(ci => ci.CurrentValue);

            collections.Add(collection1);
            collections.Add(collection2);
            collections.Add(collection3);

            return collections;
        }

        static List<Trade> InitializeTrades(List<User> users)
        {
            return new List<Trade>
            {
                new Trade
                {
                    Id = 1,
                    SenderId = 1,
                    ReceiverId = 2,
                    CardInstanceId = 1,
                    TradeDate = new DateTime(2023, 1, 15),
                    TransactionAmount = 1500m,
                    TradeStatus = "Completed"
                },
                new Trade
                {
                    Id = 2,
                    SenderId = 2,
                    ReceiverId = 3,
                    CardInstanceId = 5,
                    TradeDate = new DateTime(2023, 2, 20),
                    TransactionAmount = 2000m,
                    TradeStatus = "Completed"
                }
            };
        }

        static List<Wishlist> InitializeWishlists(List<User> users, List<PokemonCard> cards)
        {
            return new List<Wishlist>
            {
                new Wishlist
                {
                    Id = 1,
                    UserId = 1,
                    PokemonCardId = 8,
                    AddedDate = new DateTime(2023, 3, 1),
                    Priority = 1,
                    MaxPrice = 7000m
                },
                new Wishlist
                {
                    Id = 2,
                    UserId = 2,
                    PokemonCardId = 1,
                    AddedDate = new DateTime(2023, 4, 12),
                    Priority = 2,
                    MaxPrice = 16000m
                },
                new Wishlist
                {
                    Id = 3,
                    UserId = 3,
                    PokemonCardId = 5,
                    AddedDate = new DateTime(2023, 5, 7),
                    Priority = 3,
                    MaxPrice = 6000m
                }
            };
        }

        static void WireObjectGraph(
            List<User> users,
            List<CardSet> cardSets,
            List<PokemonCard> pokemonCards,
            List<Collection> collections,
            List<Trade> trades,
            List<Wishlist> wishlists)
        {
            foreach (var set in cardSets)
            {
                set.Cards.Clear();
            }

            foreach (var card in pokemonCards)
            {
                card.CardInstances.Clear();
                card.CardSet?.Cards.Add(card);
            }

            foreach (var collection in collections)
            {
                collection.User?.Collections.Add(collection);

                foreach (var cardInstance in collection.CardInstances)
                {
                    cardInstance.Collection = collection;
                    cardInstance.PokemonCard.CardInstances.Add(cardInstance);
                }
            }

            foreach (var trade in trades)
            {
                trade.Sender = users.First(u => u.Id == trade.SenderId);
                trade.Receiver = users.First(u => u.Id == trade.ReceiverId);
                trade.CardInstance = collections
                    .SelectMany(c => c.CardInstances)
                    .FirstOrDefault(ci => ci.Id == trade.CardInstanceId)
                    ?? collections.SelectMany(c => c.CardInstances).First();
            }

            foreach (var wishlist in wishlists)
            {
                wishlist.User = users.First(u => u.Id == wishlist.UserId);
                wishlist.PokemonCard = pokemonCards.First(c => c.Id == wishlist.PokemonCardId);
            }
        }

        static void DisplayUsers(List<User> users)
        {
            foreach (var user in users)
            {
                System.Console.WriteLine($"ID: {user.Id}");
                System.Console.WriteLine($"  Korisničko ime: {user.Username}");
                System.Console.WriteLine($"  Email: {user.Email}");
                System.Console.WriteLine($"  Registracija: {user.RegistrationDate:dd.MM.yyyy}");
                System.Console.WriteLine($"  Budžet: {user.Budget}€");
                System.Console.WriteLine($"  Adresa: {user.Address}");
                System.Console.WriteLine();
            }
        }

        static void DisplayCollections(List<Collection> collections)
        {
            foreach (var collection in collections)
            {
                System.Console.WriteLine($"ID: {collection.Id}");
                System.Console.WriteLine($"  Naziv: {collection.CollectionName}");
                System.Console.WriteLine($"  Vlasnik: {collection.User.Username}");
                System.Console.WriteLine($"  Broj karata: {collection.CardInstances.Count}");
                System.Console.WriteLine($"  Vrijednost: {collection.CollectionValue}€");
                System.Console.WriteLine($"  Javna: {(collection.IsPublic ? "Da" : "Ne")}");
                System.Console.WriteLine();
            }
        }

        static void DisplayTrades(List<Trade> trades)
        {
            foreach (var trade in trades)
            {
                System.Console.WriteLine($"ID: {trade.Id}");
                System.Console.WriteLine($"  Pošiljatelj: {trade.Sender.Username}");
                System.Console.WriteLine($"  Primatelj: {trade.Receiver.Username}");
                System.Console.WriteLine($"  Kartica: {trade.CardInstance.PokemonCard.CardName}");
                System.Console.WriteLine($"  Datum: {trade.TradeDate:dd.MM.yyyy}");
                System.Console.WriteLine($"  Iznos: {trade.TransactionAmount}€");
                System.Console.WriteLine($"  Status: {trade.TradeStatus}");
                System.Console.WriteLine();
            }
        }

        static void DisplayWishlists(List<Wishlist> wishlists)
        {
            foreach (var wishlist in wishlists)
            {
                System.Console.WriteLine($"ID: {wishlist.Id}");
                System.Console.WriteLine($"  Korisnik: {wishlist.User.Username}");
                System.Console.WriteLine($"  Željena kartica: {wishlist.PokemonCard.CardName}");
                System.Console.WriteLine($"  Datum dodavanja: {wishlist.AddedDate:dd.MM.yyyy}");
                System.Console.WriteLine($"  Prioritet: {wishlist.Priority}");
                System.Console.WriteLine($"  Max cijena: {wishlist.MaxPrice}€");
                System.Console.WriteLine();
            }
        }

        static void DisplayPokemonCards(List<PokemonCard> cards)
        {
            foreach (var card in cards.OrderBy(c => c.CardName))
            {
                System.Console.WriteLine($"ID: {card.Id}");
                System.Console.WriteLine($"  Naziv: {card.CardName} (#{card.PokemonNumber})");
                System.Console.WriteLine($"  Tip: {card.Type}");
                System.Console.WriteLine($"  Redakšće: {card.Rarity}");
                System.Console.WriteLine($"  Set: {card.CardSet.SetName}");
                System.Console.WriteLine($"  Cijenja: {card.MarketPrice}€");
                System.Console.WriteLine();
            }
        }
    }
}
