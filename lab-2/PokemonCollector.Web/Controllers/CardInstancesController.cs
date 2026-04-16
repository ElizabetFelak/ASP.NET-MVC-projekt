using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class CardInstancesController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public CardInstancesController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Instances", IsActive = true });

        return View(_repository.GetCardInstances());
    }

    public IActionResult Details(int id)
    {
        var instance = _repository.GetCardInstanceById(id);
        if (instance == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Card Instances", Controller = "CardInstances", Action = "Index" },
            new BreadcrumbItemViewModel { Label = $"Instance #{instance.Id}", IsActive = true });

        return View(instance);
    }
}
