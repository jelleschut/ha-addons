namespace WakeWeather.Core.Models;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Website { get; set; }
    public string? City { get; set; }

    public ICollection<OpeningPeriod> OpeningPeriods { get; set; } = [];
}
