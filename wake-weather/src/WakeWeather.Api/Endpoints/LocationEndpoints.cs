using Microsoft.EntityFrameworkCore;
using WakeWeather.Api.Data;
using WakeWeather.Core.DTOs;
using WakeWeather.Core.Models;

namespace WakeWeather.Api.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations").WithTags("Locations");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var todayDow = DateTime.Today.DayOfWeek;

            var locations = await db.Locations.OrderBy(l => l.Name).ToListAsync();

            var todayPeriods = await db.OpeningPeriods
                .Where(op => op.DayOfWeek == todayDow
                          && op.ValidFrom <= today
                          && (op.ValidUntil == null || op.ValidUntil >= today)
                          && op.OpenTime != null)
                .ToListAsync();

            var result = locations.Select(l =>
            {
                var period = todayPeriods.FirstOrDefault(p => p.LocationId == l.Id);
                var todayDto = ToOpeningDayDto(today, period);
                return new LocationSummaryDto(l.Id, l.Name, l.Slug, l.Latitude, l.Longitude, l.Website, l.City, todayDto);
            });

            return Results.Ok(result);
        });

        group.MapGet("/{slug}", async (string slug, AppDbContext db) =>
        {
            var location = await db.Locations.FirstOrDefaultAsync(l => l.Slug == slug);
            if (location is null) return Results.NotFound();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var weekDates = Enumerable.Range(0, 16).Select(i => today.AddDays(i)).ToList();

            var periods = await db.OpeningPeriods
                .Where(op => op.LocationId == location.Id
                          && op.ValidFrom <= weekDates.Last()
                          && (op.ValidUntil == null || op.ValidUntil >= today))
                .ToListAsync();

            var weekOpening = weekDates
                .Select(date =>
                {
                    var period = periods.FirstOrDefault(p =>
                        p.DayOfWeek == date.DayOfWeek &&
                        p.ValidFrom <= date &&
                        (p.ValidUntil == null || p.ValidUntil >= date));
                    return ToOpeningDayDto(date, period);
                })
                .ToList();

            return Results.Ok(new LocationDetailDto(
                location.Id, location.Name, location.Slug,
                location.Latitude, location.Longitude,
                location.Website, location.City,
                weekOpening));
        });
    }

    private static OpeningDayDto ToOpeningDayDto(DateOnly date, OpeningPeriod? period) =>
        new(date, period?.OpenTime is not null, period?.OpenTime, period?.CloseTime, period?.Note);
}
