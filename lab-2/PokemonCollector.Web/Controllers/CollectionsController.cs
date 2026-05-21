using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("kolekcije")]
public class CollectionsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public CollectionsController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("create")]
    public IActionResult Create()
    {
        var model = new Collection { CreatedDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(Collection model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Collections.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();

        var ok = await TryUpdateModelAsync<Collection>(item, "",
            c => c.CollectionName,
            c => c.UserId,
            c => c.CreatedDate,
            c => c.CollectionValue,
            c => c.Description,
            c => c.IsPublic);

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
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.Collections.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Route("")]
    [Route("index")]
    [Route("sve")]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Collections", IsActive = true });

        return View(_repository.GetCollections());
    }

    [HttpGet("search")]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.Collections
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => string.IsNullOrEmpty(query) || EF.Functions.Like(c.CollectionName, $"%{query}%") || (c.User != null && EF.Functions.Like(c.User.Username, $"%{query}%")))
            .OrderBy(c => c.CollectionName)
            .Select(c => new {
                id = c.Id,
                collectionName = c.CollectionName,
                ownerName = c.User != null ? c.User.Username : string.Empty,
                isPublic = c.IsPublic,
                collectionValue = c.CollectionValue,
                createdDate = c.CreatedDate
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}")]
    [Route("{id:int}/detalji")]
    public IActionResult Details(int id)
    {
        var collection = _repository.GetCollectionById(id);
        if (collection == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Collections", Controller = "Collections", Action = "Index" },
            new BreadcrumbItemViewModel { Label = collection.CollectionName, IsActive = true });

        return View(collection);
    }
}
