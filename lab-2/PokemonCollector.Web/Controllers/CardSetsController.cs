using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace PokemonCollector.Web.Controllers;

[Route("setovi")]
public class CardSetsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public CardSetsController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("")]
    [Route("index")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Sets", IsActive = true });

        var sets = _dbContext.CardSets
            .AsNoTracking()
            .Include(set => set.Cards)
            .OrderBy(set => set.SetName)
            .ToList();

        return View(sets);
    }

    [Route("{id:int}")]
    [Route("{id:int}/pregledaj")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var set = await _dbContext.CardSets
            .AsNoTracking()
            .Include(cardSet => cardSet.Cards)
            .FirstOrDefaultAsync(cardSet => cardSet.Id == id);

        if (set == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Sets", Controller = "CardSets", Action = "Index" },
            new BreadcrumbItemViewModel { Label = set.SetName, IsActive = true });

        return View(set);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.CardSets
            .Where(c => string.IsNullOrEmpty(query) || EF.Functions.Like(c.SetName, $"%{query}%"))
            .OrderBy(c => c.SetName)
            .Select(c => new { id = c.Id, text = c.SetName })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("create")]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(CardSet model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.CardSets.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var set = await _dbContext.CardSets.FindAsync(id);
        if (set == null) return NotFound();
        return View(set);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var set = await _dbContext.CardSets.FindAsync(id);
        if (set == null) return NotFound();

        var ok = await TryUpdateModelAsync<CardSet>(set, "",
            s => s.SetName,
            s => s.ReleaseDate,
            s => s.TotalCards,
            s => s.Publisher,
            s => s.SetSymbol,
            s => s.SetCode);

        if (ok && ModelState.IsValid)
        {
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(set);
    }

    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var set = await _dbContext.CardSets.FindAsync(id);
        if (set == null) return NotFound();
        return View(set);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var set = await _dbContext.CardSets.FindAsync(id);
        if (set == null) return NotFound();

        _dbContext.CardSets.Remove(set);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
