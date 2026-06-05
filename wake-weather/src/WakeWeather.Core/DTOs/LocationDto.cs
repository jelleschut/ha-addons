namespace WakeWeather.Core.DTOs;

public record LocationDto(
    int Id,
    string Name,
    string Slug,
    double Latitude,
    double Longitude,
    string? Website,
    string? City
);
