namespace WakeWeather.Services;

public static class WindHelper
{
    public static int Beaufort(double kmh) => kmh switch
    {
        < 1   => 0,
        < 6   => 1,
        < 12  => 2,
        < 20  => 3,
        < 29  => 4,
        < 39  => 5,
        < 50  => 6,
        < 62  => 7,
        < 75  => 8,
        < 89  => 9,
        < 103 => 10,
        < 118 => 11,
        _     => 12
    };

    public static string DirectionLabel(int deg) => deg switch
    {
        >= 337 or < 23   => "N",
        >= 23 and < 68   => "NO",
        >= 68 and < 113  => "O",
        >= 113 and < 158 => "ZO",
        >= 158 and < 203 => "Z",
        >= 203 and < 248 => "ZW",
        >= 248 and < 293 => "W",
        _                => "NW"
    };
}
