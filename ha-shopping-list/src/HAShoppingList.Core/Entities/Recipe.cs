namespace HAShoppingList.Core.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RecipeItem> Items { get; set; } = [];
}
