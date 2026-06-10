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
    [Route("api/wishlists")]
    [ApiController]
    public class WishlistsApiController : ControllerBase
    {
        private readonly PokemonCollectorDbContext _context;

        public WishlistsApiController(PokemonCollectorDbContext context)
        {
            _context = context;
        }

        // GET: api/wishlists?userId=1&priority=1
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public ActionResult<IEnumerable<WishlistDTO>> GetWishlists(
            [FromQuery] int userId = 0,
            [FromQuery] int? priority = null)
        {
            var query = _context.Wishlists.AsQueryable();

            if (userId > 0)
            {
                query = query.Where(w => w.UserId == userId);
            }

            if (priority.HasValue)
            {
                query = query.Where(w => w.Priority == priority);
            }

            var wishlists = query
                .Include(w => w.User)
                .Include(w => w.PokemonCard)
                    .ThenInclude(pc => pc.CardSet)
                .OrderBy(w => w.Priority)
                .ThenByDescending(w => w.AddedDate)
                .ToList()
                .Select(w => ToDTO(w))
                .ToList();

            return Ok(wishlists);
        }

        // GET: api/wishlists/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<ActionResult<WishlistDTO>> GetWishlist(int id)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.PokemonCard)
                    .ThenInclude(pc => pc.CardSet)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wishlist == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(wishlist));
        }

        // PUT: api/wishlists/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWishlist(int id, WishlistDTO wishlistDTO)
        {
            if (id != wishlistDTO.Id)
            {
                return BadRequest();
            }

            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            wishlist.UserId = wishlistDTO.UserId;
            wishlist.PokemonCardId = wishlistDTO.PokemonCardId;
            wishlist.AddedDate = wishlistDTO.AddedDate;
            wishlist.Priority = wishlistDTO.Priority;
            wishlist.MaxPrice = wishlistDTO.MaxPrice;

            _context.Entry(wishlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(id))
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

        // POST: api/wishlists
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<WishlistDTO>> PostWishlist(WishlistDTO wishlistDTO)
        {
            var wishlist = new Wishlist
            {
                UserId = wishlistDTO.UserId,
                PokemonCardId = wishlistDTO.PokemonCardId,
                AddedDate = wishlistDTO.AddedDate,
                Priority = wishlistDTO.Priority,
                MaxPrice = wishlistDTO.MaxPrice
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            wishlistDTO.Id = wishlist.Id;

            return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.Id }, wishlistDTO);
        }

        // DELETE: api/wishlists/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WishlistExists(int id)
        {
            return _context.Wishlists.Any(e => e.Id == id);
        }

        private static WishlistDTO ToDTO(Wishlist wishlist) =>
            new WishlistDTO
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                PokemonCardId = wishlist.PokemonCardId,
                AddedDate = wishlist.AddedDate,
                Priority = wishlist.Priority,
                MaxPrice = wishlist.MaxPrice,
                User = wishlist.User == null ? null : new UserDTO
                {
                    Id = wishlist.User.Id,
                    Username = wishlist.User.Username,
                    Email = wishlist.User.Email,
                    RegistrationDate = wishlist.User.RegistrationDate,
                    Budget = wishlist.User.Budget,
                    PhoneNumber = wishlist.User.PhoneNumber,
                    Address = wishlist.User.Address
                },
                PokemonCard = wishlist.PokemonCard == null ? null : new PokemonCardDTO
                {
                    Id = wishlist.PokemonCard.Id,
                    CardName = wishlist.PokemonCard.CardName,
                    PokemonNumber = wishlist.PokemonCard.PokemonNumber,
                    Type = wishlist.PokemonCard.Type,
                    Rarity = wishlist.PokemonCard.Rarity,
                    MarketPrice = wishlist.PokemonCard.MarketPrice,
                    CardSetId = wishlist.PokemonCard.CardSetId,
                    CreatedDate = wishlist.PokemonCard.CreatedDate,
                    CardSet = wishlist.PokemonCard.CardSet == null ? null : new CardSetDTO
                    {
                        Id = wishlist.PokemonCard.CardSet.Id,
                        SetName = wishlist.PokemonCard.CardSet.SetName,
                        ReleaseDate = wishlist.PokemonCard.CardSet.ReleaseDate,
                        TotalCards = wishlist.PokemonCard.CardSet.TotalCards,
                        Publisher = wishlist.PokemonCard.CardSet.Publisher,
                        SetSymbol = wishlist.PokemonCard.CardSet.SetSymbol,
                        SetCode = wishlist.PokemonCard.CardSet.SetCode
                    }
                }
            };
    }
}
