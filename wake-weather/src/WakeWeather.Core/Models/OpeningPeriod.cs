namespace WakeWeather.Core.Models;

public class OpeningPeriod
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidUntil { get; set; }

    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }   // null = gesloten
    public TimeOnly? CloseTime { get; set; }
    public string? Note { get; set; }
}
