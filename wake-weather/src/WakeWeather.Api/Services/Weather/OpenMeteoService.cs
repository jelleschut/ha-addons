using System.Text.Json;
using WakeWeather.Core.DTOs;

namespace WakeWeather.Api.Services.Weather;

public class OpenMeteoService(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<(WeatherDto? Current, IReadOnlyList<ForecastDayDto> Forecast, IReadOnlyList<HourlySlot> HourlySlots)> GetWeatherAsync(double lat, double lon)
    {
        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  $"&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                  $"&hourly=temperature_2m,apparent_temperature,precipitation_probability,precipitation,cloud_cover,wind_speed_10m,wind_direction_10m" +
                  $"&wind_speed_unit=kmh&timezone=Europe%2FAmsterdam&forecast_days=16";

        using var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var hourly = doc.RootElement.GetProperty("hourly");

        var times = hourly.GetProperty("time").EnumerateArray().Select(t => DateTime.Parse(t.GetString()!)).ToArray();
        var temps = hourly.GetProperty("temperature_2m").EnumerateArray().Select(t => t.GetDouble()).ToArray();
        var feelsLike = hourly.GetProperty("apparent_temperature").EnumerateArray().Select(t => t.GetDouble()).ToArray();
        var precipProb = hourly.GetProperty("precipitation_probability").EnumerateArray().Select(t => t.GetDouble()).ToArray();
        var precip = hourly.GetProperty("precipitation").EnumerateArray().Select(t => t.GetDouble()).ToArray();
        var cloudCover = hourly.GetProperty("cloud_cover").EnumerateArray().Select(t => t.GetInt32()).ToArray();
        var windSpeed = hourly.GetProperty("wind_speed_10m").EnumerateArray().Select(t => t.GetDouble()).ToArray();
        var windDir = hourly.GetProperty("wind_direction_10m").EnumerateArray().Select(t => t.GetInt32()).ToArray();

        var now = DateTime.Now;
        var currentIndex = times.Select((t, i) => (t, i)).MinBy(x => Math.Abs((x.t - now).TotalMinutes)).i;

        var current = new WeatherDto(
            Temperature: Math.Round(temps[currentIndex], 1),
            FeelsLike: Math.Round(feelsLike[currentIndex], 1),
            WindSpeedKmh: Math.Round(windSpeed[currentIndex], 1),
            WindDirectionDeg: windDir[currentIndex],
            PrecipitationProbability: precipProb[currentIndex],
            PrecipitationMm: precip[currentIndex],
            CloudCoverPercent: cloudCover[currentIndex],
            UpdatedAt: DateTime.UtcNow
        );

        var forecast = BuildForecast(times, temps, feelsLike, precipProb, precip, cloudCover, windSpeed, windDir);

        var hourlySlots = times.Select((t, i) => new HourlySlot(
            Time: t,
            Temperature: Math.Round(temps[i], 1),
            FeelsLike: Math.Round(feelsLike[i], 1),
            WindSpeedKmh: Math.Round(windSpeed[i], 1),
            WindDirectionDeg: windDir[i],
            PrecipitationProbability: precipProb[i],
            PrecipitationMm: precip[i],
            CloudCoverPercent: cloudCover[i]
        )).ToList();

        return (current, forecast, hourlySlots);
    }

    private static List<ForecastDayDto> BuildForecast(
        DateTime[] times, double[] temps, double[] feelsLike,
        double[] precipProb, double[] precip, int[] cloudCover,
        double[] windSpeed, int[] windDir)
    {
        return times
            .Select((t, i) => new { t, i })
            .GroupBy(x => DateOnly.FromDateTime(x.t))
            .Select(g => new ForecastDayDto(
                Date: g.Key,
                TempMin: Math.Round(g.Min(x => temps[x.i]), 1),
                TempMax: Math.Round(g.Max(x => temps[x.i]), 1),
                AvgWindSpeedKmh: Math.Round(g.Average(x => windSpeed[x.i]), 1),
                MaxWindSpeedKmh: Math.Round(g.Max(x => windSpeed[x.i]), 1),
                PrecipitationSumMm: Math.Round(g.Sum(x => precip[x.i]), 1),
                MaxPrecipitationProbability: g.Max(x => precipProb[x.i]),
                AvgCloudCoverPercent: (int)g.Average(x => cloudCover[x.i])
            ))
            .ToList();
    }
}
