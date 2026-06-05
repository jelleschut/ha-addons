using HAShoppingList.Core.Extensions;
using HAShoppingList.Web.Services;

namespace HAShoppingList.Web.Api;

public static class ShoppingListEndpoints
{
    public static IEndpointRouteBuilder MapShoppingListApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/shoppinglist", async (string? store, ShoppingListService shoppingListService) =>
        {
            var grouped = await shoppingListService.GetGroupedItemsAsync();

            if (!string.IsNullOrWhiteSpace(store))
                grouped = grouped
                    .Where(g => g.Store.Name.Equals(store, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var items     = grouped.SelectMany(g => g.Items).ToList();
            var storeName = string.IsNullOrWhiteSpace(store)
                ? "alle winkels"
                : grouped.FirstOrDefault()?.Store.Name ?? store;

            return Results.Ok(new
            {
                hasItems = items.Count > 0,
                count    = items.Count,
                store    = storeName,
                items    = items.Select(i => new
                {
                    name     = i.Product.Name,
                    quantity = i.Quantity,
                    unit     = i.Unit.ToLabel()
                })
            });
        });

        return app;
    }
}
