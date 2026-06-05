namespace WakeWeather.Core.DTOs;

public record LocationDetailDto(
    int Id,
    string Name,
    string Slug,
    double Latitude,
    double Longitude,
    string? Website,
    string? City,
    IReadOnlyList<OpeningDayDto> WeekOpening
);
