namespace WakeWeather.Api.Services.Weather;

public static class WakeScoreService
{
    public static double ScoreSingle(double windKmh, double feelsLike, double precipMm)
        => Math.Round(
            0.4 * ScoreWind(windKmh) +
            0.4 * ScoreTemp(feelsLike) +
            0.2 * ScorePrecip(precipMm), 1);

    public static double? Score(IReadOnlyList<HourlySlot> slots, DateOnly date, TimeOnly? open, TimeOnly? close)
    {
        if (open is null || close is null) return null;

        var relevant = slots
            .Where(s => DateOnly.FromDateTime(s.Time) == date
                     && TimeOnly.FromDateTime(s.Time) >= open
                     && TimeOnly.FromDateTime(s.Time) < close)
            .ToList();

        if (relevant.Count == 0) return null;

        var avg = relevant.Average(s =>
            0.4 * ScoreWind(s.WindSpeedKmh) +
            0.4 * ScoreTemp(s.FeelsLike) +
            0.2 * ScorePrecip(s.PrecipitationMm));

        return Math.Round(avg, 1);
    }

    private static double ScoreWind(double kmh) => PiecewiseLinear(kmh,
        (0, 10), (40, 0));

    private static double ScoreTemp(double celsius) => PiecewiseLinear(celsius,
        (15, 0), (24, 10));

    private static double ScorePrecip(double mm) => PiecewiseLinear(mm,
        (0, 10), (5, 0));

    private static double PiecewiseLinear(double x, params (double X, double Y)[] points)
    {
        if (x <= points[0].X) return points[0].Y;
        if (x >= points[^1].X) return points[^1].Y;
        for (var i = 0; i < points.Length - 1; i++)
        {
            if (x <= points[i + 1].X)
            {
                var t = (x - points[i].X) / (points[i + 1].X - points[i].X);
                return points[i].Y + t * (points[i + 1].Y - points[i].Y);
            }
        }
        return points[^1].Y;
    }
}
