using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WakeWeather.Api.Data;
using WakeWeather.Api.Services.Weather;
using WakeWeather.Core.DTOs;
using WakeWeather.Core.Models;

namespace WakeWeather.Api.Endpoints;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/weather").WithTags("Weather");

        group.MapGet("/{slug}", async (string slug, AppDbContext db, WeatherAggregator aggregator, IMemoryCache cache) =>
        {
            var location = await db.Locations.FirstOrDefaultAsync(l => l.Slug == slug);
            if (location is null) return Results.NotFound();

            var cacheKey = $"weather:{slug}";
            if (!cache.TryGetValue(cacheKey, out var cached))
            {
                var (current, forecast, hourlySlots) = await aggregator.GetAggregatedAsync(location.Latitude, location.Longitude);

                var today = DateOnly.FromDateTime(DateTime.Today);
                var endDate = forecast.Count > 0 ? forecast.Max(f => f.Date) : today;

                var openingPeriods = await db.OpeningPeriods
                    .Where(op => op.LocationId == location.Id
                              && op.ValidFrom <= endDate
                              && (op.ValidUntil == null || op.ValidUntil >= today))
                    .ToListAsync();

                var scoredForecast = forecast.Select(day =>
                {
                    var opening = openingPeriods.FirstOrDefault(op =>
                        op.DayOfWeek == day.Date.DayOfWeek
                        && op.ValidFrom <= day.Date
                        && (op.ValidUntil == null || op.ValidUntil >= day.Date));

                    var score = WakeScoreService.Score(hourlySlots, day.Date, opening?.OpenTime, opening?.CloseTime);

                    if (opening?.OpenTime is null || opening.CloseTime is null)
                        return score.HasValue ? day with { WakeScore = score } : day;

                    var slots = hourlySlots
                        .Where(s => DateOnly.FromDateTime(s.Time) == day.Date
                                 && TimeOnly.FromDateTime(s.Time) >= opening.OpenTime
                                 && TimeOnly.FromDateTime(s.Time) < opening.CloseTime)
                        .ToList();

                    if (slots.Count == 0)
                        return score.HasValue ? day with { WakeScore = score } : day;

                    return day with
                    {
                        TempMin = Math.Round(slots.Min(s => s.Temperature), 1),
                        TempMax = Math.Round(slots.Max(s => s.Temperature), 1),
                        AvgWindSpeedKmh = Math.Round(slots.Average(s => s.WindSpeedKmh), 1),
                        MaxWindSpeedKmh = Math.Round(slots.Max(s => s.WindSpeedKmh), 1),
                        PrecipitationSumMm = Math.Round(slots.Sum(s => s.PrecipitationMm), 1),
                        MaxPrecipitationProbability = slots.Max(s => s.PrecipitationProbability),
                        AvgCloudCoverPercent = (int)Math.Round(slots.Average(s => s.CloudCoverPercent)),
                        WakeScore = score
                    };
                }).ToList();

                var currentScore = current is not null
                    ? WakeScoreService.ScoreSingle(current.WindSpeedKmh, current.FeelsLike, current.PrecipitationMm)
                    : (double?)null;

                var todayOpening = openingPeriods.FirstOrDefault(op =>
                    op.DayOfWeek == today.DayOfWeek
                    && op.ValidFrom <= today
                    && (op.ValidUntil == null || op.ValidUntil >= today));
                var todayOpeningWeather = ComputeOpeningHoursWeather(hourlySlots, today, todayOpening?.OpenTime, todayOpening?.CloseTime);

                cached = new { current, forecast = scoredForecast, currentScore, todayOpeningWeather };
                cache.Set(cacheKey, cached, TimeSpan.FromMinutes(30));
                cache.Set($"weather-hourly:{slug}", hourlySlots, TimeSpan.FromMinutes(30));
            }

            return Results.Ok(cached);
        });

        group.MapGet("/{slug}/hourly/{date}", async (string slug, DateOnly date, AppDbContext db, WeatherAggregator aggregator, IMemoryCache cache) =>
        {
            var location = await db.Locations.FirstOrDefaultAsync(l => l.Slug == slug);
            if (location is null) return Results.NotFound();

            if (!cache.TryGetValue($"weather-hourly:{slug}", out IReadOnlyList<HourlySlot>? hourlySlots) || hourlySlots is null)
            {
                var (_, _, freshSlots) = await aggregator.GetAggregatedAsync(location.Latitude, location.Longitude);
                hourlySlots = freshSlots;
                cache.Set($"weather-hourly:{slug}", hourlySlots, TimeSpan.FromMinutes(30));
            }

            var opening = await db.OpeningPeriods
                .Where(op => op.LocationId == location.Id
                          && op.DayOfWeek == date.DayOfWeek
                          && op.ValidFrom <= date
                          && (op.ValidUntil == null || op.ValidUntil >= date))
                .FirstOrDefaultAsync();

            if (opening?.OpenTime is null || opening.CloseTime is null)
                return Results.Ok(Array.Empty<HourlyWeatherDto>());

            var hours = hourlySlots
                .Where(s => DateOnly.FromDateTime(s.Time) == date
                         && TimeOnly.FromDateTime(s.Time) >= opening.OpenTime
                         && TimeOnly.FromDateTime(s.Time) < opening.CloseTime)
                .Select(s => new HourlyWeatherDto(
                    Time: TimeOnly.FromDateTime(s.Time),
                    Temperature: s.Temperature,
                    FeelsLike: s.FeelsLike,
                    WindSpeedKmh: s.WindSpeedKmh,
                    WindDirectionDeg: s.WindDirectionDeg,
                    PrecipitationProbability: s.PrecipitationProbability,
                    PrecipitationMm: s.PrecipitationMm,
                    CloudCoverPercent: s.CloudCoverPercent,
                    WakeScore: WakeScoreService.ScoreSingle(s.WindSpeedKmh, s.FeelsLike, s.PrecipitationMm)
                ))
                .ToList();

            return Results.Ok(hours);
        });
    }

    private static WeatherDto? ComputeOpeningHoursWeather(
        IReadOnlyList<HourlySlot> slots, DateOnly date, TimeOnly? open, TimeOnly? close)
    {
        if (open is null || close is null) return null;

        var relevant = slots
            .Where(s => DateOnly.FromDateTime(s.Time) == date
                     && TimeOnly.FromDateTime(s.Time) >= open
                     && TimeOnly.FromDateTime(s.Time) < close)
            .ToList();

        if (relevant.Count == 0) return null;

        var sinAvg = relevant.Average(s => Math.Sin(s.WindDirectionDeg * Math.PI / 180.0));
        var cosAvg = relevant.Average(s => Math.Cos(s.WindDirectionDeg * Math.PI / 180.0));
        var avgDir = (int)((Math.Atan2(sinAvg, cosAvg) * 180.0 / Math.PI + 360) % 360);

        return new WeatherDto(
            Temperature: Math.Round(relevant.Average(s => s.Temperature), 1),
            FeelsLike: Math.Round(relevant.Average(s => s.FeelsLike), 1),
            WindSpeedKmh: Math.Round(relevant.Average(s => s.WindSpeedKmh), 1),
            WindDirectionDeg: avgDir,
            PrecipitationProbability: Math.Round(relevant.Average(s => s.PrecipitationProbability), 1),
            PrecipitationMm: Math.Round(relevant.Average(s => s.PrecipitationMm), 2),
            CloudCoverPercent: (int)Math.Round(relevant.Average(s => s.CloudCoverPercent)),
            UpdatedAt: DateTime.UtcNow
        );
    }
}
