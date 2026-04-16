using System;
using System.Collections.Generic;
using System.Linq;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.Data;

public static class MockDataFactory
{
    public static (List<User> Users, List<CardSet> CardSets, List<PokemonCard> PokemonCards, List<Collection> Collections, List<CardInstance> CardInstances, List<Trade> Trades, List<Wishlist> Wishlists) Build()
    {
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "PokemonMaster",
                Email = "master@pokemon.com",
                RegistrationDate = new DateTime(2020, 1, 15),
                Budget = 5000m,
                PhoneNumber = "+385-1-1234567",
                Address = "Zagreb, Hrvatska"
            },
            new()
            {
                Id = 2,
                Username = "CardCollector92",
                Email = "collector@example.com",
                RegistrationDate = new DateTime(2021, 6, 22),
                Budget = 2500m,
                PhoneNumber = "+385-1-2345678",
                Address = "Split, Hrvatska"
            },
            new()
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

        var cardSets = new List<CardSet>
        {
            new()
            {
                Id = 1,
                SetName = "Base Set",
                ReleaseDate = new DateTime(1999, 1, 9),
                TotalCards = 102,
                Publisher = "The Pokemon Company International",
                SetSymbol = "Circle",
                SetCode = "BS"
            },
            new()
            {
                Id = 2,
                SetName = "Jungle",
                ReleaseDate = new DateTime(1999, 6, 16),
                TotalCards = 64,
                Publisher = "The Pokemon Company International",
                SetSymbol = "Leaf",
                SetCode = "JU"
            },
            new()
            {
                Id = 3,
                SetName = "Fossil",
                ReleaseDate = new DateTime(1999, 10, 10),
                TotalCards = 62,
                Publisher = "The Pokemon Company International",
                SetSymbol = "Skull",
                SetCode = "FO"
            }
        };

        var pokemonCards = new List<PokemonCard>
        {
            new() { Id = 1, CardName = "Charizard", PokemonNumber = 6, Type = PokemonType.Fire, Rarity = CardRarity.SecretRare, MarketPrice = 1200m, CardSetId = 1, CreatedDate = new DateTime(2024, 1, 10) },
            new() { Id = 2, CardName = "Blastoise", PokemonNumber = 9, Type = PokemonType.Water, Rarity = CardRarity.UltraRare, MarketPrice = 780m, CardSetId = 1, CreatedDate = new DateTime(2024, 1, 10) },
            new() { Id = 3, CardName = "Pikachu", PokemonNumber = 25, Type = PokemonType.Electric, Rarity = CardRarity.Rare, MarketPrice = 320m, CardSetId = 2, CreatedDate = new DateTime(2024, 2, 2) },
            new() { Id = 4, CardName = "Gengar", PokemonNumber = 94, Type = PokemonType.Psychic, Rarity = CardRarity.Rare, MarketPrice = 290m, CardSetId = 3, CreatedDate = new DateTime(2024, 2, 20) },
            new() { Id = 5, CardName = "Mew", PokemonNumber = 151, Type = PokemonType.Psychic, Rarity = CardRarity.Promo, MarketPrice = 540m, CardSetId = 2, CreatedDate = new DateTime(2024, 3, 5) }
        };

