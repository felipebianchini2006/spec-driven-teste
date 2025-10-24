using Microsoft.EntityFrameworkCore;
using EstanteVirtual.Data.Models;
using EstanteVirtual.Data.Data.EntityConfigurations;

namespace EstanteVirtual.Data.Data;

/// <summary>
/// Application database context for Estante Virtual MVP.
/// Manages Books and Reviews entities with PostgreSQL backend.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Books collection - represents cataloged books in user's shelf.
    /// </summary>
    public DbSet<Book> Books { get; set; } = null!;

    /// <summary>
    /// Reviews collection - represents user reviews for books (1:1 relationship).
    /// </summary>
    public DbSet<Review> Reviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations using Fluent API
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        // modelBuilder.ApplyConfiguration(new ReviewConfiguration()); // Will be added in T081
    }
}
