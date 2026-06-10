using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("[controller]")]
public class TradesController : AppControllerBase
{
    private readonly IPokemonRepository _repository;
    private readonly PokemonCollectorDbContext _dbContext;

    public TradesController(IPokemonRepository repository, PokemonCollectorDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var users = await _dbContext.DomainUsers.ToListAsync();
        var cardInstances = await _dbContext.CardInstances
            .Include(ci => ci.PokemonCard)
            .Include(ci => ci.Collection)
            .ToListAsync();
        ViewBag.Users = users;
        ViewBag.CardInstances = cardInstances;
        var model = new Trade { TradeDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    [Authorize]
    public async Task<IActionResult> Create(Trade model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Trades.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var users = await _dbContext.DomainUsers.ToListAsync();
        var cardInstances = await _dbContext.CardInstances
            .Include(ci => ci.PokemonCard)
            .Include(ci => ci.Collection)
            .ToListAsync();
        ViewBag.Users = users;
        ViewBag.CardInstances = cardInstances;
        return View(model);
    }

    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();
        
        var users = await _dbContext.DomainUsers.ToListAsync();
        var cardInstances = await _dbContext.CardInstances
            .Include(ci => ci.PokemonCard)
            .Include(ci => ci.Collection)
            .ToListAsync();
        ViewBag.Users = users;
        ViewBag.CardInstances = cardInstances;
        
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();

        var ok = await TryUpdateModelAsync<Trade>(item, "",
            t => t.SenderId,
            t => t.ReceiverId,
            t => t.CardInstanceId,
            t => t.TradeDate,
            t => t.TransactionAmount,
            t => t.TradeStatus);

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
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.Trades.Remove(item);
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
            new BreadcrumbItemViewModel { Label = "Trades", IsActive = true });

        var trades = _dbContext.Trades
            .AsNoTracking()
            .Include(trade => trade.Sender)
            .Include(trade => trade.Receiver)
            .Include(trade => trade.CardInstance)
            .OrderByDescending(trade => trade.TradeDate)
            .ToList();

        return View(trades);
    }

    [HttpGet("/Trades/search")]
    [AllowAnonymous]
    public IActionResult Search(string q)
    {
        var query = q?.Trim() ?? string.Empty;
        var items = _dbContext.Trades
            .AsNoTracking()
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t => string.IsNullOrEmpty(query)
                || EF.Functions.Like(t.TradeStatus, $"%{query}%")
                || (t.Sender != null && EF.Functions.Like(t.Sender.Username, $"%{query}%"))
                || (t.Receiver != null && EF.Functions.Like(t.Receiver.Username, $"%{query}%")))
            .OrderByDescending(t => t.TradeDate)
            .Select(t => new {
                id = t.Id,
                senderName = t.Sender != null ? t.Sender.Username : string.Empty,
                receiverName = t.Receiver != null ? t.Receiver.Username : string.Empty,
                tradeStatus = t.TradeStatus,
                transactionAmount = t.TransactionAmount,
                tradeDate = t.TradeDate
            })
            .Take(10)
            .ToList();

        return Json(items);
    }

    [Route("{id:int}/details")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var trade = await _dbContext.Trades
            .AsNoTracking()
            .Include(existingTrade => existingTrade.Sender)
            .Include(existingTrade => existingTrade.Receiver)
            .Include(existingTrade => existingTrade.CardInstance)
            .FirstOrDefaultAsync(existingTrade => existingTrade.Id == id);

        if (trade == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Trades", Controller = "Trades", Action = "Index" },
            new BreadcrumbItemViewModel { Label = $"Trade #{trade.Id}", IsActive = true });

        return View(trade);
    }
}
