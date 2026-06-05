namespace HAShoppingList.Core.Entities;

public class StoreProductOrder
{
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int Position { get; set; }
}