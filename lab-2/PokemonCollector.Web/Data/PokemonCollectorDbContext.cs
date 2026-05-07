using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.Data;

/// <summary>
/// Entity Framework DbContext za PokemonCollector aplikaciju.
/// Upravlja konekcijom na bazu podataka i omogućava rad s entitetima.
/// </summary>
public class PokemonCollectorDbContext : DbContext
{
    public PokemonCollectorDbContext() { }

    public PokemonCollectorDbContext(DbContextOptions<PokemonCollectorDbContext> options) 
        : base(options)
    {
    }

    // DbSet za sve entitete
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<CardSet> CardSets { get; set; } = null!;
    public DbSet<PokemonCard> PokemonCards { get; set; } = null!;
    public DbSet<Collection> Collections { get; set; } = null!;
    public DbSet<CardInstance> CardInstances { get; set; } = null!;
    public DbSet<Trade> Trades { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguracija za Trade - zbog dva stranog ključa prema User
        modelBuilder.Entity<Trade>()
            .HasOne(t => t.Sender)
            .WithMany(u => u.SentTrades)
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trade>()
            .HasOne(t => t.Receiver)
            .WithMany(u => u.ReceivedTrades)
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Osiguraj da je u bazi pravilno postavljen odnos između entiteta
        modelBuilder.Entity<CardInstance>()
            .HasOne(ci => ci.Collection)
            .WithMany(c => c.CardInstances)
            .HasForeignKey(ci => ci.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CardInstance>()
            .HasOne(ci => ci.PokemonCard)
            .WithMany(pc => pc.CardInstances)
            .HasForeignKey(ci => ci.PokemonCardId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.PokemonCard)
            .WithMany(pc => pc.Wishlists)
            .HasForeignKey(w => w.PokemonCardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
