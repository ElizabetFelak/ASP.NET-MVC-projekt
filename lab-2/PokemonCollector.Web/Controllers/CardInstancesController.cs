using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("[controller]")]
public class CardInstancesController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public CardInstancesController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var collections = await _dbContext.Collections.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Collections = collections;
        ViewBag.Cards = cards;
        var model = new CardInstance { AcquisitionDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(CardInstance model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.CardInstances.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var collections = await _dbContext.Collections.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Collections = collections;
        ViewBag.Cards = cards;
        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();
        
        var collections = await _dbContext.Collections.ToListAsync();
        var cards = await _dbContext.PokemonCards.ToListAsync();
        ViewBag.Collections = collections;
        ViewBag.Cards = cards;
        
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();

        var ok = await TryUpdateModelAsync<CardInstance>(item, "",
            c => c.CollectionId,
            c => c.PokemonCardId,
            c => c.Condition,
            c => c.Quantity,
            c => c.AcquisitionDate,
            c => c.CurrentValue);

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
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.CardInstances.Remove(item);
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
            new BreadcrumbItemViewModel { Label = "Card Instances", IsActive = true });

        var instances = _dbContext.CardInstances
            .AsNoTracking()
            .Include(instance => instance.Collection)
            .Include(instance => instance.PokemonCard)
            .OrderBy(instance => instance.Id)
            .ToList();

        return View(instances);
    }

    [HttpGet("/CardInstances/search")]
    [AllowAnonymous]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.CardInstances
            .AsNoTracking()
            .Include(i => i.Collection)
            .Include(i => i.PokemonCard)
            .Where(i => string.IsNullOrEmpty(query)
                || (i.PokemonCard != null && EF.Functions.Like(i.PokemonCard.CardName, $"%{query}%"))
                || (i.Collection != null && EF.Functions.Like(i.Collection.CollectionName, $"%{query}%")))
            .OrderBy(i => i.Id)
            .Select(i => new {
                id = i.Id,
                pokemonCardName = i.PokemonCard != null ? i.PokemonCard.CardName : string.Empty,
                collectionName = i.Collection != null ? i.Collection.CollectionName : string.Empty,
                condition = i.Condition.ToString(),
                quantity = i.Quantity,
                currentValue = i.CurrentValue
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}/details")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var instance = await _dbContext.CardInstances
            .AsNoTracking()
            .Include(cardInstance => cardInstance.PokemonCard)
            .Include(cardInstance => cardInstance.Collection)
            .FirstOrDefaultAsync(cardInstance => cardInstance.Id == id);

        if (instance == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Instances", Controller = "CardInstances", Action = "Index" },
            new BreadcrumbItemViewModel { Label = $"Instance #{instance.Id}", IsActive = true });

        return View(instance);
    }
}
