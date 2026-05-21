using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

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
    public IActionResult Create()
    {
        var model = new CardInstance { AcquisitionDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(CardInstance model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.CardInstances.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
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
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.CardInstances.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.CardInstances.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Instances", IsActive = true });

        return View(_repository.GetCardInstances());
    }

    [HttpGet("/CardInstances/search")]
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

    public IActionResult Details(int id)
    {
        var instance = _repository.GetCardInstanceById(id);
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
