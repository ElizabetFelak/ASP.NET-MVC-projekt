namespace PokemonCollector.Web.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public int? CardSetId { get; set; }
        public CardSet? CardSet { get; set; }

        public int? PokemonCardId { get; set; }
        public PokemonCard? PokemonCard { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
