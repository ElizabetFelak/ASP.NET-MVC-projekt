using System.Collections.Generic;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<Collection> TopCollections { get; set; } = new List<Collection>();
    public IReadOnlyList<Wishlist> TopWishlistItems { get; set; } = new List<Wishlist>();
    public IReadOnlyList<Trade> LatestTrades { get; set; } = new List<Trade>();
}
