using System.Collections.Generic;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.Data;

public interface IPokemonRepository
{
    IReadOnlyList<User> GetUsers();
    User? GetUserById(int id);

    IReadOnlyList<CardSet> GetCardSets();
    CardSet? GetCardSetById(int id);

    IReadOnlyList<PokemonCard> GetPokemonCards();
    PokemonCard? GetPokemonCardById(int id);

    IReadOnlyList<Collection> GetCollections();
    Collection? GetCollectionById(int id);

    IReadOnlyList<CardInstance> GetCardInstances();
    CardInstance? GetCardInstanceById(int id);

    IReadOnlyList<Trade> GetTrades();
    Trade? GetTradeById(int id);

    IReadOnlyList<Wishlist> GetWishlists();
    Wishlist? GetWishlistById(int id);
}
