using System.Text.Json;
using WakeWeather.Core.DTOs;

namespace WakeWeather.Api.Services.Weather;

public class BuienradarService(HttpClient http)
{
    private const string FeedUrl = "https://data.buienradar.nl/2.0/feed/json";

    public async Task<WeatherDto?> GetCurrentWeatherAsync(double lat, double lon)
    {
        using var response = await http.GetAsync(FeedUrl);
        if (!response.IsSuccessStatusCode) return null;

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        var stations = doc.RootElement
            .GetProperty("actual")
            .GetProperty("stationmeasurements")
            .EnumerateArray()
            .ToArray();

        var nearest = stations
            .Where(s => s.TryGetProperty("lat", out _) && s.TryGetProperty("lon", out _))
            .MinBy(s => HaversineKm(lat, lon,
                s.GetProperty("lat").GetDouble(),
                s.GetProperty("lon").GetDouble()));

        if (nearest.ValueKind == JsonValueKind.Undefined) return null;

        var windSpeed = nearest.TryGetProperty("windspeed", out var ws) ? ws.GetDouble() : 0;
        var windDir = nearest.TryGetProperty("winddirectiondegrees", out var wd) ? wd.GetInt32() : 0;
        var temp = nearest.TryGetProperty("temperature", out var t) ? t.GetDouble() : 0;
        var feelsLike = nearest.TryGetProperty("feeltemperature", out var ft) ? ft.GetDouble() : temp;
        var precipRate = nearest.TryGetProperty("precipitation", out var pr) ? pr.GetDouble() : 0;

        return new WeatherDto(
            Temperature: Math.Round(temp, 1),
            FeelsLike: Math.Round(feelsLike, 1),
            WindSpeedKmh: Math.Round(windSpeed * 3.6, 1),
            WindDirectionDeg: windDir,
            PrecipitationProbability: 0,
            PrecipitationMm: precipRate,
            CloudCoverPercent: 0,
            UpdatedAt: DateTime.UtcNow
        );
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return r * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
