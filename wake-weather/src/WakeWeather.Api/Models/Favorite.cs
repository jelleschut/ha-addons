using WakeWeather.Core.Models;

namespace WakeWeather.Api.Models;

public class Favorite
{
    public string UserId { get; set; } = string.Empty;
    public int LocationId { get; set; }

    public AppUser User { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
