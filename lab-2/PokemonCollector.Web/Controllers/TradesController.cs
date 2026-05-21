using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

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
    public IActionResult Create()
    {
        var model = new Trade { TradeDate = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(Trade model)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Trades.Add(model);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Route("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/edit")]
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
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _dbContext.Trades.FindAsync(id);
        if (item == null) return NotFound();

        _dbContext.Trades.Remove(item);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Trades", IsActive = true });

        return View(_repository.GetTrades());
    }

    [HttpGet("/Trades/search")]
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

    public IActionResult Details(int id)
    {
        var trade = _repository.GetTradeById(id);
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
