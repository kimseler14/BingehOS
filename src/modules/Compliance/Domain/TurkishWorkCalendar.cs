namespace BingehOS.Modules.Compliance.Domain;

public sealed record TurkishHoliday(DateOnly Date, string Name);

public sealed class TurkishWorkCalendar
{
    private static readonly (int Month, int Day, string Name)[] FixedHolidays =
    [
        (1, 1, "Yılbaşı"),
        (4, 23, "Ulusal Egemenlik ve Çocuk Bayramı"),
        (5, 1, "Emek ve Dayanışma Günü"),
        (5, 19, "Atatürk'ü Anma, Gençlik ve Spor Bayramı"),
        (7, 15, "Demokrasi ve Millî Birlik Günü"),
        (8, 30, "Zafer Bayramı"),
        (10, 29, "Cumhuriyet Bayramı")
    ];

    private static readonly IReadOnlyDictionary<int, (DateOnly Date, string Name)[]> ReligiousHolidays =
        new Dictionary<int, (DateOnly Date, string Name)[]>
        {
            [2026] =
            [
                (new DateOnly(2026, 3, 20), "Ramazan Bayramı 1. Gün"),
                (new DateOnly(2026, 3, 21), "Ramazan Bayramı 2. Gün"),
                (new DateOnly(2026, 3, 22), "Ramazan Bayramı 3. Gün"),
                (new DateOnly(2026, 5, 27), "Kurban Bayramı 1. Gün"),
                (new DateOnly(2026, 5, 28), "Kurban Bayramı 2. Gün"),
                (new DateOnly(2026, 5, 29), "Kurban Bayramı 3. Gün"),
                (new DateOnly(2026, 5, 30), "Kurban Bayramı 4. Gün")
            ],
            [2027] =
            [
                (new DateOnly(2027, 3, 9), "Ramazan Bayramı 1. Gün"),
                (new DateOnly(2027, 3, 10), "Ramazan Bayramı 2. Gün"),
                (new DateOnly(2027, 3, 11), "Ramazan Bayramı 3. Gün"),
                (new DateOnly(2027, 5, 16), "Kurban Bayramı 1. Gün"),
                (new DateOnly(2027, 5, 17), "Kurban Bayramı 2. Gün"),
                (new DateOnly(2027, 5, 18), "Kurban Bayramı 3. Gün"),
                (new DateOnly(2027, 5, 19), "Kurban Bayramı 4. Gün")
            ]
        };

    public IReadOnlyList<TurkishHoliday> GetHolidays(int year)
    {
        if (year is < 1 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        var holidays = FixedHolidays
            .Select(h => new TurkishHoliday(
                new DateOnly(year, h.Month, h.Day),
                h.Name))
            .Concat(
                ReligiousHolidays.TryGetValue(year, out var religious)
                    ? religious.Select(h => new TurkishHoliday(h.Date, h.Name))
                    : [])
            .GroupBy(h => h.Date)
            .Select(group => new TurkishHoliday(
                group.Key,
                string.Join(" / ", group.Select(h => h.Name))))
            .OrderBy(h => h.Date)
            .ToList();

        return holidays;
    }

    public bool IsBusinessDay(DateOnly date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return false;
        }

        return GetHolidays(date.Year).All(h => h.Date != date);
    }

    public DateOnly AddBusinessDays(DateOnly date, int businessDays)
    {
        var direction = Math.Sign(businessDays);
        var remaining = Math.Abs(businessDays);
        var result = date;

        while (remaining > 0)
        {
            result = result.AddDays(direction);
            if (IsBusinessDay(result))
            {
                remaining--;
            }
        }

        return result;
    }
}
