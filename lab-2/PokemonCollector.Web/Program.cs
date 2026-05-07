using PokemonCollector.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Konfiguracija Entity Framework Core s SQL Server bazom
var connectionString = builder.Configuration.GetConnectionString("PokemonCollectorDb");
builder.Services.AddDbContext<PokemonCollectorDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository - za sada mock, kasnije ce biti EF repository
builder.Services.AddSingleton<IPokemonRepository, MockPokemonRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/greska");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
