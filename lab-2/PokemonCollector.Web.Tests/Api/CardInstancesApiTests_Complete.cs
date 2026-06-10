using System.Net;
using System.Net.Http.Json;
using PokemonCollector.Web.Models;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class CardInstancesApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public CardInstancesApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCardInstances_Anonymous_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/cardinstances");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCardInstanceById_WithValidId_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/cardinstances/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCardInstanceById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/cardinstances/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostCardInstance_Anonymous_ReturnsUnauthorized()
        {
            var newInstance = new CardInstanceDTO { PokemonCardId = 1, CollectionId = 1, CurrentValue = 50.00m };
            var response = await _client.PostAsJsonAsync("/api/cardinstances", newInstance);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostCardInstance_WithValidData_ReturnsCreated()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newInstance = new CardInstanceDTO 
            { 
                PokemonCardId = 1,
                CollectionId = 1,
                CurrentValue = 50.00m,
                Condition = CardCondition.NearMint,
                Quantity = 1,
                AcquisitionDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/cardinstances", newInstance);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PutCardInstance_Anonymous_ReturnsUnauthorized()
        {
            var updated = new CardInstanceDTO { PokemonCardId = 1, CurrentValue = 60.00m };
            var response = await _client.PutAsJsonAsync("/api/cardinstances/1", updated);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutCardInstance_WithValidId_ReturnsOk()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new CardInstanceDTO 
            { 
                Id = 1,
                PokemonCardId = 1,
                CollectionId = 1,
                CurrentValue = 60.00m,
                Condition = CardCondition.Mint,
                Quantity = 2,
                AcquisitionDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/cardinstances/1", updated);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutCardInstance_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new CardInstanceDTO 
            { 
                Id = 999,
                PokemonCardId = 1,
                CollectionId = 1,
                CurrentValue = 60.00m,
                Condition = CardCondition.NearMint,
                Quantity = 1,
                AcquisitionDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/cardinstances/999", updated);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCardInstance_Anonymous_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/cardinstances/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCardInstance_WithValidId_ReturnsNoContent()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/cardinstances/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCardInstance_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/cardinstances/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
