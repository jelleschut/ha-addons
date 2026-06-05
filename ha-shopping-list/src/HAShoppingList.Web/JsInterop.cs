namespace HAShoppingList.Web;

internal static class JsInterop
{
    public const string InitSortable    = "initSortable";
    public const string DestroySortable = "destroySortable";

    public static class ElementIds
    {
        public const string StoreList         = "store-list";
        public const string PositionOrderList = "position-order-list";

        public static string ProductListForStore(int storeId) => $"product-list-{storeId}";
    }
}
