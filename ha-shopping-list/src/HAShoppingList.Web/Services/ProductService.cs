using HAShoppingList.Core.Entities;
using HAShoppingList.Core.Interfaces;

namespace HAShoppingList.Web.Services;

public class ProductService(IProductRepository productRepository)
{
    public Task<List<Product>> GetAllAsync()          => productRepository.GetAllAsync();
    public Task<Product>       AddAsync(Product p)    => productRepository.AddAsync(p);
    public Task                UpdateAsync(Product p)  => productRepository.UpdateAsync(p);
    public Task                DeleteAsync(int id)    => productRepository.DeleteAsync(id);

    public Task SaveAllStorePositionsAsync(int storeId, IReadOnlyList<(int ProductId, int Position)> positions)
        => productRepository.SaveAllStorePositionsAsync(storeId, positions);
}
