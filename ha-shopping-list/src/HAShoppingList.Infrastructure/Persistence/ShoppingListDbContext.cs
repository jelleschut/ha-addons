using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HAShoppingList.Infrastructure.Persistence;

public class ShoppingListDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StoreProductOrder> StoreProductOrders => Set<StoreProductOrder>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeItem> RecipeItems => Set<RecipeItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreProductOrder>()
            .HasKey(x => new { x.StoreId, x.ProductId });
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingListDbContext).Assembly);
    }
}