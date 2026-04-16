using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class TradesController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public TradesController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Trades", IsActive = true });

        return View(_repository.GetTrades());
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
