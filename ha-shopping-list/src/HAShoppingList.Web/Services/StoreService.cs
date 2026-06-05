using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;

namespace HAShoppingList.Web.Services;

public class StoreService(IStoreRepository storeRepository)
{
    public Task<List<Store>> GetAllOrderedAsync()  => storeRepository.GetAllOrderedAsync();
    public Task<Store>       AddAsync(Store store) => storeRepository.AddAsync(store);
    public Task              UpdateAsync(Store s)  => storeRepository.UpdateAsync(s);
    public Task              DeleteAsync(int id)   => storeRepository.DeleteAsync(id);

    public Task UpdateDisplayOrderAsync(List<(int StoreId, int NewOrder)> updates) =>
        storeRepository.UpdateDisplayOrderAsync(updates);
}
