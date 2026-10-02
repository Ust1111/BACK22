using ArticulationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticulationAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<FavoritePair> FavoritePairs => Set<FavoritePair>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
        });

        b.Entity<FavoritePair>(e =>
        {
            e.HasKey(f => f.Id);
            e.HasOne(f => f.User)
             .WithMany(u => u.Favorites)
             .HasForeignKey(f => f.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}