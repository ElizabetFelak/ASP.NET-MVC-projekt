using System;

namespace PokemonCollector.Web.Models.DTOs
{
    public class PokemonCardDTO
    {
        public int Id { get; set; }
        public string CardName { get; set; } = string.Empty;
        public int PokemonNumber { get; set; }
        public PokemonType Type { get; set; }
        public CardRarity Rarity { get; set; }
        public decimal MarketPrice { get; set; }
        public int CardSetId { get; set; }
        public DateTime CreatedDate { get; set; }
        public CardSetDTO? CardSet { get; set; }
    }
}
