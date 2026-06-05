using HAShoppingList.Core.Entities;

namespace HAShoppingList.Core.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdWithStoreOrdersAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task UpsertStoreOrderAsync(int productId, int storeId, int position);
    Task SaveAllStorePositionsAsync(int storeId, IReadOnlyList<(int ProductId, int Position)> positions);
    Task<List<Product>> GetWeeklyRecurringAsync();
}