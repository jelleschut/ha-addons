using HAShoppingList.Core.Entities;

namespace HAShoppingList.Core.Interfaces;

public interface IStoreRepository
{
    Task<List<Store>> GetAllOrderedAsync();
    Task<Store?> GetByIdAsync(int id);
    Task<Store> AddAsync(Store store);
    Task UpdateAsync(Store store);
    Task DeleteAsync(int id);
    Task UpdateDisplayOrderAsync(List<(int StoreId, int NewOrder)> updates);
}