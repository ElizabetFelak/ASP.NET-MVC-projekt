using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("setovi")]
public class CardSetsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public CardSetsController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Sets", IsActive = true });

        return View(_repository.GetCardSets());
    }

    [Route("{id:int}")]
    [Route("{id:int}/pregledaj")]
    public IActionResult Details(int id)
    {
        var set = _repository.GetCardSetById(id);
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
}
