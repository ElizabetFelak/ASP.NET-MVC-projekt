using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class CardSetsApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public CardSetsApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        #region GET All Tests

        [Fact]
        public async Task GetCardSets_Anonymous_ReturnsOkWithData()
        {
            // Act
            var response = await _client.GetAsync("/api/cardsets");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cardSets = await response.Content.ReadAsAsync<List<CardSetDTO>>();
            Assert.NotNull(cardSets);
            Assert.NotEmpty(cardSets);
        }

        #endregion

        #region GET By ID Tests

        [Fact]
        public async Task GetCardSetById_WithValidId_ReturnsOkWithData()
        {
            // Act
            var response = await _client.GetAsync("/api/cardsets/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cardSet = await response.Content.ReadAsAsync<CardSetDTO>();
            Assert.NotNull(cardSet);
            Assert.Equal("Test Set", cardSet.SetName);
        }

        [Fact]
        public async Task GetCardSetById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/cardsets/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task PostCardSet_Anonymous_ReturnsUnauthorized()
        {
            // Arrange
            var newCardSet = new CardSetDTO
            {
                SetName = "New Set",
                ReleaseDate = DateTime.UtcNow,
                TotalCards = 50,
                Publisher = "New Publisher",
                SetSymbol = "NS",
                SetCode = "NEW"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/cardsets", newCardSet);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostCardSet_WithValidData_ReturnsCreated()
        {
            // Arrange
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newCardSet = new CardSetDTO
            {
                SetName = "Brand New Set",
                ReleaseDate = DateTime.UtcNow,
                TotalCards = 75,
                Publisher = "New Publisher",
                SetSymbol = "BN",
                SetCode = "BRNEW"
            };

            // Act
            var response = await authenticatedClient.PostAsJsonAsync("/api/cardsets", newCardSet);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadAsAsync<CardSetDTO>();
            Assert.NotNull(created);
            Assert.Equal("Brand New Set", created.SetName);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task PutCardSet_Anonymous_ReturnsUnauthorized()
        {
            // Arrange
            var updatedCardSet = new CardSetDTO
            {
                SetName = "Updated Set",
                ReleaseDate = DateTime.UtcNow,
                TotalCards = 100,
                Publisher = "Updated",
                SetSymbol = "UP",
                SetCode = "UPD"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/cardsets/1", updatedCardSet);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutCardSet_WithValidId_ReturnsOkAndUpdates()
        {
            // Arrange
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updatedCardSet = new CardSetDTO
            {
                Id = 1,
                SetName = "Updated Test Set",
                ReleaseDate = DateTime.UtcNow,
                TotalCards = 150,
                Publisher = "Updated Publisher",
                SetSymbol = "UT",
                SetCode = "UPDT"
            };

            // Act
            var response = await authenticatedClient.PutAsJsonAsync("/api/cardsets/1", updatedCardSet);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutCardSet_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updatedCardSet = new CardSetDTO
            {
                Id = 999,
                SetName = "Update Attempt",
                ReleaseDate = DateTime.UtcNow,
                TotalCards = 50,
                Publisher = "Pub",
                SetSymbol = "UA",
                SetCode = "UAT"
            };

            // Act
            var response = await authenticatedClient.PutAsJsonAsync("/api/cardsets/999", updatedCardSet);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task DeleteCardSet_Anonymous_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/cardsets/1");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCardSet_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var authenticatedClient = _factory.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.DeleteAsync("/api/cardsets/2");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync("/api/cardsets/2");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteCardSet_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var authenticatedClient = _factory.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.DeleteAsync("/api/cardsets/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }

    // Helper extension for reading responses
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