        var collections = new List<Collection>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                CollectionName = "Kanto Vault",
                CreatedDate = new DateTime(2023, 8, 12),
                CollectionValue = 4100m,
                Description = "Premium Kanto cards and historic pulls.",
                IsPublic = true
            },
            new()
            {
                Id = 2,
                UserId = 2,
                CollectionName = "Budget Collector's Corner",
                CreatedDate = new DateTime(2023, 10, 5),
                CollectionValue = 1800m,
                Description = "Affordable cards with strong type coverage.",
                IsPublic = true
            },
            new()
            {
                Id = 3,
                UserId = 3,
                CollectionName = "Shadow Rarity Lab",
                CreatedDate = new DateTime(2024, 1, 2),
                CollectionValue = 2350m,
                Description = "Rare psychic and dark collection focus.",
                IsPublic = false
            }
        };

        var cardInstances = new List<CardInstance>
        {
            new() { Id = 1, CollectionId = 1, PokemonCardId = 1, Condition = CardCondition.NearMint, Quantity = 1, AcquisitionDate = new DateTime(2024, 1, 15), CurrentValue = 1200m },
            new() { Id = 2, CollectionId = 1, PokemonCardId = 2, Condition = CardCondition.Excellent, Quantity = 1, AcquisitionDate = new DateTime(2024, 1, 22), CurrentValue = 760m },
            new() { Id = 3, CollectionId = 2, PokemonCardId = 3, Condition = CardCondition.Mint, Quantity = 2, AcquisitionDate = new DateTime(2024, 2, 14), CurrentValue = 640m },
            new() { Id = 4, CollectionId = 2, PokemonCardId = 4, Condition = CardCondition.VeryGood, Quantity = 1, AcquisitionDate = new DateTime(2024, 2, 25), CurrentValue = 260m },
            new() { Id = 5, CollectionId = 3, PokemonCardId = 5, Condition = CardCondition.NearMint, Quantity = 1, AcquisitionDate = new DateTime(2024, 3, 1), CurrentValue = 540m }
        };

        var trades = new List<Trade>
        {
            new() { Id = 1, SenderId = 1, ReceiverId = 2, CardInstanceId = 2, TradeDate = new DateTime(2024, 3, 8), TransactionAmount = 780m, TradeStatus = "Completed" },
            new() { Id = 2, SenderId = 2, ReceiverId = 3, CardInstanceId = 3, TradeDate = new DateTime(2024, 3, 24), TransactionAmount = 350m, TradeStatus = "Pending" },
            new() { Id = 3, SenderId = 3, ReceiverId = 1, CardInstanceId = 5, TradeDate = new DateTime(2024, 4, 4), TransactionAmount = 560m, TradeStatus = "Negotiating" }
        };

        var wishlists = new List<Wishlist>
        {
            new() { Id = 1, UserId = 1, PokemonCardId = 5, AddedDate = new DateTime(2024, 3, 1), Priority = 1, MaxPrice = 600m },
            new() { Id = 2, UserId = 2, PokemonCardId = 1, AddedDate = new DateTime(2024, 3, 2), Priority = 2, MaxPrice = 1300m },
            new() { Id = 3, UserId = 3, PokemonCardId = 2, AddedDate = new DateTime(2024, 3, 10), Priority = 1, MaxPrice = 850m }
        };

        foreach (var card in pokemonCards)
        {
            card.CardSet = cardSets.First(s => s.Id == card.CardSetId);
            card.CardSet.Cards.Add(card);
        }

        foreach (var collection in collections)
        {
            collection.User = users.First(u => u.Id == collection.UserId);
            collection.User.Collections.Add(collection);
        }

        foreach (var instance in cardInstances)
        {
            instance.Collection = collections.First(c => c.Id == instance.CollectionId);
            instance.PokemonCard = pokemonCards.First(p => p.Id == instance.PokemonCardId);
            instance.Collection.CardInstances.Add(instance);
            instance.PokemonCard.CardInstances.Add(instance);
        }

        foreach (var trade in trades)
        {
            trade.Sender = users.First(u => u.Id == trade.SenderId);
            trade.Receiver = users.First(u => u.Id == trade.ReceiverId);
            trade.CardInstance = cardInstances.First(ci => ci.Id == trade.CardInstanceId);
        }

        foreach (var wishlist in wishlists)
        {
            wishlist.User = users.First(u => u.Id == wishlist.UserId);
            wishlist.PokemonCard = pokemonCards.First(pc => pc.Id == wishlist.PokemonCardId);
        }

        return (users, cardSets, pokemonCards, collections, cardInstances, trades, wishlists);
    }
}
