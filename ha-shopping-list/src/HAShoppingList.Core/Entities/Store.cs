namespace HAShoppingList.Core.Entities;

public class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public List<StoreProductOrder> ProductOrders { get; set; } = [];
    public List<ShoppingListItem> ShoppingListItems { get; set; } = [];
}