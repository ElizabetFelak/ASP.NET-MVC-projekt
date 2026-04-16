using System.Collections.Generic;
using System.Linq;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.Data;

public class MockPokemonRepository : IPokemonRepository
{
    private readonly IReadOnlyList<User> _users;
    private readonly IReadOnlyList<CardSet> _cardSets;
    private readonly IReadOnlyList<PokemonCard> _pokemonCards;
    private readonly IReadOnlyList<Collection> _collections;
    private readonly IReadOnlyList<CardInstance> _cardInstances;
    private readonly IReadOnlyList<Trade> _trades;
    private readonly IReadOnlyList<Wishlist> _wishlists;

    public MockPokemonRepository()
    {
        var data = MockDataFactory.Build();
        _users = data.Users;
        _cardSets = data.CardSets;
        _pokemonCards = data.PokemonCards;
        _collections = data.Collections;
        _cardInstances = data.CardInstances;
        _trades = data.Trades;
        _wishlists = data.Wishlists;
    }

    public IReadOnlyList<User> GetUsers() => _users;
    public User? GetUserById(int id) => _users.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<CardSet> GetCardSets() => _cardSets;
    public CardSet? GetCardSetById(int id) => _cardSets.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<PokemonCard> GetPokemonCards() => _pokemonCards;
    public PokemonCard? GetPokemonCardById(int id) => _pokemonCards.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<Collection> GetCollections() => _collections;
    public Collection? GetCollectionById(int id) => _collections.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<CardInstance> GetCardInstances() => _cardInstances;
    public CardInstance? GetCardInstanceById(int id) => _cardInstances.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<Trade> GetTrades() => _trades;
    public Trade? GetTradeById(int id) => _trades.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<Wishlist> GetWishlists() => _wishlists;
    public Wishlist? GetWishlistById(int id) => _wishlists.FirstOrDefault(x => x.Id == id);
}
