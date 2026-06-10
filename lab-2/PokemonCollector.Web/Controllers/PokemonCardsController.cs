using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace PokemonCollector.Web.Controllers;

[Route("karte")]
public class PokemonCardsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public PokemonCardsController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
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
            new BreadcrumbItemViewModel { Label = "Pokemon Cards", IsActive = true });

        var cards = _dbContext.PokemonCards
            .AsNoTracking()
            .Include(card => card.CardSet)
            .OrderBy(card => card.CardName)
            .ToList();

        return View(cards);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.PokemonCards
            .AsNoTracking()
            .Include(c => c.CardSet)
            .Where(c => string.IsNullOrEmpty(query)
                || EF.Functions.Like(c.CardName, $"%{query}%")
                || (c.CardSet != null && EF.Functions.Like(c.CardSet.SetName, $"%{query}%")))
            .OrderBy(c => c.CardName)
            .Select(c => new {
                id = c.Id,
                cardName = c.CardName,
                pokemonNumber = c.PokemonNumber,
                type = c.Type.ToString(),
                rarity = c.Rarity.ToString(),
                marketPrice = c.MarketPrice,
                setName = c.CardSet != null ? c.CardSet.SetName : string.Empty
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var card = await _dbContext.PokemonCards
            .AsNoTracking()
            .Include(pokemonCard => pokemonCard.CardSet)
            .Include(pokemonCard => pokemonCard.CardInstances)
                .ThenInclude(cardInstance => cardInstance.Collection)
            .Include(pokemonCard => pokemonCard.Attachments)
            .FirstOrDefaultAsync(pokemonCard => pokemonCard.Id == id);

        if (card == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Pokemon Cards", Controller = "PokemonCards", Action = "Index" },
            new BreadcrumbItemViewModel { Label = card.CardName, IsActive = true });

        return View(card);
    }

    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var card = await _dbContext.PokemonCards
            .AsNoTracking()
            .Include(c => c.CardSet)
            .Include(c => c.CardInstances)
            .Include(c => c.Wishlists)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound();
        }

        return View(card);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var card = await _dbContext.PokemonCards
            .Include(c => c.CardInstances)
            .Include(c => c.Wishlists)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null)
        {
            return NotFound();
        }

        if (card.CardInstances.Any() || card.Wishlists.Any())
        {
            ModelState.AddModelError(string.Empty, "This card cannot be deleted because it is used by related records.");
            return View("Delete", card);
        }

        _dbContext.PokemonCards.Remove(card);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Route("create")]
    [Authorize]
    public IActionResult Create()
    {
        var model = new PokemonCard { CreatedDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(PokemonCard model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.PokemonCards.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // provide selected text for autocomplete if CardSetId present
        if (model.CardSetId != 0)
        {
            var set = await _dbContext.CardSets.FindAsync(model.CardSetId);
            ViewData["SelectedCardSetText"] = set?.SetName ?? string.Empty;
        }
        else if (Request.HasFormContentType && Request.Form.ContainsKey("CardSetId_text"))
        {
            // preserve typed value when validation fails
            ViewData["SelectedCardSetText"] = Request.Form["CardSetId_text"].ToString();
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var card = await _dbContext.PokemonCards.FindAsync(id);
        if (card == null) return NotFound();
        var set = await _dbContext.CardSets.FindAsync(card.CardSetId);
        ViewData["SelectedCardSetText"] = set?.SetName ?? string.Empty;
        return View(card);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var card = await _dbContext.PokemonCards.FindAsync(id);
        if (card == null) return NotFound();

        var ok = await TryUpdateModelAsync<PokemonCard>(card, "",
            c => c.CardName,
            c => c.PokemonNumber,
            c => c.Type,
            c => c.Rarity,
            c => c.MarketPrice,
            c => c.CardSetId);

        if (ok && ModelState.IsValid)
        {
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var set = await _dbContext.CardSets.FindAsync(card.CardSetId);
        ViewData["SelectedCardSetText"] = set?.SetName ?? string.Empty;
        return View(card);
    }
}
