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
    [Route("api/pokemoncards")]
    [ApiController]
    public class PokemonCardsApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public PokemonCardsApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/pokemoncards?search=keyword&type=Fire&rarity=Rare
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<PokemonCardDTO>> GetPokemonCards(
            [FromQuery] string search = "",
            [FromQuery] PokemonType? type = null,
            [FromQuery] CardRarity? rarity = null,
            [FromQuery] int cardSetId = 0)
        {
            var query = _context.PokemonCards.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(pc =>
                    EF.Functions.Like(pc.CardName, $"%{search}%") ||
                    (pc.CardSet != null && EF.Functions.Like(pc.CardSet.SetName, $"%{search}%")));
            }

            if (type.HasValue)
            {
                query = query.Where(pc => pc.Type == type);
            }

            if (rarity.HasValue)
            {
                query = query.Where(pc => pc.Rarity == rarity);
            }

            if (cardSetId > 0)
            {
                query = query.Where(pc => pc.CardSetId == cardSetId);
            }

            var pokemonCards = query
                .Include(pc => pc.CardSet)
                .OrderBy(pc => pc.CardName)
                .ToList()
                .Select(pc => ToDTO(pc))
                .ToList();

            return Ok(pokemonCards);
        }

        // GET: api/pokemoncards/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<PokemonCardDTO> GetPokemonCard(int id)
        {
            var pokemonCard = _context.PokemonCards
                .Include(pc => pc.CardSet)
                .FirstOrDefault(pc => pc.Id == id);

            if (pokemonCard == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(pokemonCard));
        }

        // PUT: api/pokemoncards/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPokemonCard(int id, PokemonCardDTO pokemonCardDTO)
        {
            if (id != pokemonCardDTO.Id)
            {
                return BadRequest();
            }

            var pokemonCard = await _context.PokemonCards.FindAsync(id);
            if (pokemonCard == null)
            {
                return NotFound();
            }

            pokemonCard.CardName = pokemonCardDTO.CardName;
            pokemonCard.PokemonNumber = pokemonCardDTO.PokemonNumber;
            pokemonCard.Type = pokemonCardDTO.Type;
            pokemonCard.Rarity = pokemonCardDTO.Rarity;
            pokemonCard.MarketPrice = pokemonCardDTO.MarketPrice;
            pokemonCard.CardSetId = pokemonCardDTO.CardSetId;
            pokemonCard.CreatedDate = pokemonCardDTO.CreatedDate;

            _context.Entry(pokemonCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PokemonCardExists(id))
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

        // POST: api/pokemoncards
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PokemonCardDTO>> PostPokemonCard(PokemonCardDTO pokemonCardDTO)
        {
            var pokemonCard = new PokemonCard
            {
                CardName = pokemonCardDTO.CardName,
                PokemonNumber = pokemonCardDTO.PokemonNumber,
                Type = pokemonCardDTO.Type,
                Rarity = pokemonCardDTO.Rarity,
                MarketPrice = pokemonCardDTO.MarketPrice,
                CardSetId = pokemonCardDTO.CardSetId,
                CreatedDate = pokemonCardDTO.CreatedDate
            };

            _context.PokemonCards.Add(pokemonCard);
            await _context.SaveChangesAsync();

            pokemonCardDTO.Id = pokemonCard.Id;

            return CreatedAtAction(nameof(GetPokemonCard), new { id = pokemonCard.Id }, pokemonCardDTO);
        }

        // DELETE: api/pokemoncards/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePokemonCard(int id)
        {
            var pokemonCard = await _context.PokemonCards.FindAsync(id);
            if (pokemonCard == null)
            {
                return NotFound();
            }

            _context.PokemonCards.Remove(pokemonCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PokemonCardExists(int id)
        {
            return _context.PokemonCards.Any(e => e.Id == id);
        }

        private static PokemonCardDTO ToDTO(PokemonCard pokemonCard) =>
            new PokemonCardDTO
            {
                Id = pokemonCard.Id,
                CardName = pokemonCard.CardName,
                PokemonNumber = pokemonCard.PokemonNumber,
                Type = pokemonCard.Type,
                Rarity = pokemonCard.Rarity,
                MarketPrice = pokemonCard.MarketPrice,
                CardSetId = pokemonCard.CardSetId,
                CreatedDate = pokemonCard.CreatedDate,
                CardSet = pokemonCard.CardSet == null ? null : new CardSetDTO
                {
                    Id = pokemonCard.CardSet.Id,
                    SetName = pokemonCard.CardSet.SetName,
                    ReleaseDate = pokemonCard.CardSet.ReleaseDate,
                    TotalCards = pokemonCard.CardSet.TotalCards,
                    Publisher = pokemonCard.CardSet.Publisher,
                    SetSymbol = pokemonCard.CardSet.SetSymbol,
                    SetCode = pokemonCard.CardSet.SetCode
                }
            };
    }
}
