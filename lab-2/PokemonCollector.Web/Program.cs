using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Konfiguracija Entity Framework Core s SQL Server bazom
var connectionString = builder.Configuration.GetConnectionString("PokemonCollectorDb");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<PokemonCollectorDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    // Fallback to in-memory database for local development when no connection string is configured
    builder.Services.AddDbContext<PokemonCollectorDbContext>(options =>
        options.UseInMemoryDatabase("PokemonCollectorDev"));
}

// Repository - za sada mock, kasnije ce biti EF repository
builder.Services.AddSingleton<IPokemonRepository, MockPokemonRepository>();

var app = builder.Build();

// Seed sample data when using in-memory database to allow immediate testing
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PokemonCollectorDbContext>();
    if (!db.CardSets.Any())
    {
        db.CardSets.AddRange(new[] {
            new PokemonCollector.Web.Models.CardSet { SetName = "Base Set", ReleaseDate = DateTime.Parse("1999-01-09"), TotalCards = 102, Publisher = "Wizards" , SetCode = "BS" , SetSymbol = "BS" },
            new PokemonCollector.Web.Models.CardSet { SetName = "Jungle", ReleaseDate = DateTime.Parse("1999-06-16"), TotalCards = 64, Publisher = "Wizards" , SetCode = "JU" , SetSymbol = "JU" },
            new PokemonCollector.Web.Models.CardSet { SetName = "Fossil", ReleaseDate = DateTime.Parse("1999-10-10"), TotalCards = 62, Publisher = "Wizards" , SetCode = "FO" , SetSymbol = "FO" }
        });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/greska");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
// Configure request localization to support hr and en formats for dates
var supportedCultures = new[] { new CultureInfo("hr-HR"), new CultureInfo("en-US") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
