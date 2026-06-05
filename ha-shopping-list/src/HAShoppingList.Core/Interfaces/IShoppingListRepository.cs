using HAShoppingList.Core.Entities;

namespace HAShoppingList.Core.Interfaces;

public interface IShoppingListRepository
{
    Task<List<ShoppingListItem>> GetActiveItemsAsync();
    Task<ShoppingListItem?> GetActiveItemByProductIdAsync(int productId);
    Task<ShoppingListItem> AddItemAsync(ShoppingListItem item);
    Task UpdateItemAsync(ShoppingListItem item);
    Task DeleteItemAsync(int id);
    Task CheckItemAsync(int id, bool isChecked);
    Task AddWeeklyRecurringItemsAsync();
    Task ClearCheckedItemsAsync();
    Task ClearAllItemsAsync();
}