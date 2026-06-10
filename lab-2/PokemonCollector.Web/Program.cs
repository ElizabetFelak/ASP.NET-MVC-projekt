using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using PokemonCollector.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

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

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PokemonCollectorDbContext>();

var authBuilder = builder.Services.AddAuthentication();

// Only add Google OAuth if credentials are provided
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

// Only add Facebook OAuth if credentials are provided
var facebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
var facebookAppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
if (!string.IsNullOrEmpty(facebookAppId) && !string.IsNullOrEmpty(facebookAppSecret))
{
    authBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId;
        options.AppSecret = facebookAppSecret;
    });
}

// Register repository
builder.Services.AddScoped<IPokemonRepository, MockPokemonRepository>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed sample data when using in-memory database to allow immediate testing
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<PokemonCollectorDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    var roles = new[] { "Admin", "Collector" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed default domain user
    if (!db.DomainUsers.Any())
    {
        db.DomainUsers.Add(new PokemonCollector.Web.Models.User
        {
            Username = "collector",
            Email = "collector@example.com",
            RegistrationDate = DateTime.Now,
            Budget = 1000m,
            PhoneNumber = "+1-234-567-8900",
            Address = "123 Pokemon St, Collector City, CC 12345"
        });
        db.SaveChanges();
    }

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
app.UseStaticFiles();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
