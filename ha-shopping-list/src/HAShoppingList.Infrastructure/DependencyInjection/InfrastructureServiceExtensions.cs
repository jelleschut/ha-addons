using HAShoppingList.Core.Interfaces;
using HAShoppingList.Infrastructure.Persistence;
using HAShoppingList.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HAShoppingList.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbPath = configuration.GetSection("Database:Path").Value ?? "shoppinglist.db";
        
        services.AddDbContext<ShoppingListDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        
        return services;
    }
}