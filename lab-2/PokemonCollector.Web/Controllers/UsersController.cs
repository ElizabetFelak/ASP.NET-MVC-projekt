using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("[controller]")]
public class UsersController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public UsersController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("create")]
    public IActionResult Create()
    {
        var model = new User { RegistrationDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(User model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.DomainUsers.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.DomainUsers.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var item = await _dbContext.DomainUsers.FindAsync(id);
        if (item == null) return NotFound();

        var ok = await TryUpdateModelAsync<User>(item, "",
            u => u.Username,
            u => u.Email,
            u => u.RegistrationDate,
            u => u.Budget,
            u => u.PhoneNumber,
            u => u.Address);

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
        var item = await _dbContext.DomainUsers.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.DomainUsers.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.DomainUsers.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Route("")]
    [Route("index")]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Users", IsActive = true });

        var users = _dbContext.DomainUsers
            .AsNoTracking()
            .Include(u => u.Collections)
            .OrderBy(u => u.Username)
            .ToList();

        return View(users);
    }

    [HttpGet("/Users/search")]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.DomainUsers
            .AsNoTracking()
            .Where(u => string.IsNullOrEmpty(query) || EF.Functions.Like(u.Username, $"%{query}%") || EF.Functions.Like(u.Email, $"%{query}%"))
            .OrderBy(u => u.Username)
            .Select(u => new {
                id = u.Id,
                username = u.Username,
                email = u.Email,
                budget = u.Budget,
                collections = u.Collections.Count,
                registrationDate = u.RegistrationDate
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _dbContext.DomainUsers
            .AsNoTracking()
            .Include(u => u.Collections)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Users", Controller = "Users", Action = "Index" },
            new BreadcrumbItemViewModel { Label = user.Username, IsActive = true });

        return View(user);
    }
}
