using System.Net;
using System.Net.Http.Json;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class WishlistsApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public WishlistsApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetWishlists_Anonymous_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/wishlists");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetWishlistById_WithValidId_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/wishlists/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetWishlistById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/wishlists/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostWishlist_Anonymous_ReturnsUnauthorized()
        {
            var newWishlist = new WishlistDTO { PokemonCardId = 1, MaxPrice = 100.00m, Priority = 1 };
            var response = await _client.PostAsJsonAsync("/api/wishlists", newWishlist);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostWishlist_WithValidData_ReturnsCreated()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newWishlist = new WishlistDTO 
            { 
                PokemonCardId = 1,
                UserId = 1,
                MaxPrice = 100.00m,
                Priority = 1,
                AddedDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/wishlists", newWishlist);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PutWishlist_Anonymous_ReturnsUnauthorized()
        {
            var updated = new WishlistDTO { PokemonCardId = 1, MaxPrice = 120.00m, Priority = 2 };
            var response = await _client.PutAsJsonAsync("/api/wishlists/1", updated);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutWishlist_WithValidId_ReturnsOk()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new WishlistDTO 
            { 
                Id = 1,
                PokemonCardId = 1,
                UserId = 1,
                MaxPrice = 120.00m,
                Priority = 2,
                AddedDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/wishlists/1", updated);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutWishlist_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new WishlistDTO 
            { 
                Id = 999,
                PokemonCardId = 1,
                UserId = 1,
                MaxPrice = 100.00m,
                Priority = 1,
                AddedDate = DateTime.UtcNow
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/wishlists/999", updated);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteWishlist_Anonymous_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/wishlists/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteWishlist_WithValidId_ReturnsNoContent()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/wishlists/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteWishlist_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/wishlists/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
