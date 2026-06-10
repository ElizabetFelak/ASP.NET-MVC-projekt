using System;
using System.Collections.Generic;

namespace PokemonCollector.Web.Models.DTOs
{
    public class CollectionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CollectionName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal CollectionValue { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public UserDTO? User { get; set; }
        public List<CardInstanceDTO> CardInstances { get; set; } = new();
    }
}
