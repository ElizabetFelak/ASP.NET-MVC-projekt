using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var users = await _dbContext.DomainUsers.ToListAsync();
        ViewBag.Users = users;
        var model = new Collection { CreatedDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(Collection model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Collections.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var users = await _dbContext.DomainUsers.ToListAsync();
        ViewBag.Users = users;
        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();
        
        var users = await _dbContext.DomainUsers.ToListAsync();
        ViewBag.Users = users;
        
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _dbContext.Collections.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
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
    [AllowAnonymous]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Collections", IsActive = true });

        var collections = _dbContext.Collections
            .AsNoTracking()
            .Include(collection => collection.User)
            .OrderBy(collection => collection.CollectionName)
            .ToList();

        return View(collections);
    }

    [HttpGet("search")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var collection = await _dbContext.Collections
            .AsNoTracking()
            .Include(existingCollection => existingCollection.User)
            .Include(existingCollection => existingCollection.CardInstances)
                .ThenInclude(cardInstance => cardInstance.PokemonCard)
            .FirstOrDefaultAsync(existingCollection => existingCollection.Id == id);

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
