using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WakeWeather.Services;

public class FavoritesService(ProtectedLocalStorage storage)
{
    private const string Key = "favorites";

    public async Task<HashSet<int>> GetAsync()
    {
        var result = await storage.GetAsync<int[]>(Key);
        return result.Success && result.Value is not null
            ? [.. result.Value]
            : [];
    }

    public async Task<bool> IsFavoriteAsync(int locationId)
    {
        var favorites = await GetAsync();
        return favorites.Contains(locationId);
    }

    public async Task ToggleAsync(int locationId)
    {
        var favorites = await GetAsync();
        if (!favorites.Add(locationId))
            favorites.Remove(locationId);
        await storage.SetAsync(Key, favorites.ToArray());
    }
}
