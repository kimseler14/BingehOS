using BingehOS.Modules.Compliance.Domain;

namespace BingehOS.UnitTests;

public class TurkishWorkCalendarTests
{
    private readonly TurkishWorkCalendar _calendar = new();

    [Fact]
    public void IncludesFixedAndReligiousHolidaysFor2026()
    {
        var holidays = _calendar.GetHolidays(2026);

        Assert.Contains(holidays, h =>
            h.Date == new DateOnly(2026, 10, 29));
        Assert.Contains(holidays, h =>
            h.Date == new DateOnly(2026, 3, 20));
        Assert.Equal(14, holidays.Count);
    }

    [Fact]
    public void IsBusinessDayExcludesWeekendAndHoliday()
    {
        Assert.False(_calendar.IsBusinessDay(new DateOnly(2026, 1, 1)));
        Assert.False(_calendar.IsBusinessDay(new DateOnly(2026, 1, 3)));
        Assert.True(_calendar.IsBusinessDay(new DateOnly(2026, 1, 2)));
    }

    [Fact]
    public void MergesHolidaysThatShareADate()
    {
        var holidays = _calendar.GetHolidays(2027);

        Assert.Equal(13, holidays.Count);
        var may19 = Assert.Single(holidays.Where(h =>
            h.Date == new DateOnly(2027, 5, 19)));
        Assert.Contains("Atatürk", may19.Name);
        Assert.Contains("Kurban", may19.Name);
    }

    [Fact]
    public void AddBusinessDaysSkipsHolidayAndWeekend()
    {
        var result = _calendar.AddBusinessDays(new DateOnly(2026, 4, 22), 1);

        Assert.Equal(new DateOnly(2026, 4, 24), result);
    }

    [Fact]
    public void SupportsBusinessDaySubtraction()
    {
        var result = _calendar.AddBusinessDays(new DateOnly(2026, 4, 24), -1);

        Assert.Equal(new DateOnly(2026, 4, 22), result);
    }
}
