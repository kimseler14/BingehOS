using Microsoft.AspNetCore.Mvc;

namespace BingehOS.Api.Responses;

public static class ControllerResponseExtensions
{
    public static IActionResult CreatedWithId(this ControllerBase controller, string actionName, Guid id) =>
        controller.CreatedAtAction(actionName, new { id }, new { success = true, data = new { id } });

    public static IActionResult OkWithData<T>(this ControllerBase controller, T data) =>
        controller.Ok(new { success = true, data });

    public static IActionResult OkOrNotFound<T>(this ControllerBase controller, T? data)
        where T : class =>
        data is null
            ? controller.NotFound(new { success = false, error = "not found" })
            : controller.OkWithData(data);

    public static IActionResult IdMismatch(this ControllerBase controller) =>
        controller.BadRequest(new { error = "id mismatch" });
}
