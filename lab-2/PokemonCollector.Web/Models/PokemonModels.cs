using System;
using System.Collections.Generic;

namespace PokemonCollector.Web.Models;

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    UltraRare,
    SecretRare,
    Promo
}

public enum CardCondition
{
    Poor,
    Fair,
    Good,
    VeryGood,
    Excellent,
    NearMint,
    Mint
}

public enum PokemonType
{
    Colorless,
    Fire,
    Water,
    Electric,
    Grass,
    Fighting,
    Psychic,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public decimal Budget { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<Collection> Collections { get; set; } = new();
}

public class CardSet
{
    public int Id { get; set; }
    public string SetName { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int TotalCards { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string SetSymbol { get; set; } = string.Empty;
    public string SetCode { get; set; } = string.Empty;
    public List<PokemonCard> Cards { get; set; } = new();
}

public class PokemonCard
{
    public int Id { get; set; }
    public string CardName { get; set; } = string.Empty;
    public int PokemonNumber { get; set; }
    public PokemonType Type { get; set; }
    public CardRarity Rarity { get; set; }
    public decimal MarketPrice { get; set; }
    public int CardSetId { get; set; }
    public DateTime CreatedDate { get; set; }
    public CardSet? CardSet { get; set; }
    public List<CardInstance> CardInstances { get; set; } = new();
}

public class Collection
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public decimal CollectionValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public User? User { get; set; }
    public List<CardInstance> CardInstances { get; set; } = new();
}

public class CardInstance
{
    public int Id { get; set; }
    public int CollectionId { get; set; }
    public int PokemonCardId { get; set; }
    public CardCondition Condition { get; set; }
    public int Quantity { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public decimal CurrentValue { get; set; }
    public Collection? Collection { get; set; }
    public PokemonCard? PokemonCard { get; set; }
}

public class Trade
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public int CardInstanceId { get; set; }
    public DateTime TradeDate { get; set; }
    public decimal TransactionAmount { get; set; }
    public string TradeStatus { get; set; } = string.Empty;
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public CardInstance? CardInstance { get; set; }
}

public class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PokemonCardId { get; set; }
    public DateTime AddedDate { get; set; }
    public int Priority { get; set; }
    public decimal MaxPrice { get; set; }
    public User? User { get; set; }
    public PokemonCard? PokemonCard { get; set; }
}
