using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BingehOS.Api.Auth;

public record TokenResponse(string AccessToken, string TokenType = "Bearer", int ExpiresIn = 3600);

public class JwtTokenService
{
    private readonly JwtSettings _settings;
    public JwtTokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public TokenResponse Generate(Guid userId, string username, IReadOnlyCollection<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_settings.ExpiresInSeconds),
            signingCredentials: creds);

        return new TokenResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn: _settings.ExpiresInSeconds);
    }
}
