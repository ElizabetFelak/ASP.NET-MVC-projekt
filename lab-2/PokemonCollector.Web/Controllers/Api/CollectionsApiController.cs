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
    [Route("api/collections")]
    [ApiController]
    public class CollectionsApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public CollectionsApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/collections?userId=1&isPublic=true&search=keyword
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<CollectionDTO>> GetCollections(
            [FromQuery] int userId = 0,
            [FromQuery] bool? isPublic = null,
            [FromQuery] string search = "")
        {
            var query = _context.Collections.AsQueryable();

            if (userId > 0)
            {
                query = query.Where(c => c.UserId == userId);
            }

            if (isPublic.HasValue)
            {
                query = query.Where(c => c.IsPublic == isPublic);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    EF.Functions.Like(c.CollectionName, $"%{search}%") ||
                    EF.Functions.Like(c.Description, $"%{search}%"));
            }

            var collections = query
                .Include(c => c.User)
                .Include(c => c.CardInstances)
                    .ThenInclude(ci => ci.PokemonCard)
                .OrderBy(c => c.CollectionName)
                .ToList()
                .Select(c => ToDTO(c))
                .ToList();

            return Ok(collections);
        }

        // GET: api/collections/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<CollectionDTO> GetCollection(int id)
        {
            var collection = _context.Collections
                .Include(c => c.User)
                .Include(c => c.CardInstances)
                    .ThenInclude(ci => ci.PokemonCard)
                        .ThenInclude(pc => pc.CardSet)
                .FirstOrDefault(c => c.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(collection));
        }

        // PUT: api/collections/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection(int id, CollectionDTO collectionDTO)
        {
            if (id != collectionDTO.Id)
            {
                return BadRequest();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            collection.UserId = collectionDTO.UserId;
            collection.CollectionName = collectionDTO.CollectionName;
            collection.CreatedDate = collectionDTO.CreatedDate;
            collection.CollectionValue = collectionDTO.CollectionValue;
            collection.Description = collectionDTO.Description;
            collection.IsPublic = collectionDTO.IsPublic;

            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/collections
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CollectionDTO>> PostCollection(CollectionDTO collectionDTO)
        {
            var collection = new Collection
            {
                UserId = collectionDTO.UserId,
                CollectionName = collectionDTO.CollectionName,
                CreatedDate = collectionDTO.CreatedDate,
                CollectionValue = collectionDTO.CollectionValue,
                Description = collectionDTO.Description,
                IsPublic = collectionDTO.IsPublic
            };

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            collectionDTO.Id = collection.Id;

            return CreatedAtAction(nameof(GetCollection), new { id = collection.Id }, collectionDTO);
        }

        // DELETE: api/collections/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }

        private static CollectionDTO ToDTO(Collection collection) =>
            new CollectionDTO
            {
                Id = collection.Id,
                UserId = collection.UserId,
                CollectionName = collection.CollectionName,
                CreatedDate = collection.CreatedDate,
                CollectionValue = collection.CollectionValue,
                Description = collection.Description,
                IsPublic = collection.IsPublic,
                User = collection.User == null ? null : new UserDTO
                {
                    Id = collection.User.Id,
                    Username = collection.User.Username,
                    Email = collection.User.Email,
                    RegistrationDate = collection.User.RegistrationDate,
                    Budget = collection.User.Budget,
                    PhoneNumber = collection.User.PhoneNumber,
                    Address = collection.User.Address
                },
                CardInstances = collection.CardInstances?.Select(ci => new CardInstanceDTO
                {
                    Id = ci.Id,
                    CollectionId = ci.CollectionId,
                    PokemonCardId = ci.PokemonCardId,
                    Condition = ci.Condition,
                    Quantity = ci.Quantity,
                    AcquisitionDate = ci.AcquisitionDate,
                    CurrentValue = ci.CurrentValue,
                    PokemonCard = ci.PokemonCard == null ? null : new PokemonCardDTO
                    {
                        Id = ci.PokemonCard.Id,
                        CardName = ci.PokemonCard.CardName,
                        PokemonNumber = ci.PokemonCard.PokemonNumber,
                        Type = ci.PokemonCard.Type,
                        Rarity = ci.PokemonCard.Rarity,
                        MarketPrice = ci.PokemonCard.MarketPrice,
                        CardSetId = ci.PokemonCard.CardSetId,
                        CreatedDate = ci.PokemonCard.CreatedDate
                    }
                }).ToList() ?? new()
            };
    }
}
