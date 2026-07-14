using MediatR;
using Microsoft.AspNetCore.Mvc;
using BingehOS.Modules.Identity.Application;

namespace BingehOS.Api.Auth;

[ApiController]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    public record RegisterRequest(string Email, string Password, string FullName);
    public record LoginReq(string Email, string Password);
    public record AssignRoleReq(Guid UserId, Guid RoleId);

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { success = false, error = "email and password are required" });

        var createdBy = User.GetUserId();
        var result = await _mediator.Send(new RegisterUserCommand(
            new RegisterUserRequest(req.Email, req.Password, req.FullName), createdBy), ct);

        return CreatedAtAction(nameof(Login), null, new { success = true, data = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReq req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { success = false, error = "email and password are required" });

        try
        {
            var result = await _mediator.Send(new LoginQuery(new LoginRequest(req.Email, req.Password)), ct);
            return this.OkWithData(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "invalid credentials" });
        }
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleReq req, CancellationToken ct)
    {
        var assignedBy = User.GetUserId();
        var result = await _mediator.Send(new AssignRoleCommand(
            new AssignRoleRequest(req.UserId, req.RoleId), assignedBy), ct);

        return this.OkWithData(result);
    }
}
