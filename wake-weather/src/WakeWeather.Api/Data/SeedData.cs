using WakeWeather.Core.Models;
using static System.DayOfWeek;

namespace WakeWeather.Api.Data;

public static class SeedData
{
    public static readonly Location[] Locations =
    [
        new() { Name = "Down Under",              Slug = "down-under",           City = "Nieuwegein",          Latitude = 52.0170, Longitude = 5.0922, Website = "https://downunder.nl" },
        new() { Name = "Wet 'n Wild WakePark",    Slug = "wet-n-wild",           City = "Alphen aan den Rijn", Latitude = 52.1228, Longitude = 4.6635, Website = "https://wetnwild.nl" },
        new() { Name = "Project 7 Cablepark",     Slug = "project-7",            City = "Rotterdam",           Latitude = 51.9572, Longitude = 4.5244, Website = "https://www.project7cablepark.nl" },
        new() { Name = "Cablepark VIEW Almere",   Slug = "cablepark-view-almere",City = "Almere",              Latitude = 52.3590, Longitude = 5.2150, Website = "https://cableparkviewalmere.nl" },
        new() { Name = "Lakeside Cablepark",      Slug = "lakeside-zwolle",      City = "Zwolle",              Latitude = 52.5163, Longitude = 6.0600, Website = "https://www.lakesidezwolle.nl" },
        new() { Name = "Wakepark Groningen",      Slug = "wakepark-groningen",   City = "Groningen",           Latitude = 53.2194, Longitude = 6.5665, Website = "https://wakeparkgroningen.nl" },
        new() { Name = "Seven-Twenty Cablepark",  Slug = "seven-twenty",         City = "Sevenum",             Latitude = 51.4112, Longitude = 6.0260, Website = "https://www.seven-twenty.nl" },
        new() { Name = "Beaver Creek Wake Park",  Slug = "beaver-creek",         City = "Roermond",            Latitude = 51.1983, Longitude = 5.9892, Website = "https://www.wake-park.nl" },
        new() { Name = "Zeumeren Watersport",     Slug = "zeumeren-watersport",  City = "Voorthuizen",         Latitude = 52.1838, Longitude = 5.6174, Website = "https://zeumerenwatersport.nl" },
        new() { Name = "Cablepark Aquabest",      Slug = "cablepark-aquabest",   City = "Best",                Latitude = 51.5006, Longitude = 5.4390, Website = "https://www.cableparkaquabest.nl" },
        new() { Name = "Burnside Cablepark",      Slug = "burnside",             City = "Emst",                Latitude = 52.3401, Longitude = 5.9813, Website = "https://www.burnsidecablepark.nl" },
        new() { Name = "SKEEF / Luna Lake",       Slug = "skeef",                City = "Heerhugowaard",       Latitude = 52.6702, Longitude = 4.8393, Website = "https://lunalake.nl" },
    ];

    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Locations.Any())
        {
            await context.Locations.AddRangeAsync(Locations);
            await context.SaveChangesAsync();
        }

        if (!context.OpeningPeriods.Any())
        {
            var ids = context.Locations.Local
                .ToDictionary(l => l.Slug, l => l.Id, StringComparer.Ordinal);
            await context.OpeningPeriods.AddRangeAsync(BuildPeriods(ids));
            await context.SaveChangesAsync();
        }
    }

    // -------------------------------------------------------------------------
    // Opening periods — sources: cableparks.nl, individual websites (June 2026)
    // Update annually at the start of each season.
    // -------------------------------------------------------------------------
    private static IEnumerable<OpeningPeriod> BuildPeriods(Dictionary<string, int> ids)
    {
        var periods = new List<OpeningPeriod>();

        // Down Under (downunder.nl) — seizoen maart t/m oktober
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["down-under"], (4, 1), (5, 31),
            (Monday,    17, 0, 21, 0),
            (Tuesday,   17, 0, 21, 0),
            (Wednesday, 15, 0, 21, 0),
            (Thursday,  15, 0, 21, 0),
            (Friday,    15, 0, 21, 0),
            (Saturday,  12, 0, 21, 0),
            (Sunday,    12, 0, 21, 0)));
        periods.AddRange(P(ids["down-under"], (6, 1), (6, 30),
            (Monday,    17, 0, 21, 0),
            (Tuesday,   17, 0, 21, 0),
            (Wednesday, 15, 0, 21, 0),
            (Thursday,  15, 0, 21, 0),
            (Friday,    15, 0, 21, 0),
            (Saturday,  12, 0, 21, 0),
            (Sunday,    12, 0, 21, 0)));
        periods.AddRange(P(ids["down-under"], (7, 1), (10, 31),
            (Monday,    13, 0, 21, 0),
            (Tuesday,   13, 0, 21, 0),
            (Wednesday, 13, 0, 21, 0),
            (Thursday,  13, 0, 21, 0),
            (Friday,    13, 0, 21, 0),
            (Saturday,  12, 0, 21, 0),
            (Sunday,    12, 0, 21, 0)));

        // Wet 'n Wild (wetnwild.nl) — seizoen mei t/m september
        // Bron: website mei–sep 2026
        periods.AddRange(P(ids["wet-n-wild"], (5, 1), (9, 30),
            (Monday,    11, 0, 22, 0),
            (Tuesday,   11, 0, 22, 0),
            (Wednesday, 11, 0, 22, 0),
            (Thursday,  11, 0, 22, 0),
            (Friday,    11, 0, 22, 0),
            (Saturday,  11, 0, 22, 0),
            (Sunday,    10, 0, 22, 0)));

        // Project 7 (project7cablepark.nl)
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["project-7"], (4, 1), (5, 31),
            (Tuesday,   14, 0, 21, 30),
            (Wednesday, 14, 0, 21, 30),
            (Thursday,  14, 0, 21, 30),
            (Friday,    14, 0, 21, 30),
            (Saturday,  12, 30, 21, 30),
            (Sunday,    12, 30, 21, 30)));
        periods.AddRange(P(ids["project-7"], (6, 1), (6, 30),
            (Monday,    14, 0, 21, 30),
            (Tuesday,   14, 0, 21, 30),
            (Wednesday, 14, 0, 21, 30),
            (Thursday,  14, 0, 21, 30),
            (Friday,    14, 0, 21, 30),
            (Saturday,  12, 30, 21, 30),
            (Sunday,    12, 30, 21, 30)));
        periods.AddRange(P(ids["project-7"], (7, 1), (10, 31),
            (Monday,    11, 0, 21, 30),
            (Tuesday,   11, 0, 21, 30),
            (Wednesday, 11, 0, 21, 30),
            (Thursday,  11, 0, 21, 30),
            (Friday,    11, 0, 21, 30),
            (Saturday,  11, 0, 21, 30),
            (Sunday,    11, 0, 21, 30)));

        // Cablepark VIEW Almere (cableparkviewalmere.nl) — seizoen v.a. 15 maart
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["cablepark-view-almere"], (3, 15), (5, 31),
            (Wednesday, 15, 0, 20, 0),
            (Thursday,  15, 0, 20, 0),
            (Friday,    15, 0, 20, 0),
            (Saturday,  12, 0, 20, 0),
            (Sunday,    12, 0, 20, 0)));
        periods.AddRange(P(ids["cablepark-view-almere"], (6, 1), (6, 30),
            (Wednesday, 15, 0, 20, 0),
            (Thursday,  15, 0, 20, 0),
            (Friday,    15, 0, 20, 0),
            (Saturday,  12, 0, 20, 0),
            (Sunday,    12, 0, 20, 0)));
        periods.AddRange(P(ids["cablepark-view-almere"], (7, 1), (10, 31),
            (Tuesday,   12, 0, 20, 0),
            (Wednesday, 12, 0, 20, 0),
            (Thursday,  12, 0, 20, 0),
            (Friday,    12, 0, 20, 0),
            (Saturday,  12, 0, 20, 0),
            (Sunday,    12, 0, 20, 0)));

        // Lakeside Cablepark Zwolle (lakesidezwolle.nl) — seizoen Pasen t/m oktober
        // Bron: cableparks.nl juni/juli 2026; sluitingstijd = zonsondergang (approx 21:30 zomer)
        periods.AddRange(P(ids["lakeside-zwolle"], (3, 28), (5, 31),
            (Wednesday, 11, 0, 20, 0),
            (Thursday,  11, 0, 20, 0),
            (Friday,    11, 0, 20, 0),
            (Saturday,  11, 0, 20, 0),
            (Sunday,    11, 0, 20, 0)));
        periods.AddRange(P(ids["lakeside-zwolle"], (6, 1), (6, 30),
            (Wednesday, 11, 0, 21, 30),
            (Thursday,  11, 0, 21, 30),
            (Friday,    11, 0, 21, 30),
            (Saturday,  11, 0, 21, 30),
            (Sunday,    11, 0, 21, 30)));
        periods.AddRange(P(ids["lakeside-zwolle"], (7, 1), (8, 31),
            (Monday,    12, 0, 21, 30),
            (Tuesday,   12, 0, 21, 30),
            (Wednesday, 12, 0, 21, 30),
            (Thursday,  12, 0, 21, 30),
            (Friday,    12, 0, 21, 30),
            (Saturday,  12, 0, 21, 30),
            (Sunday,    12, 0, 21, 30)));
        periods.AddRange(P(ids["lakeside-zwolle"], (9, 1), (10, 31),
            (Wednesday, 11, 0, 20, 0),
            (Thursday,  11, 0, 20, 0),
            (Friday,    11, 0, 20, 0),
            (Saturday,  11, 0, 20, 0),
            (Sunday,    11, 0, 20, 0)));

        // Wakepark Groningen (wakeparkgroningen.nl) — geen vaste uren, trainingsschema
        // Bron: website; op afspraak buiten deze tijden
        periods.AddRange(P(ids["wakepark-groningen"], (4, 1), (10, 31),
            (Tuesday,   16, 0, 22, 0),
            (Wednesday, 14, 0, 20, 0),
            (Thursday,  16, 0, 22, 0),
            (Friday,    14, 0, 20, 0),
            (Saturday,  12, 0, 18, 0),
            (Sunday,    12, 0, 18, 0)));

        // Seven-Twenty Cablepark (seven-twenty.nl) — seizoen maart t/m oktober
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["seven-twenty"], (3, 1), (6, 30),
            (Monday,    12, 0, 19, 0),
            (Tuesday,   12, 0, 19, 0),
            (Wednesday, 12, 0, 19, 0),
            (Thursday,  12, 0, 19, 0),
            (Friday,    12, 0, 19, 0),
            (Saturday,  12, 0, 19, 0),
            (Sunday,    12, 0, 19, 0)));
        periods.AddRange(P(ids["seven-twenty"], (7, 1), (8, 31),
            (Monday,    10, 0, 21, 0),
            (Tuesday,   10, 0, 21, 0),
            (Wednesday, 10, 0, 21, 0),
            (Thursday,  10, 0, 21, 0),
            (Friday,    10, 0, 21, 0),
            (Saturday,  10, 0, 21, 0),
            (Sunday,    10, 0, 21, 0)));
        periods.AddRange(P(ids["seven-twenty"], (9, 1), (10, 31),
            (Monday,    12, 0, 19, 0),
            (Tuesday,   12, 0, 19, 0),
            (Wednesday, 12, 0, 19, 0),
            (Thursday,  12, 0, 19, 0),
            (Friday,    12, 0, 19, 0),
            (Saturday,  12, 0, 19, 0),
            (Sunday,    12, 0, 19, 0)));

        // Beaver Creek Wake Park (wake-park.nl) — seizoen april t/m oktober
        // Bron: Instagram/website; laagseizoen wo+vr 17-20, za-di 13-20; hoogseizoen dagelijks 13-20
        periods.AddRange(P(ids["beaver-creek"], (4, 1), (6, 30),
            (Monday,    13, 0, 20, 0),
            (Tuesday,   13, 0, 20, 0),
            (Wednesday, 17, 0, 20, 0),
            (Friday,    17, 0, 20, 0),
            (Saturday,  13, 0, 20, 0),
            (Sunday,    13, 0, 20, 0)));
        periods.AddRange(P(ids["beaver-creek"], (7, 1), (8, 31),
            (Monday,    13, 0, 20, 0),
            (Tuesday,   13, 0, 20, 0),
            (Wednesday, 13, 0, 20, 0),
            (Thursday,  13, 0, 20, 0),
            (Friday,    13, 0, 20, 0),
            (Saturday,  13, 0, 20, 0),
            (Sunday,    13, 0, 20, 0)));
        periods.AddRange(P(ids["beaver-creek"], (9, 1), (10, 31),
            (Monday,    13, 0, 20, 0),
            (Tuesday,   13, 0, 20, 0),
            (Wednesday, 17, 0, 20, 0),
            (Friday,    17, 0, 20, 0),
            (Saturday,  13, 0, 20, 0),
            (Sunday,    13, 0, 20, 0)));

        // Zeumeren Watersport (zeumerenwatersport.nl)
        // Bron: cableparks.nl juni + juli 2026
        periods.AddRange(P(ids["zeumeren-watersport"], (4, 1), (7, 17),
            (Tuesday,   14, 0, 21, 30),
            (Wednesday, 14, 0, 21, 30),
            (Thursday,  14, 0, 21, 30),
            (Friday,    14, 0, 21, 30),
            (Saturday,  12, 30, 20, 0),
            (Sunday,    12, 30, 20, 0)));
        periods.AddRange(P(ids["zeumeren-watersport"], (7, 18), (10, 31),
            (Monday,    12, 30, 21, 30),
            (Tuesday,   12, 30, 21, 30),
            (Wednesday, 12, 30, 21, 30),
            (Thursday,  12, 30, 21, 30),
            (Friday,    12, 30, 21, 30),
            (Saturday,  12, 30, 20, 0),
            (Sunday,    12, 30, 20, 0)));

        // Cablepark Aquabest (cableparkaquabest.nl)
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["cablepark-aquabest"], (4, 1), (6, 7),
            (Tuesday,   14, 0, 21, 0),
            (Wednesday, 14, 0, 20, 0),
            (Thursday,  14, 0, 21, 0),
            (Friday,    14, 0, 20, 0),
            (Saturday,  11, 0, 19, 0),
            (Sunday,    11, 0, 19, 0)));
        periods.AddRange(P(ids["cablepark-aquabest"], (6, 8), (7, 12),
            (Tuesday,   14, 0, 21, 0),
            (Wednesday, 16, 0, 20, 0),
            (Thursday,  16, 0, 21, 0),
            (Friday,    16, 0, 20, 0),
            (Saturday,  11, 0, 19, 0),
            (Sunday,    11, 0, 19, 0)));
        periods.AddRange(P(ids["cablepark-aquabest"], (7, 13), (10, 31),
            (Tuesday,   12, 0, 21, 0),
            (Wednesday, 12, 0, 21, 0),
            (Thursday,  12, 0, 21, 0),
            (Friday,    12, 0, 21, 0),
            (Saturday,  11, 0, 19, 0),
            (Sunday,    11, 0, 19, 0)));

        // Burnside Cablepark (burnsidecablepark.nl) — seizoen april t/m oktober
        // Bron: cableparks.nl juni/juli 2026
        periods.AddRange(P(ids["burnside"], (4, 1), (6, 30),
            (Tuesday,   15, 0, 21, 0),
            (Wednesday, 15, 0, 21, 0),
            (Thursday,  15, 0, 21, 0),
            (Friday,    15, 0, 21, 0),
            (Saturday,  12, 0, 20, 0),
            (Sunday,    12, 0, 20, 0)));
        periods.AddRange(P(ids["burnside"], (7, 1), (10, 31),
            (Tuesday,   12, 0, 21, 0),
            (Wednesday, 12, 0, 21, 0),
            (Thursday,  12, 0, 21, 0),
            (Friday,    12, 0, 21, 0),
            (Saturday,  12, 0, 20, 0),
            (Sunday,    12, 0, 20, 0)));

        // SKEEF / Luna Lake (lunalake.nl) — seizoen april t/m oktober
        // Bron: lunalake.nl juni 2026
        periods.AddRange(P(ids["skeef"], (4, 1), (10, 31),
            (Wednesday, 15, 0, 21, 0),
            (Thursday,  15, 0, 21, 0),
            (Friday,    15, 0, 21, 0),
            (Saturday,  10, 0, 19, 0),
            (Sunday,    10, 0, 19, 0)));

        return periods;
    }

    // Helper: maak openingsperiode-rijen voor meerdere dagen in één aanroep
    private static IEnumerable<OpeningPeriod> P(
        int locationId,
        (int Month, int Day) from,
        (int Month, int Day) until,
        params (DayOfWeek Day, int OpenH, int OpenM, int CloseH, int CloseM)[] schedule)
    {
        var validFrom  = new DateOnly(2026, from.Month,  from.Day);
        var validUntil = new DateOnly(2026, until.Month, until.Day);

        return schedule.Select(s => new OpeningPeriod
        {
            LocationId = locationId,
            ValidFrom  = validFrom,
            ValidUntil = validUntil,
            DayOfWeek  = s.Day,
            OpenTime   = new TimeOnly(s.OpenH, s.OpenM),
            CloseTime  = new TimeOnly(s.CloseH, s.CloseM),
        });
    }
}
