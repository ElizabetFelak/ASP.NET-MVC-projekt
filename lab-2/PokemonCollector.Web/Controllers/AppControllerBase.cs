using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PokemonCollector.Web.ViewModels;

namespace PokemonCollector.Web.Controllers;

public abstract class AppControllerBase : Controller
{
    protected void SetBreadcrumbs(params BreadcrumbItemViewModel[] items)
    {
        ViewData["Breadcrumbs"] = new List<BreadcrumbItemViewModel>(items);
    }
}
