using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime RegistrationDate { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal Budget { get; set; }
    [Phone]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;
    
    // 1-N veza: User -> Collections
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    
    // 1-N veza: User -> Trade (kao Sender)
    public virtual ICollection<Trade> SentTrades { get; set; } = new List<Trade>();
    
    // 1-N veza: User -> Trade (kao Receiver)
    public virtual ICollection<Trade> ReceivedTrades { get; set; } = new List<Trade>();
    
    // 1-N veza: User -> Wishlist
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}

public class CardSet
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(200)]
    public string SetName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ReleaseDate { get; set; }
    [Range(1, 10000)]
    public int TotalCards { get; set; }
    [Required]
    [StringLength(200)]
    public string Publisher { get; set; } = string.Empty;
    [Required]
    [StringLength(20)]
    public string SetSymbol { get; set; } = string.Empty;
    [Required]
    [StringLength(20)]
    public string SetCode { get; set; } = string.Empty;
    
    // 1-N veza: CardSet -> PokemonCard
    public virtual ICollection<PokemonCard> Cards { get; set; } = new List<PokemonCard>();
}

public class PokemonCard
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(200)]
    public string CardName { get; set; } = string.Empty;
    [Range(1, 10000)]
    public int PokemonNumber { get; set; }
    public PokemonType Type { get; set; }
    public CardRarity Rarity { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal MarketPrice { get; set; }
    
    [ForeignKey(nameof(CardSet))]
    [Range(1, int.MaxValue)]
    public int CardSetId { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }
    
    // N-1 veza prema CardSet
    public virtual CardSet? CardSet { get; set; }
    
    // 1-N veza: PokemonCard -> CardInstance
    public virtual ICollection<CardInstance> CardInstances { get; set; } = new List<CardInstance>();
    
    // 1-N veza: PokemonCard -> Wishlist
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}

public class Collection
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(User))]
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string CollectionName { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal CollectionValue { get; set; }
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    
    // N-1 veza prema User
    public virtual User? User { get; set; }
    
    // 1-N veza: Collection -> CardInstance
    public virtual ICollection<CardInstance> CardInstances { get; set; } = new List<CardInstance>();
}

public class CardInstance
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Collection))]
    [Range(1, int.MaxValue)]
    public int CollectionId { get; set; }
    
    [ForeignKey(nameof(PokemonCard))]
    [Range(1, int.MaxValue)]
    public int PokemonCardId { get; set; }
    
    public CardCondition Condition { get; set; }
    [Range(1, 100000)]
    public int Quantity { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime AcquisitionDate { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal CurrentValue { get; set; }
    
    // N-1 veza prema Collection
    public virtual Collection? Collection { get; set; }
    
    // N-1 veza prema PokemonCard
    public virtual PokemonCard? PokemonCard { get; set; }
    
    // 1-N veza: CardInstance -> Trade
    public virtual ICollection<Trade> Trades { get; set; } = new List<Trade>();
}

public class Trade
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Sender))]
    [Range(1, int.MaxValue)]
    public int SenderId { get; set; }
    
    [ForeignKey(nameof(Receiver))]
    [Range(1, int.MaxValue)]
    public int ReceiverId { get; set; }
    
    [ForeignKey(nameof(CardInstance))]
    [Range(1, int.MaxValue)]
    public int CardInstanceId { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime TradeDate { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal TransactionAmount { get; set; }
    [Required]
    [StringLength(50)]
    public string TradeStatus { get; set; } = string.Empty;
    
    // N-1 veza prema User kao Sender
    public virtual User? Sender { get; set; }
    
    // N-1 veza prema User kao Receiver
    public virtual User? Receiver { get; set; }
    
    // N-1 veza prema CardInstance
    public virtual CardInstance? CardInstance { get; set; }
}

public class Wishlist
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(User))]
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
    
    [ForeignKey(nameof(PokemonCard))]
    [Range(1, int.MaxValue)]
    public int PokemonCardId { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime AddedDate { get; set; }
    [Range(1, 3)]
    public int Priority { get; set; }
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal MaxPrice { get; set; }
    
    // N-1 veza prema User
    public virtual User? User { get; set; }
    
    // N-1 veza prema PokemonCard
    public virtual PokemonCard? PokemonCard { get; set; }
}
