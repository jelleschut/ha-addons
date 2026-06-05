using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;
using HAShoppingList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HAShoppingList.Infrastructure.Repositories;

public class StoreRepository(ShoppingListDbContext context) : IStoreRepository
{
    public Task<List<Store>> GetAllOrderedAsync() =>
        context.Stores
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();

    public Task<Store?> GetByIdAsync(int id) =>
        context.Stores
            .FindAsync(id).AsTask();

    public async Task<Store> AddAsync(Store store)
    {
        context.Stores.Add(store);
        await context.SaveChangesAsync();
        return store;
    }

    public async Task UpdateAsync(Store store)
    {
        context.Stores.Update(store);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var store = await context.Stores.FindAsync(id);
        if (store is not null)
        {
            context.Stores.Remove(store);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateDisplayOrderAsync(List<(int StoreId, int NewOrder)> updates)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var (storeId, newOrder) in updates)
            await context.Stores
                .Where(s => s.Id == storeId)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.DisplayOrder, newOrder));

        await transaction.CommitAsync();
    }
}