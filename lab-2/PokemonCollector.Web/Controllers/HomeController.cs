using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class HomeController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public HomeController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(new BreadcrumbItemViewModel { Label = "Home", IsActive = true });

        var model = new HomeIndexViewModel
        {
            TopCollections = _repository.GetCollections()
                .OrderByDescending(x => x.CollectionValue)
                .Take(3)
                .ToList(),
            TopWishlistItems = _repository.GetWishlists()
                .OrderBy(x => x.Priority)
                .ThenByDescending(x => x.MaxPrice)
                .Take(3)
                .ToList(),
            LatestTrades = _repository.GetTrades()
                .OrderByDescending(x => x.TradeDate)
                .Take(3)
                .ToList()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
