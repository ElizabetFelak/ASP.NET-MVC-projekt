using System;

namespace PokemonCollector.Web.Models.DTOs
{
    public class WishlistDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PokemonCardId { get; set; }
        public DateTime AddedDate { get; set; }
        public int Priority { get; set; }
        public decimal MaxPrice { get; set; }
        public UserDTO? User { get; set; }
        public PokemonCardDTO? PokemonCard { get; set; }
    }
}
