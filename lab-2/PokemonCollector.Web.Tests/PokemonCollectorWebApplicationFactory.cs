using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PokemonCollector.Web;
using PokemonCollector.Web.Data;
using PokemonCollector.Web.Models;
using System.Net.Http.Json;

namespace PokemonCollector.Web.Tests
{
    public class PokemonCollectorWebApplicationFactory : WebApplicationFactory<Program>
    {
        private string _dbName = "";
        private string _testUserId = "";
        private const string TestUserEmail = "testuser@example.com";
        private const string TestUserPassword = "TestPassword123!";
        private const string TestUserOIB = "12345678901";
        private const string TestUserJMBG = "1234567890123";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _dbName = "InMemoryDbForTesting" + Guid.NewGuid().ToString();
            builder.ConfigureServices(services =>
            {
                // Remove the original DbContext registrations
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<PokemonCollectorDbContext>)).ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Add DbContext using an in-memory database for testing
                services.AddDbContext<PokemonCollectorDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });
            });

            builder.ConfigureAppConfiguration((context, config) => { });

            // Seed the database after the application is built
            builder.ConfigureTestServices(services =>
            {
                // Remove SQL Server EF Core provider
                var descriptors = services.Where(
                    d => d.ServiceType.Name.Contains("EntityFramework") ||
                         d.ServiceType.Name.Contains("DbContext") ||
                         d.ImplementationType?.Name.Contains("SqlServer") == true).ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Also explicitly remove the main DbContextOptions
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PokemonCollectorDbContext>));
                
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Add InMemory database
                services.AddDbContext<PokemonCollectorDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Add test authentication scheme that recognizes X-Test-User header
                services.AddAuthentication("TestScheme")
                    .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationSchemeHandler>(
                        "TestScheme", options => { });

                // Seed database
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<PokemonCollectorDbContext>();
                    db.Database.EnsureCreated();
                    SeedTestData(db, scope.ServiceProvider).Wait();
                }
            });
        }

        private async Task SeedTestData(PokemonCollectorDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                // Clear existing data
                context.CardSets.RemoveRange(context.CardSets);
                context.PokemonCards.RemoveRange(context.PokemonCards);
                context.Collections.RemoveRange(context.Collections);
                context.CardInstances.RemoveRange(context.CardInstances);
                context.Trades.RemoveRange(context.Trades);
                context.Wishlists.RemoveRange(context.Wishlists);
                context.DomainUsers.RemoveRange(context.DomainUsers);
                context.SaveChanges();

                // Create test user in Identity
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var testUser = new AppUser
                {
                    UserName = TestUserEmail,
                    Email = TestUserEmail,
                    OIB = TestUserOIB,
                    JMBG = TestUserJMBG
                };
                await userManager.CreateAsync(testUser, TestUserPassword);
                _testUserId = testUser.Id;

                // Create corresponding domain User entity
                var domainUser = new User
                {
                    Username = TestUserEmail,
                    Email = TestUserEmail,
                    RegistrationDate = DateTime.UtcNow,
                    Budget = 1000m
                };
                context.DomainUsers.Add(domainUser);
                context.SaveChanges();
                int domainUserId = domainUser.Id;
                System.Diagnostics.Debug.WriteLine($"[SEED] Domain User created with ID: {domainUserId}");

                // Add seed data for CardSets
                var cardSet1 = new CardSet
                {
                    SetName = "Test Set",
                    ReleaseDate = DateTime.UtcNow,
                    TotalCards = 100,
                    Publisher = "Test Publisher",
                    SetSymbol = "TS",
                    SetCode = "TEST"
                };
                var cardSet2 = new CardSet
                {
                    SetName = "Second Set",
                    ReleaseDate = DateTime.UtcNow.AddDays(-30),
                    TotalCards = 50,
                    Publisher = "Second Publisher",
                    SetSymbol = "SS",
                    SetCode = "SEC2"
                };
                context.CardSets.AddRange(cardSet1, cardSet2);
                context.SaveChanges();

                // Add seed data for PokemonCards
                var card1 = new PokemonCard
                {
                    CardName = "Test Card",
                    PokemonNumber = 1,
                    Type = PokemonType.Fire,
                    Rarity = CardRarity.Common,
                    MarketPrice = 10.00m,
                    CardSetId = cardSet1.Id,
                    CreatedDate = DateTime.UtcNow
                };
                var card2 = new PokemonCard
                {
                    CardName = "Rare Card",
                    PokemonNumber = 25,
                    Type = PokemonType.Electric,
                    Rarity = CardRarity.Rare,
                    MarketPrice = 50.00m,
                    CardSetId = cardSet1.Id,
                    CreatedDate = DateTime.UtcNow
                };
                context.PokemonCards.AddRange(card1, card2);
                context.SaveChanges();

                // Add seed data for Collections
                var collection = new Collection
                {
                    CollectionName = "Test Collection",
                    UserId = domainUserId,
                    CollectionValue = 500.00m,
                    CreatedDate = DateTime.UtcNow
                };
                context.Collections.Add(collection);
                context.SaveChanges();

                // Add seed data for CardInstances
                var instance = new CardInstance
                {
                    PokemonCardId = card1.Id,
                    CollectionId = collection.Id,
                    CurrentValue = 15.00m,
                    Condition = CardCondition.NearMint,
                    AcquisitionDate = DateTime.UtcNow.AddDays(-10),
                    Quantity = 1
                };
                context.CardInstances.Add(instance);
                context.SaveChanges();

                // Add seed data for Trades
                var trade = new Trade
                {
                    SenderId = domainUserId,
                    ReceiverId = domainUserId,
                    CardInstanceId = instance.Id,
                    TransactionAmount = 100.00m,
                    TradeDate = DateTime.UtcNow,
                    TradeStatus = "Completed"
                };
                context.Trades.Add(trade);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"[SEED] Trade created with ID: {trade.Id}, SenderId: {trade.SenderId}, ReceiverId: {trade.ReceiverId}, CardInstanceId: {trade.CardInstanceId}");

                // Add seed data for Wishlists
                var wishlist = new Wishlist
                {
                    PokemonCardId = card2.Id,
                    UserId = domainUserId,
                    MaxPrice = 75.00m,
                    Priority = 1,
                    AddedDate = DateTime.UtcNow
                };
                context.Wishlists.Add(wishlist);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SEED-ERROR] Seeding error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SEED-ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Creates an authenticated HttpClient with a valid user token
        /// </summary>
        public HttpClient CreateAuthenticatedClient()
        {
            var client = CreateClient();
            // Add test user header for authentication
            client.DefaultRequestHeaders.Add("X-Test-User", _testUserId);
            return client;
        }

        /// <summary>
        /// Gets the test user ID created during seeding
        /// </summary>
        public string GetTestUserId() => _testUserId;

        /// <summary>
        /// Gets the test user email
        /// </summary>
        public string GetTestUserEmail() => TestUserEmail;

        /// <summary>
        /// Gets the test user password
        /// </summary>
        public string GetTestUserPassword() => TestUserPassword;
    }

    // Test authentication scheme for integration tests
    public class TestAuthenticationSchemeOptions : Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions
    {
    }

    public class TestAuthenticationSchemeHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthenticationSchemeHandler(
            IOptionsMonitor<TestAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override System.Threading.Tasks.Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check for test user header
            if (!Request.Headers.TryGetValue("X-Test-User", out var userIdValue))
            {
                return System.Threading.Tasks.Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.NoResult());
            }

            var userId = userIdValue.ToString();
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, "testuser@example.com")
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestScheme");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var ticket = new Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "TestScheme");

            return System.Threading.Tasks.Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.Success(ticket));
        }
    }
}
