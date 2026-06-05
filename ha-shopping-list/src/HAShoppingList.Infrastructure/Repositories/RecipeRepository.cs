using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Enums;
using HAShoppingList.Core.Interfaces;
using HAShoppingList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HAShoppingList.Infrastructure.Repositories;

public class RecipeRepository(ShoppingListDbContext context) : IRecipeRepository
{
    public Task<List<Recipe>> GetAllAsync() =>
        context.Recipes
            .AsNoTracking()
            .Include(r => r.Items.OrderBy(i => i.Position))
                .ThenInclude(i => i.Product)
            .OrderBy(r => r.Name)
            .ToListAsync();

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();
        return recipe;
    }

    public Task UpdateNameAsync(int id, string name) =>
        context.Recipes
            .Where(r => r.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Name, name));

    public async Task DeleteAsync(int id)
    {
        var recipe = await context.Recipes.FindAsync(id);
        if (recipe is not null)
        {
            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();
        }
    }

    public async Task SetItemsAsync(int recipeId, IReadOnlyList<(int ProductId, decimal Quantity, QuantityUnit Unit, bool IsDefaultChecked)> items)
    {
        await context.RecipeItems
            .Where(i => i.RecipeId == recipeId)
            .ExecuteDeleteAsync();

        context.RecipeItems.AddRange(items.Select((i, idx) => new RecipeItem
        {
            RecipeId         = recipeId,
            ProductId        = i.ProductId,
            Quantity         = i.Quantity,
            Unit             = i.Unit,
            Position         = idx + 1,
            IsDefaultChecked = i.IsDefaultChecked
        }));

        await context.SaveChangesAsync();
    }
}
