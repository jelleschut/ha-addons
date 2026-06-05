namespace WakeWeather.Core.DTOs;

public record ForecastDayDto(
    DateOnly Date,
    double TempMin,
    double TempMax,
    double AvgWindSpeedKmh,
    double MaxWindSpeedKmh,
    double PrecipitationSumMm,
    double MaxPrecipitationProbability,
    int AvgCloudCoverPercent,
    double? WakeScore = null
);
