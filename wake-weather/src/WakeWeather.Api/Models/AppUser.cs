using Microsoft.AspNetCore.Identity;

namespace WakeWeather.Api.Models;

public class AppUser : IdentityUser
{
    public ICollection<Favorite> Favorites { get; set; } = [];
}
