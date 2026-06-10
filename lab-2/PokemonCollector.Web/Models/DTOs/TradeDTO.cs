using System;

namespace PokemonCollector.Web.Models.DTOs
{
    public class TradeDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int CardInstanceId { get; set; }
        public DateTime TradeDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TradeStatus { get; set; } = string.Empty;
        public UserDTO? Sender { get; set; }
        public UserDTO? Receiver { get; set; }
        public CardInstanceDTO? CardInstance { get; set; }
    }
}
