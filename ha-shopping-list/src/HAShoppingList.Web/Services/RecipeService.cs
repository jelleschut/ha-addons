using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Enums;
using HAShoppingList.Core.Interfaces;

namespace HAShoppingList.Web.Services;

public class RecipeService(IRecipeRepository recipeRepository)
{
    public Task<List<Recipe>> GetAllAsync()      => recipeRepository.GetAllAsync();
    public Task<Recipe>       AddAsync(Recipe r) => recipeRepository.AddAsync(r);
    public Task                DeleteAsync(int id)   => recipeRepository.DeleteAsync(id);

    public Task UpdateNameAsync(int id, string name) =>
        recipeRepository.UpdateNameAsync(id, name);

    public Task SetItemsAsync(int recipeId, IReadOnlyList<(int ProductId, decimal Quantity, QuantityUnit Unit, bool IsDefaultChecked)> items) =>
        recipeRepository.SetItemsAsync(recipeId, items);
}
