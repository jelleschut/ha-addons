using HAShoppingList.Core.Enums;

namespace HAShoppingList.Core.Entities;

public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int DefaultStoreId { get; set; }
    public Store DefaultStore { get; set; } = null!;

    public decimal DefaultQuantity { get; set; } = 1;
    public QuantityUnit DefaultUnit { get; set; } = QuantityUnit.Stuks;
    
    public bool IsWeeklyRecurring { get; set; }
    
    public List<StoreProductOrder> StoreOrders { get; set; } = [];
}