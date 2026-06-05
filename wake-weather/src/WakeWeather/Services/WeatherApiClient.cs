using System.Net.Http.Json;
using WakeWeather.Core.DTOs;

namespace WakeWeather.Services;

public class WeatherApiClient(HttpClient http)
{
    public Task<List<LocationSummaryDto>?> GetLocationsAsync() =>
        http.GetFromJsonAsync<List<LocationSummaryDto>>("/api/locations");

    public Task<LocationDetailDto?> GetLocationAsync(string slug) =>
        http.GetFromJsonAsync<LocationDetailDto>($"/api/locations/{slug}");

    public Task<WeatherResponse?> GetWeatherAsync(string slug) =>
        http.GetFromJsonAsync<WeatherResponse>($"/api/weather/{slug}");

    public Task<List<HourlyWeatherDto>?> GetHourlyWeatherAsync(string slug, DateOnly date) =>
        http.GetFromJsonAsync<List<HourlyWeatherDto>>($"/api/weather/{slug}/hourly/{date:yyyy-MM-dd}");
}

public record WeatherResponse(WeatherDto? Current, List<ForecastDayDto> Forecast, double? CurrentScore = null, WeatherDto? TodayOpeningWeather = null);
