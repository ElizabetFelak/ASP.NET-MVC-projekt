using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

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
    public IActionResult Create()
    {
        var model = new Wishlist { AddedDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(Wishlist model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Wishlists.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
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
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.Wishlists.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.Wishlists.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Wishlists", IsActive = true });

        return View(_repository.GetWishlists());
    }

    [HttpGet("/Wishlists/search")]
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

    public IActionResult Details(int id)
    {
        var wishlist = _repository.GetWishlistById(id);
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
