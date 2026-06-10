using System;
using System.Collections.Generic;

namespace PokemonCollector.Web.Models.DTOs
{
    public class CardSetDTO
    {
        public int Id { get; set; }
        public string SetName { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int TotalCards { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string SetSymbol { get; set; } = string.Empty;
        public string SetCode { get; set; } = string.Empty;
        public List<PokemonCardDTO> Cards { get; set; } = new();
        public List<AttachmentDTO> Attachments { get; set; } = new();
    }
}
