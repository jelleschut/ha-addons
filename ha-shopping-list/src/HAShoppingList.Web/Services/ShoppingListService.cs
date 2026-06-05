using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;

namespace HAShoppingList.Web.Services;

public class ShoppingListService(
    IShoppingListRepository shoppingListRepository,
    IProductRepository productRepository,
    IStoreRepository storeRepository)
{
    public async Task<List<StoreWithItems>> GetGroupedItemsAsync()
    {
        var items = await shoppingListRepository.GetActiveItemsAsync();

        return items
            .GroupBy(i => i.StoreId)
            .Select(g => new StoreWithItems
            {
                Store = g.First().Store,
                Items = g.ToList()
            })
            .ToList();
    }

    public Task<List<Store>>   GetAllStoresAsync()   => storeRepository.GetAllOrderedAsync();
    public Task<List<Product>> GetAllProductsAsync() => productRepository.GetAllAsync();

    public async Task AddFromProductAsync(int productId, decimal? quantityOverride = null)
    {
        var product = await productRepository.GetByIdWithStoreOrdersAsync(productId);
        if (product is null) return;

        var quantityToAdd = quantityOverride ?? product.DefaultQuantity;

        var existing = await shoppingListRepository.GetActiveItemByProductIdAsync(productId);
        if (existing is not null)
        {
            existing.Quantity += quantityToAdd;
            await shoppingListRepository.UpdateItemAsync(existing);
            return;
        }

        await shoppingListRepository.AddItemAsync(new ShoppingListItem
        {
            ProductId   = product.Id,
            StoreId     = product.DefaultStoreId,
            Quantity    = quantityToAdd,
            Unit        = product.DefaultUnit,
            IsRecurring = product.IsWeeklyRecurring,
            AddedOn     = DateOnly.FromDateTime(DateTime.Today),
        });
    }

    public Task DeleteItemAsync(int id)                        => shoppingListRepository.DeleteItemAsync(id);
    public Task CheckItemAsync(int id, bool isChecked)         => shoppingListRepository.CheckItemAsync(id, isChecked);
    public Task UpdateItemAsync(ShoppingListItem item)         => shoppingListRepository.UpdateItemAsync(item);
    public Task AddWeeklyRecurringItemsAsync()                 => shoppingListRepository.AddWeeklyRecurringItemsAsync();
    public Task ClearCheckedItemsAsync() => shoppingListRepository.ClearCheckedItemsAsync();
    public Task ClearAllItemsAsync()     => shoppingListRepository.ClearAllItemsAsync();
}

public class StoreWithItems
{
    public Store Store { get; set; } = null!;
    public List<ShoppingListItem> Items { get; set; } = [];
}
