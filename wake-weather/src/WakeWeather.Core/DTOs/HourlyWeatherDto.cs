namespace WakeWeather.Core.DTOs;

public record HourlyWeatherDto(
    TimeOnly Time,
    double Temperature,
    double FeelsLike,
    double WindSpeedKmh,
    int WindDirectionDeg,
    double PrecipitationProbability,
    double PrecipitationMm,
    int CloudCoverPercent,
    double WakeScore
);
