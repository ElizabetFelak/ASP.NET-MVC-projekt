using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("[controller]")]
public class WishlistsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public WishlistsController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var users = await _dbContext.DomainUsers.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Users = users;
        ViewBag.Cards = cards;
        var model = new Wishlist { AddedDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(Wishlist model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Wishlists.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var users = await _dbContext.DomainUsers.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Users = users;
        ViewBag.Cards = cards;
        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();
        
        var users = await _dbContext.DomainUsers.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Users = users;
        ViewBag.Cards = cards;
        
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();

        var ok = await TryUpdateModelAsync<Wishlist>(item, "",
            w => w.UserId,
            w => w.PokemonCardId,
            w => w.AddedDate,
            w => w.Priority,
            w => w.MaxPrice);

        if (ok && ModelState.IsValid)
        {
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(item);
    }

    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.Wishlists.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Route("")]
    [Route("index")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Wishlists", IsActive = true });

        var wishlists = _dbContext.Wishlists
            .AsNoTracking()
            .Include(wishlist => wishlist.User)
            .Include(wishlist => wishlist.PokemonCard)
            .OrderBy(wishlist => wishlist.Priority)
            .ToList();

        return View(wishlists);
    }

    [HttpGet("/Wishlists/search")]
    [AllowAnonymous]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.Wishlists
            .AsNoTracking()
            .Include(w => w.User)
            .Include(w => w.PokemonCard)
            .Where(w => string.IsNullOrEmpty(query)
                || (w.PokemonCard != null && EF.Functions.Like(w.PokemonCard.CardName, $"%{query}%"))
                || (w.User != null && EF.Functions.Like(w.User.Username, $"%{query}%")))
            .OrderBy(w => w.Priority)
            .Select(w => new {
                id = w.Id,
                cardName = w.PokemonCard != null ? w.PokemonCard.CardName : string.Empty,
                userName = w.User != null ? w.User.Username : string.Empty,
                priority = w.Priority,
                maxPrice = w.MaxPrice,
                addedDate = w.AddedDate
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}/details")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var wishlist = await _dbContext.Wishlists
            .AsNoTracking()
            .Include(existingWishlist => existingWishlist.User)
            .Include(existingWishlist => existingWishlist.PokemonCard)
            .FirstOrDefaultAsync(existingWishlist => existingWishlist.Id == id);

        if (wishlist == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Wishlists", Controller = "Wishlists", Action = "Index" },
            new BreadcrumbItemViewModel { Label = $"Wishlist #{wishlist.Id}", IsActive = true });

        return View(wishlist);
    }
}
