using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BingehOS.Api.Auth;

[ApiController]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _settings;
    public AuthController(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string AccessToken, string TokenType, int ExpiresIn);

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "username and password required" });

        if (req.Password != "admin")
            return Unauthorized(new { error = "invalid credentials" });

        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var service = new JwtTokenService(Options.Create(_settings));
        var token = service.Generate(userId, req.Username, new[] { "Admin" });

        return Ok(new LoginResponse(token.AccessToken, token.TokenType, token.ExpiresIn));
    }
}
