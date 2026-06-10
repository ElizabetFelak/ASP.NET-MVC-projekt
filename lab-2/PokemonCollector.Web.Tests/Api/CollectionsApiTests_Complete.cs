using System.Net;
using System.Net.Http.Json;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class CollectionsApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public CollectionsApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCollections_Anonymous_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/collections");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCollectionById_WithValidId_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/collections/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCollectionById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/collections/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostCollection_Anonymous_ReturnsUnauthorized()
        {
            var newCollection = new CollectionDTO { CollectionName = "New Collection", CollectionValue = 500.00m };
            var response = await _client.PostAsJsonAsync("/api/collections", newCollection);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostCollection_WithValidData_ReturnsCreated()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newCollection = new CollectionDTO 
            { 
                CollectionName = "New Collection",
                UserId = 1,
                CollectionValue = 500.00m,
                CreatedDate = DateTime.UtcNow,
                Description = "",
                IsPublic = false
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/collections", newCollection);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PutCollection_Anonymous_ReturnsUnauthorized()
        {
            var updated = new CollectionDTO { CollectionName = "Updated Collection", CollectionValue = 600.00m };
            var response = await _client.PutAsJsonAsync("/api/collections/1", updated);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutCollection_WithValidId_ReturnsOk()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new CollectionDTO 
            { 
                Id = 1,
                CollectionName = "Updated Collection",
                UserId = 1,
                CollectionValue = 600.00m,
                CreatedDate = DateTime.UtcNow,
                Description = "Updated description",
                IsPublic = true
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/collections/1", updated);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutCollection_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new CollectionDTO 
            { 
                Id = 999,
                CollectionName = "Invalid",
                UserId = 1,
                CollectionValue = 500.00m,
                CreatedDate = DateTime.UtcNow,
                Description = "",
                IsPublic = false
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/collections/999", updated);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCollection_Anonymous_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/collections/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCollection_WithValidId_ReturnsNoContent()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/collections/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCollection_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/collections/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
