using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PokemonCollector.Web.Models;

namespace PokemonCollector.Web.Data;

/// <summary>
/// Entity Framework DbContext za PokemonCollector aplikaciju.
/// Upravlja konekcijom na bazu podataka i omogućava rad s entitetima.
/// </summary>
public class PokemonCollectorDbContext : IdentityDbContext<AppUser>
{
    public PokemonCollectorDbContext(DbContextOptions<PokemonCollectorDbContext> options) 
        : base(options)
    {
    }

    // DbSet za sve entitete
    public DbSet<User> DomainUsers { get; set; } = null!;
    public DbSet<CardSet> CardSets { get; set; } = null!;
    public DbSet<PokemonCard> PokemonCards { get; set; } = null!;
    public DbSet<Collection> Collections { get; set; } = null!;
    public DbSet<CardInstance> CardInstances { get; set; } = null!;
    public DbSet<Trade> Trades { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; } = null!;

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

        // Konfiguracija za Attachment - CardSet veza (no cascade to avoid cycles)
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.CardSet)
            .WithMany(cs => cs.Attachments)
            .HasForeignKey(a => a.CardSetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Konfiguracija za Attachment - PokemonCard veza
        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.PokemonCard)
            .WithMany(pc => pc.Attachments)
            .HasForeignKey(a => a.PokemonCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Konfiguracija za CardSet - PokemonCard veza
        modelBuilder.Entity<PokemonCard>()
            .HasOne(pc => pc.CardSet)
            .WithMany(cs => cs.Cards)
            .HasForeignKey(pc => pc.CardSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
