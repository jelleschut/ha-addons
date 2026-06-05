namespace WakeWeather.Core.DTOs;

public record LocationSummaryDto(
    int Id,
    string Name,
    string Slug,
    double Latitude,
    double Longitude,
    string? Website,
    string? City,
    OpeningDayDto? TodayOpening
);
