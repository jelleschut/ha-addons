using HAShoppingList.Core.Enums;

namespace HAShoppingList.Core.Extensions;

public static class QuantityUnitExtensions
{
    public static string ToLabel(this QuantityUnit unit) => unit switch
    {
        QuantityUnit.Stuks      => "st",
        QuantityUnit.Gram       => "g",
        QuantityUnit.Kilo       => "kg",
        QuantityUnit.Milliliter => "ml",
        QuantityUnit.Liter      => "l",
        QuantityUnit.Theelepel  => "tl",
        QuantityUnit.Eetlepel   => "el",
        QuantityUnit.Blik       => "blik",
        QuantityUnit.Zak        => "zak",
        _                       => unit.ToString()
    };
}
