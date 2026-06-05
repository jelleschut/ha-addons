namespace WakeWeather.Core.DTOs;

public record OpeningDayDto(
    DateOnly Date,
    bool IsOpen,
    TimeOnly? OpenTime,
    TimeOnly? CloseTime,
    string? Note
);
