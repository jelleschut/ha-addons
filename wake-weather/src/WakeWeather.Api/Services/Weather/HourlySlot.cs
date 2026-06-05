namespace WakeWeather.Api.Services.Weather;

public record HourlySlot(
    DateTime Time,
    double Temperature,
    double FeelsLike,
    double WindSpeedKmh,
    int WindDirectionDeg,
    double PrecipitationProbability,
    double PrecipitationMm,
    int CloudCoverPercent
);
