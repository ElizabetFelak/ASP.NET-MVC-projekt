using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class UsersController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public UsersController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Users", IsActive = true });

        return View(_repository.GetUsers());
    }

    public IActionResult Details(int id)
    {
        var user = _repository.GetUserById(id);
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
