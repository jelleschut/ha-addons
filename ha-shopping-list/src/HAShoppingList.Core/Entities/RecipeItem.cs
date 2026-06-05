using HAShoppingList.Core.Enums;

namespace HAShoppingList.Core.Entities;

public class RecipeItem
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal      Quantity         { get; set; } = 1;
    public QuantityUnit Unit             { get; set; } = QuantityUnit.Stuks;
    public int          Position         { get; set; }
    public bool         IsDefaultChecked { get; set; } = true;
}
