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
    [Route("api/cardinstances")]
    [ApiController]
    public class CardInstancesApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public CardInstancesApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/cardinstances?collectionId=1&condition=Mint
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<CardInstanceDTO>> GetCardInstances(
            [FromQuery] int collectionId = 0,
            [FromQuery] CardCondition? condition = null)
        {
            var query = _context.CardInstances.AsQueryable();

            if (collectionId > 0)
            {
                query = query.Where(ci => ci.CollectionId == collectionId);
            }

            if (condition.HasValue)
            {
                query = query.Where(ci => ci.Condition == condition);
            }

            var cardInstances = query
                .Include(ci => ci.PokemonCard)
                    .ThenInclude(pc => pc.CardSet)
                .Include(ci => ci.Collection)
                .OrderBy(ci => ci.PokemonCard.CardName)
                .ToList()
                .Select(ci => ToDTO(ci))
                .ToList();

            return Ok(cardInstances);
        }

        // GET: api/cardinstances/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<CardInstanceDTO> GetCardInstance(int id)
        {
            var cardInstance = _context.CardInstances
                .Include(ci => ci.PokemonCard)
                    .ThenInclude(pc => pc.CardSet)
                .Include(ci => ci.Collection)
                .FirstOrDefault(ci => ci.Id == id);

            if (cardInstance == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(cardInstance));
        }

        // PUT: api/cardinstances/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCardInstance(int id, CardInstanceDTO cardInstanceDTO)
        {
            if (id != cardInstanceDTO.Id)
            {
                return BadRequest();
            }

            var cardInstance = await _context.CardInstances.FindAsync(id);
            if (cardInstance == null)
            {
                return NotFound();
            }

            cardInstance.CollectionId = cardInstanceDTO.CollectionId;
            cardInstance.PokemonCardId = cardInstanceDTO.PokemonCardId;
            cardInstance.Condition = cardInstanceDTO.Condition;
            cardInstance.Quantity = cardInstanceDTO.Quantity;
            cardInstance.AcquisitionDate = cardInstanceDTO.AcquisitionDate;
            cardInstance.CurrentValue = cardInstanceDTO.CurrentValue;

            _context.Entry(cardInstance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardInstanceExists(id))
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

        // POST: api/cardinstances
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CardInstanceDTO>> PostCardInstance(CardInstanceDTO cardInstanceDTO)
        {
            var cardInstance = new CardInstance
            {
                CollectionId = cardInstanceDTO.CollectionId,
                PokemonCardId = cardInstanceDTO.PokemonCardId,
                Condition = cardInstanceDTO.Condition,
                Quantity = cardInstanceDTO.Quantity,
                AcquisitionDate = cardInstanceDTO.AcquisitionDate,
                CurrentValue = cardInstanceDTO.CurrentValue
            };

            _context.CardInstances.Add(cardInstance);
            await _context.SaveChangesAsync();

            cardInstanceDTO.Id = cardInstance.Id;

            return CreatedAtAction(nameof(GetCardInstance), new { id = cardInstance.Id }, cardInstanceDTO);
        }

        // DELETE: api/cardinstances/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardInstance(int id)
        {
            var cardInstance = await _context.CardInstances.FindAsync(id);
            if (cardInstance == null)
            {
                return NotFound();
            }

            _context.CardInstances.Remove(cardInstance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardInstanceExists(int id)
        {
            return _context.CardInstances.Any(e => e.Id == id);
        }

        private static CardInstanceDTO ToDTO(CardInstance cardInstance) =>
            new CardInstanceDTO
            {
                Id = cardInstance.Id,
                CollectionId = cardInstance.CollectionId,
                PokemonCardId = cardInstance.PokemonCardId,
                Condition = cardInstance.Condition,
                Quantity = cardInstance.Quantity,
                AcquisitionDate = cardInstance.AcquisitionDate,
                CurrentValue = cardInstance.CurrentValue,
                PokemonCard = cardInstance.PokemonCard == null ? null : new PokemonCardDTO
                {
                    Id = cardInstance.PokemonCard.Id,
                    CardName = cardInstance.PokemonCard.CardName,
                    PokemonNumber = cardInstance.PokemonCard.PokemonNumber,
                    Type = cardInstance.PokemonCard.Type,
                    Rarity = cardInstance.PokemonCard.Rarity,
                    MarketPrice = cardInstance.PokemonCard.MarketPrice,
                    CardSetId = cardInstance.PokemonCard.CardSetId,
                    CreatedDate = cardInstance.PokemonCard.CreatedDate,
                    CardSet = cardInstance.PokemonCard.CardSet == null ? null : new CardSetDTO
                    {
                        Id = cardInstance.PokemonCard.CardSet.Id,
                        SetName = cardInstance.PokemonCard.CardSet.SetName,
                        ReleaseDate = cardInstance.PokemonCard.CardSet.ReleaseDate,
                        TotalCards = cardInstance.PokemonCard.CardSet.TotalCards,
                        Publisher = cardInstance.PokemonCard.CardSet.Publisher,
                        SetSymbol = cardInstance.PokemonCard.CardSet.SetSymbol,
                        SetCode = cardInstance.PokemonCard.CardSet.SetCode
                    }
                },
                Collection = cardInstance.Collection == null ? null : new CollectionDTO
                {
                    Id = cardInstance.Collection.Id,
                    UserId = cardInstance.Collection.UserId,
                    CollectionName = cardInstance.Collection.CollectionName,
                    CreatedDate = cardInstance.Collection.CreatedDate,
                    CollectionValue = cardInstance.Collection.CollectionValue,
                    Description = cardInstance.Collection.Description,
                    IsPublic = cardInstance.Collection.IsPublic
                }
            };
    }
}
