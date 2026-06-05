using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Enums;

namespace HAShoppingList.Core.Interfaces;

public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync();
    Task<Recipe> AddAsync(Recipe recipe);
    Task UpdateNameAsync(int id, string name);
    Task DeleteAsync(int id);
    Task SetItemsAsync(int recipeId, IReadOnlyList<(int ProductId, decimal Quantity, QuantityUnit Unit, bool IsDefaultChecked)> items);
}
