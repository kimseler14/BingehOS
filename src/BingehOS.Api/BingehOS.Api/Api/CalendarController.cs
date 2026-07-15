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
        if (requestedYear is < 1 or > 9999)
        {
            return BadRequest(new { success = false, error = "year must be between 1 and 9999" });
        }

        return this.OkWithData(new
        {
            year = requestedYear,
            holidays = _calendar.GetHolidays(requestedYear)
        });
    }
}
