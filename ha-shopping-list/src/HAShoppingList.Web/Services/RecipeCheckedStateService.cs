namespace HAShoppingList.Web.Services;

public class RecipeCheckedStateService
{
    private readonly Dictionary<(int RecipeId, int ItemId), bool> _state = new();

    public bool GetChecked(int recipeId, int itemId, bool defaultValue) =>
        _state.TryGetValue((recipeId, itemId), out var value) ? value : defaultValue;

    public void SetChecked(int recipeId, int itemId, bool value) =>
        _state[(recipeId, itemId)] = value;

    public void Initialize(int recipeId, int itemId, bool defaultValue) =>
        _state.TryAdd((recipeId, itemId), defaultValue);
}
