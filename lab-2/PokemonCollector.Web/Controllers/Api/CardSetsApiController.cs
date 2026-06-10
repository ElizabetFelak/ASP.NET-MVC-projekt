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
    [Route("api/cardsets")]
    [ApiController]
    public class CardSetsApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public CardSetsApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/cardsets?search=keyword
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<CardSetDTO>> GetCardSets([FromQuery] string search = "")
        {
            var query = _context.CardSets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(cs =>
                    EF.Functions.Like(cs.SetName, $"%{search}%") ||
                    EF.Functions.Like(cs.Publisher, $"%{search}%") ||
                    EF.Functions.Like(cs.SetCode, $"%{search}%"));
            }

            var cardSets = query
                .Include(cs => cs.Cards)
                .Include(cs => cs.Attachments)
                .OrderBy(cs => cs.SetName)
                .ToList()
                .Select(cs => ToDTO(cs))
                .ToList();

            return Ok(cardSets);
        }

        // GET: api/cardsets/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<CardSetDTO> GetCardSet(int id)
        {
            var cardSet = _context.CardSets
                .Include(cs => cs.Cards)
                .Include(cs => cs.Attachments)
                .FirstOrDefault(cs => cs.Id == id);

            if (cardSet == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(cardSet));
        }

        // PUT: api/cardsets/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCardSet(int id, CardSetDTO cardSetDTO)
        {
            if (id != cardSetDTO.Id)
            {
                return BadRequest();
            }

            var cardSet = await _context.CardSets.FindAsync(id);
            if (cardSet == null)
            {
                return NotFound();
            }

            cardSet.SetName = cardSetDTO.SetName;
            cardSet.ReleaseDate = cardSetDTO.ReleaseDate;
            cardSet.TotalCards = cardSetDTO.TotalCards;
            cardSet.Publisher = cardSetDTO.Publisher;
            cardSet.SetSymbol = cardSetDTO.SetSymbol;
            cardSet.SetCode = cardSetDTO.SetCode;

            _context.Entry(cardSet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardSetExists(id))
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

        // POST: api/cardsets
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CardSetDTO>> PostCardSet(CardSetDTO cardSetDTO)
        {
            var cardSet = new CardSet
            {
                SetName = cardSetDTO.SetName,
                ReleaseDate = cardSetDTO.ReleaseDate,
                TotalCards = cardSetDTO.TotalCards,
                Publisher = cardSetDTO.Publisher,
                SetSymbol = cardSetDTO.SetSymbol,
                SetCode = cardSetDTO.SetCode
            };

            _context.CardSets.Add(cardSet);
            await _context.SaveChangesAsync();

            cardSetDTO.Id = cardSet.Id;

            return CreatedAtAction(nameof(GetCardSet), new { id = cardSet.Id }, cardSetDTO);
        }

        // DELETE: api/cardsets/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardSet(int id)
        {
            var cardSet = await _context.CardSets.FindAsync(id);
            if (cardSet == null)
            {
                return NotFound();
            }

            _context.CardSets.Remove(cardSet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardSetExists(int id)
        {
            return _context.CardSets.Any(e => e.Id == id);
        }

        private static CardSetDTO ToDTO(CardSet cardSet) =>
            new CardSetDTO
            {
                Id = cardSet.Id,
                SetName = cardSet.SetName,
                ReleaseDate = cardSet.ReleaseDate,
                TotalCards = cardSet.TotalCards,
                Publisher = cardSet.Publisher,
                SetSymbol = cardSet.SetSymbol,
                SetCode = cardSet.SetCode,
                Cards = cardSet.Cards?.Select(c => new PokemonCardDTO
                {
                    Id = c.Id,
                    CardName = c.CardName,
                    PokemonNumber = c.PokemonNumber,
                    Type = c.Type,
                    Rarity = c.Rarity,
                    MarketPrice = c.MarketPrice,
                    CardSetId = c.CardSetId,
                    CreatedDate = c.CreatedDate
                }).ToList() ?? new(),
                Attachments = cardSet.Attachments?.Select(a => new AttachmentDTO
                {
                    Id = a.Id,
                    CardSetId = a.CardSetId,
                    FileName = a.FileName,
                    FilePath = a.FilePath,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    CreatedAt = a.CreatedAt
                }).ToList() ?? new()
            };
    }
}
