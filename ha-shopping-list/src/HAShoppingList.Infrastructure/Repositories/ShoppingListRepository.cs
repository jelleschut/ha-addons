using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;
using HAShoppingList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HAShoppingList.Infrastructure.Repositories;

public class ShoppingListRepository(ShoppingListDbContext context) : IShoppingListRepository
{
    public Task<List<ShoppingListItem>> GetActiveItemsAsync() =>
        context.ShoppingListItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Store)
                .ThenInclude(s => s.ProductOrders)
            .Where(i => !i.IsChecked)
            .OrderBy(i => i.Store.DisplayOrder)
            .ThenBy(i => i.Store.ProductOrders
                .Where(o => o.ProductId == i.ProductId)
                .Select(o => o.Position)
                .FirstOrDefault())
            .ToListAsync();

    public Task<ShoppingListItem?> GetActiveItemByProductIdAsync(int productId) =>
        context.ShoppingListItems
            .FirstOrDefaultAsync(i => i.ProductId == productId && !i.IsChecked);

    public async Task<ShoppingListItem> AddItemAsync(ShoppingListItem item)
    {
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateItemAsync(ShoppingListItem item)
    {
        var tracked = await context.ShoppingListItems.FindAsync(item.Id);
        if (tracked is null) return;

        tracked.Quantity  = item.Quantity;
        tracked.Unit      = item.Unit;
        tracked.Note      = item.Note;
        tracked.StoreId   = item.StoreId;
        tracked.IsChecked = item.IsChecked;

        await context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await context.ShoppingListItems.FindAsync(id);
        if (item is not null)
        {
            context.ShoppingListItems.Remove(item);
            await context.SaveChangesAsync();
        }
    }

    public async Task CheckItemAsync(int id, bool isChecked)
    {
        var item = await context.ShoppingListItems.FindAsync(id);
        if (item is not null)
        {
            item.IsChecked = isChecked;
            await context.SaveChangesAsync();
        }
    }

    public async Task AddWeeklyRecurringItemsAsync()
    {
        var recurringProducts = await context.Products
            .Where(p => p.IsWeeklyRecurring)
            .ToListAsync();

        // Alleen toevoegen wat er nog niet op staat (niet afgevinkt)
        var alreadyOnList = await context.ShoppingListItems
            .Where(i => !i.IsChecked)
            .Select(i => i.ProductId)
            .ToListAsync();

        var toAdd = recurringProducts
            .Where(p => !alreadyOnList.Contains(p.Id))
            .Select(p => new ShoppingListItem
            {
                ProductId   = p.Id,
                StoreId     = p.DefaultStoreId,
                Quantity    = p.DefaultQuantity,
                Unit        = p.DefaultUnit,
                IsRecurring = true,
                AddedOn     = DateOnly.FromDateTime(DateTime.Today)
            });

        context.ShoppingListItems.AddRange(toAdd);
        await context.SaveChangesAsync();
    }

    public async Task ClearCheckedItemsAsync()
    {
        var checkedItems = await context.ShoppingListItems
            .Where(i => i.IsChecked && !i.IsRecurring)
            .ToListAsync();

        context.ShoppingListItems.RemoveRange(checkedItems);
        await context.SaveChangesAsync();
    }

    public Task ClearAllItemsAsync() =>
        context.ShoppingListItems.ExecuteDeleteAsync();
}