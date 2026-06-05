using WakeWeather.Core.DTOs;

namespace WakeWeather.Api.Services.Weather;

public class WeatherAggregator(OpenMeteoService openMeteo, BuienradarService buienradar)
{
    public async Task<(WeatherDto? Current, IReadOnlyList<ForecastDayDto> Forecast, IReadOnlyList<HourlySlot> HourlySlots)> GetAggregatedAsync(double lat, double lon)
    {
        var openMeteoTask = openMeteo.GetWeatherAsync(lat, lon);
        var buienradarTask = buienradar.GetCurrentWeatherAsync(lat, lon);

        await Task.WhenAll(openMeteoTask, buienradarTask);

        var (omCurrent, forecast, hourlySlots) = await openMeteoTask;
        var brCurrent = await buienradarTask;

        var current = Merge(omCurrent, brCurrent);
        return (current, forecast, hourlySlots);
    }

    private static WeatherDto? Merge(WeatherDto? om, WeatherDto? br)
    {
        if (om is null && br is null) return null;
        if (om is null) return br;
        if (br is null) return om;

        return new WeatherDto(
            Temperature: Math.Round((om.Temperature + br.Temperature) / 2, 1),
            FeelsLike: Math.Round((om.FeelsLike + br.FeelsLike) / 2, 1),
            WindSpeedKmh: Math.Round((om.WindSpeedKmh + br.WindSpeedKmh) / 2, 1),
            WindDirectionDeg: AverageCircularDeg(om.WindDirectionDeg, br.WindDirectionDeg),
            PrecipitationProbability: om.PrecipitationProbability,
            PrecipitationMm: br.PrecipitationMm > 0 ? br.PrecipitationMm : om.PrecipitationMm,
            CloudCoverPercent: om.CloudCoverPercent,
            UpdatedAt: DateTime.UtcNow
        );
    }

    private static int AverageCircularDeg(int a, int b)
    {
        var diff = ((b - a + 540) % 360) - 180;
        return (int)((a + diff / 2.0 + 360) % 360);
    }
}
