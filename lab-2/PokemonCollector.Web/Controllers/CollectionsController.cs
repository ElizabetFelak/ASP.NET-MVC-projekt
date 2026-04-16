using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class CollectionsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public CollectionsController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Collections", IsActive = true });

        return View(_repository.GetCollections());
    }

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
