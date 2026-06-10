using System;

namespace PokemonCollector.Web.Models.DTOs
{
    public class CardInstanceDTO
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public int PokemonCardId { get; set; }
        public CardCondition Condition { get; set; }
        public int Quantity { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal CurrentValue { get; set; }
        public PokemonCardDTO? PokemonCard { get; set; }
        public CollectionDTO? Collection { get; set; }
    }
}
