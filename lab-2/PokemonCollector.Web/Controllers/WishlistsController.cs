using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public class WishlistsController : AppControllerBase
{
    private readonly IPokemonRepository _repository;

    public WishlistsController(IPokemonRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Wishlists", IsActive = true });

        return View(_repository.GetWishlists());
    }

    public IActionResult Details(int id)
    {
        var wishlist = _repository.GetWishlistById(id);
        if (wishlist == null)
        {
            return NotFound();
        }

        SetBreadcrumbs(
            new BreadcrumbItemViewModel { Label = "Home", Controller = "Home", Action = "Index" },
            new BreadcrumbItemViewModel { Label = "Wishlists", Controller = "Wishlists", Action = "Index" },
            new BreadcrumbItemViewModel { Label = $"Wishlist #{wishlist.Id}", IsActive = true });

        return View(wishlist);
    }
}
