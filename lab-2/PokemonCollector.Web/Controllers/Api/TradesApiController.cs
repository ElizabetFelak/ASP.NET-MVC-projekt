using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonCollector.Web.Controllers.Api
{
    [Route("api/trades")]
    [ApiController]
    public class TradesApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public TradesApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/trades?status=Pending&senderId=1&receiverId=2
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<TradeDTO>> GetTrades(
            [FromQuery] string status = "",
            [FromQuery] int senderId = 0,
            [FromQuery] int receiverId = 0)
        {
            var query = _context.Trades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => EF.Functions.Like(t.TradeStatus, $"%{status}%"));
            }

            if (senderId > 0)
            {
                query = query.Where(t => t.SenderId == senderId);
            }

            if (receiverId > 0)
            {
                query = query.Where(t => t.ReceiverId == receiverId);
            }

            var trades = query
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Include(t => t.CardInstance)
                    .ThenInclude(ci => ci.PokemonCard)
                .OrderByDescending(t => t.TradeDate)
                .ToList()
                .Select(t => ToDTO(t))
                .ToList();

            return Ok(trades);
        }

        // GET: api/trades/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<TradeDTO> GetTrade(int id)
        {
            var trade = _context.Trades
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Include(t => t.CardInstance)
                    .ThenInclude(ci => ci.PokemonCard)
                        .ThenInclude(pc => pc.CardSet)
                .Include(t => t.CardInstance)
                    .ThenInclude(ci => ci.Collection)
                .FirstOrDefault(t => t.Id == id);

            if (trade == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(trade));
        }

        // PUT: api/trades/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrade(int id, TradeDTO tradeDTO)
        {
            if (id != tradeDTO.Id)
            {
                return BadRequest();
            }

            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
            {
                return NotFound();
            }

            trade.SenderId = tradeDTO.SenderId;
            trade.ReceiverId = tradeDTO.ReceiverId;
            trade.CardInstanceId = tradeDTO.CardInstanceId;
            trade.TradeDate = tradeDTO.TradeDate;
            trade.TransactionAmount = tradeDTO.TransactionAmount;
            trade.TradeStatus = tradeDTO.TradeStatus;

            _context.Entry(trade).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TradeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/trades
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TradeDTO>> PostTrade(TradeDTO tradeDTO)
        {
            var trade = new Trade
            {
                SenderId = tradeDTO.SenderId,
                ReceiverId = tradeDTO.ReceiverId,
                CardInstanceId = tradeDTO.CardInstanceId,
                TradeDate = tradeDTO.TradeDate,
                TransactionAmount = tradeDTO.TransactionAmount,
                TradeStatus = tradeDTO.TradeStatus
            };

            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();

            tradeDTO.Id = trade.Id;

            return CreatedAtAction(nameof(GetTrade), new { id = trade.Id }, tradeDTO);
        }

        // DELETE: api/trades/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
            {
                return NotFound();
            }

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TradeExists(int id)
        {
            return _context.Trades.Any(e => e.Id == id);
        }

        private static TradeDTO ToDTO(Trade trade) =>
            new TradeDTO
            {
                Id = trade.Id,
                SenderId = trade.SenderId,
                ReceiverId = trade.ReceiverId,
                CardInstanceId = trade.CardInstanceId,
                TradeDate = trade.TradeDate,
                TransactionAmount = trade.TransactionAmount,
                TradeStatus = trade.TradeStatus,
                Sender = trade.Sender == null ? null : new UserDTO
                {
                    Id = trade.Sender.Id,
                    Username = trade.Sender.Username,
                    Email = trade.Sender.Email,
                    RegistrationDate = trade.Sender.RegistrationDate,
                    Budget = trade.Sender.Budget,
                    PhoneNumber = trade.Sender.PhoneNumber,
                    Address = trade.Sender.Address
                },
                Receiver = trade.Receiver == null ? null : new UserDTO
                {
                    Id = trade.Receiver.Id,
                    Username = trade.Receiver.Username,
                    Email = trade.Receiver.Email,
                    RegistrationDate = trade.Receiver.RegistrationDate,
                    Budget = trade.Receiver.Budget,
                    PhoneNumber = trade.Receiver.PhoneNumber,
                    Address = trade.Receiver.Address
                },
                CardInstance = trade.CardInstance == null ? null : new CardInstanceDTO
                {
                    Id = trade.CardInstance.Id,
                    CollectionId = trade.CardInstance.CollectionId,
                    PokemonCardId = trade.CardInstance.PokemonCardId,
                    Condition = trade.CardInstance.Condition,
                    Quantity = trade.CardInstance.Quantity,
                    AcquisitionDate = trade.CardInstance.AcquisitionDate,
                    CurrentValue = trade.CardInstance.CurrentValue
                }
            };
    }
}
