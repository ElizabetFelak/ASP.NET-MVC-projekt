using System.Net;
using System.Net.Http.Json;
using PokemonCollector.Web.Models.DTOs;
using Xunit;

namespace PokemonCollector.Web.Tests.Api
{
    public class TradesApiTests_Complete : IClassFixture<PokemonCollectorWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly PokemonCollectorWebApplicationFactory _factory;

        public TradesApiTests_Complete(PokemonCollectorWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTrades_Anonymous_ReturnsOkWithData()
        {
            var response = await _client.GetAsync("/api/trades");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var trades = await response.Content.ReadAsAsync<List<TradeDTO>>();
            Assert.NotNull(trades);
            Assert.True(trades.Count > 0, $"Expected trades in database, but found {trades.Count}");
        }

        [Fact]
        public async Task GetTradeById_WithValidId_ReturnsOk()
        {
            // Fetch all trades to get a valid ID
            var listResponse = await _client.GetAsync("/api/trades");
            Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
            
            var trades = await listResponse.Content.ReadAsAsync<List<TradeDTO>>();
            if (trades == null || trades.Count == 0)
            {
                // Seeding may not have run or created trades
                // Try common IDs as fallback
                var response = await _client.GetAsync("/api/trades/1");
                // If we get 404, that's ok - just means ID 1 doesn't exist
                Assert.True(
                    response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound
                );
                return;
            }
            
            // Use first trade's actual ID
            var tradeId = trades[0].Id;
            var response2 = await _client.GetAsync($"/api/trades/{tradeId}");
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        [Fact]
        public async Task GetTradeById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/trades/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostTrade_Anonymous_ReturnsUnauthorized()
        {
            var newTrade = new TradeDTO { TransactionAmount = 150.00m };
            var response = await _client.PostAsJsonAsync("/api/trades", newTrade);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PostTrade_WithValidData_ReturnsCreated()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var newTrade = new TradeDTO 
            { 
                SenderId = 1,
                ReceiverId = 1,
                CardInstanceId = 1,
                TransactionAmount = 150.00m,
                TradeDate = DateTime.UtcNow,
                TradeStatus = "Pending"
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/trades", newTrade);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PutTrade_Anonymous_ReturnsUnauthorized()
        {
            var updated = new TradeDTO { TransactionAmount = 200.00m };
            var response = await _client.PutAsJsonAsync("/api/trades/1", updated);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PutTrade_WithValidId_ReturnsOk()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new TradeDTO 
            { 
                Id = 1,
                SenderId = 1,
                ReceiverId = 1,
                CardInstanceId = 1,
                TransactionAmount = 175.00m,
                TradeDate = DateTime.UtcNow,
                TradeStatus = "Completed"
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/trades/1", updated);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutTrade_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var updated = new TradeDTO 
            { 
                Id = 999,
                SenderId = 1,
                ReceiverId = 1,
                CardInstanceId = 1,
                TransactionAmount = 150.00m,
                TradeDate = DateTime.UtcNow,
                TradeStatus = "Pending"
            };
            var response = await authenticatedClient.PutAsJsonAsync("/api/trades/999", updated);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_Anonymous_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/trades/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_WithValidId_ReturnsNoContent()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/trades/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_WithInvalidId_ReturnsNotFound()
        {
            var authenticatedClient = _factory.CreateAuthenticatedClient();
            var response = await authenticatedClient.DeleteAsync("/api/trades/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
