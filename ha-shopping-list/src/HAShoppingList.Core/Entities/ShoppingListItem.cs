using HAShoppingList.Core.Enums;

namespace HAShoppingList.Core.Entities;

public class ShoppingListItem
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    
    public decimal Quantity { get; set; }
    public QuantityUnit Unit { get; set; }
    
    public bool IsChecked { get; set; }
    
    public bool IsRecurring { get; set; }
    
    public DateOnly AddedOn { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    
    public string? Note { get; set; }
}