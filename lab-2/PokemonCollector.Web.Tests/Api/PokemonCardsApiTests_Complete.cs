using System.Net;
using System.Net.Http.Json;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class PokemonCardsApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public PokemonCardsApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPokemonCards_Anonymous_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/pokemoncards");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cards = await response.Content.ReadAsAsync<List<PokemonCardDTO>>();
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
        }

        [Fact]
        public async Task GetPokemonCardById_WithValidId_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/pokemoncards/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var card = await response.Content.ReadAsAsync<PokemonCardDTO>();
            Assert.NotNull(card);
        }

        [Fact]
        public async Task GetPokemonCardById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/pokemoncards/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostPokemonCard_Anonymous_ReturnsUnauthorized()
        {
            var newCard = new PokemonCardDTO
            {
                CardName = "Test Card",
                PokemonNumber = 100,
                MarketPrice = 25.00m,
                CardSetId = 1
            };

            var response = await _client.PostAsJsonAsync("/api/pokemoncards", newCard);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostPokemonCard_WithValidData_ReturnsCreated()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newCard = new PokemonCardDTO
            {
                CardName = "New Card",
                PokemonNumber = 150,
                MarketPrice = 30.00m,
                CardSetId = 1,
                Type = PokemonType.Colorless,
                Rarity = CardRarity.Rare,
                CreatedDate = DateTime.UtcNow
            };

            var response = await authenticatedClient.PostAsJsonAsync("/api/pokemoncards", newCard);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadAsAsync<PokemonCardDTO>();
            Assert.NotNull(created);
            Assert.Equal("New Card", created.CardName);
        }

        [Fact]
        public async Task PutPokemonCard_Anonymous_ReturnsUnauthorized()
        {
            var updated = new PokemonCardDTO { CardName = "Updated", MarketPrice = 40.00m };
            var response = await _client.PutAsJsonAsync("/api/pokemoncards/1", updated);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutPokemonCard_WithValidId_ReturnsOk()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new PokemonCardDTO 
            { 
                Id = 1,
                CardName = "Updated Card",
                PokemonNumber = 25,
                Type = PokemonType.Electric,
                Rarity = CardRarity.Uncommon,
                MarketPrice = 45.00m,
                CardSetId = 1,
                CreatedDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/pokemoncards/1", updated);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutPokemonCard_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new PokemonCardDTO 
            { 
                Id = 999,
                CardName = "Updated",
                PokemonNumber = 100,
                Type = PokemonType.Colorless,
                Rarity = CardRarity.Common,
                MarketPrice = 40.00m,
                CardSetId = 1,
                CreatedDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/pokemoncards/999", updated);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeletePokemonCard_Anonymous_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/pokemoncards/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeletePokemonCard_WithValidId_ReturnsNoContent()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/pokemoncards/2");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync("/api/pokemoncards/2");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeletePokemonCard_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/pokemoncards/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
