namespace WakeWeather.Core.DTOs;

public record WeatherDto(
    double Temperature,
    double FeelsLike,
    double WindSpeedKmh,
    int WindDirectionDeg,
    double PrecipitationProbability,
    double PrecipitationMm,
    int CloudCoverPercent,
    DateTime UpdatedAt
);
