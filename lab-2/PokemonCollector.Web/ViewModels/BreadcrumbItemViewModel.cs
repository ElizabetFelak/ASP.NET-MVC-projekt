namespace PokemonCollector.Web.ViewModels;

public class BreadcrumbItemViewModel
{
    public string Label { get; set; } = string.Empty;
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public object? RouteValues { get; set; }
    public bool IsActive { get; set; }
}
