using BingehOS.Modules.Compliance.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Api;

[ApiController]
[Route("v1/calendar")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly TurkishWorkCalendar _calendar;

    public CalendarController(TurkishWorkCalendar calendar) => _calendar = calendar;

    [HttpGet("holidays")]
    public IActionResult Holidays([FromQuery] int? year = null)
    {
        var requestedYear = year ?? DateTime.UtcNow.Year;
        return this.OkWithData(new
        {
            year = requestedYear,
            holidays = _calendar.GetHolidays(requestedYear)
        });
    }
}
