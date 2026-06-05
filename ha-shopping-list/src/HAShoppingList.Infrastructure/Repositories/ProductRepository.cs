using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;
using HAShoppingList.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HAShoppingList.Infrastructure.Repositories;

public class ProductRepository(ShoppingListDbContext context) : IProductRepository 
{
    public Task<List<Product>> GetAllAsync() =>
        context.Products
            .AsNoTracking()
            .Include(p => p.DefaultStore)
            .Include(p => p.StoreOrders)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public Task<Product?> GetByIdWithStoreOrdersAsync(int id) =>
    context.Products
        .Include(p => p.DefaultStore)
        .Include(p => p.StoreOrders)
            .ThenInclude(o => o.Store)
        .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> AddAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        var tracked = await context.Products.FindAsync(product.Id);
        if (tracked is null) return;

        tracked.Name             = product.Name;
        tracked.DefaultQuantity  = product.DefaultQuantity;
        tracked.DefaultUnit      = product.DefaultUnit;
        tracked.DefaultStoreId   = product.DefaultStoreId;
        tracked.IsWeeklyRecurring = product.IsWeeklyRecurring;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product is not null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpsertStoreOrderAsync(int productId, int storeId, int position)
    {
        {
            var rows = await context.StoreProductOrders
                .Where(o => o.ProductId == productId && o.StoreId == storeId)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.Position, position));

            if (rows == 0)
            {
                // Bestaat nog niet — nieuw aanmaken
                context.StoreProductOrders.Add(new StoreProductOrder
                {
                    ProductId = productId,
                    StoreId = storeId,
                    Position = position
                });
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task SaveAllStorePositionsAsync(int storeId, IReadOnlyList<(int ProductId, int Position)> positions)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var (productId, position) in positions)
        {
            var rows = await context.StoreProductOrders
                .Where(o => o.ProductId == productId && o.StoreId == storeId)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.Position, position));

            if (rows == 0)
                context.StoreProductOrders.Add(new StoreProductOrder
                {
                    ProductId = productId,
                    StoreId   = storeId,
                    Position  = position
                });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public Task<List<Product>> GetWeeklyRecurringAsync() =>
        context.Products
            .Where(p => p.IsWeeklyRecurring)
            .Include(p => p.DefaultStore)
            .ToListAsync();
}