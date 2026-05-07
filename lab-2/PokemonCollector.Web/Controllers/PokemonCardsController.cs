using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

[Route("karte")]
public class PokemonCardsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public PokemonCardsController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Pokemon Cards", IsActive = true });

        return View(_repository.GetPokemonCards());
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public IActionResult Details(int id)
    {
        var card = _repository.GetPokemonCardById(id);
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
}
