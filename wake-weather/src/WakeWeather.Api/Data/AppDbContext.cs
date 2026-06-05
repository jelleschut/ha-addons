using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WakeWeather.Api.Models;
using WakeWeather.Core.Models;

namespace WakeWeather.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<OpeningPeriod> OpeningPeriods => Set<OpeningPeriod>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Favorite>()
            .HasKey(f => new { f.UserId, f.LocationId });

        builder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId);

        builder.Entity<Favorite>()
            .HasOne(f => f.Location)
            .WithMany()
            .HasForeignKey(f => f.LocationId);

        builder.Entity<Location>()
            .HasIndex(l => l.Slug)
            .IsUnique();
    }
}
