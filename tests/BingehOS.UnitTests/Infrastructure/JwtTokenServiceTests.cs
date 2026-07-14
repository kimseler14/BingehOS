using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BingehOS.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BingehOS.UnitTests.Infrastructure;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateService(out JwtSettings settings)
    {
        settings = new JwtSettings
        {
            Secret = "this-is-a-very-long-test-secret-key-32-chars!!",
            Issuer = "BingehOS.Tests",
            Audience = "BingehOS.Tests.Audience",
            ExpiresInSeconds = 3600
        };
        return new JwtTokenService(Options.Create(settings));
    }

    [Fact]
    public void GenerateToken_Returns_NonEmpty_Token_With_Three_Segments()
    {
        var service = CreateService(out _);
        var response = service.GenerateToken(Guid.NewGuid(), "user@example.com", new[] { "Admin" }, Guid.NewGuid());

        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.Equal(3, response.AccessToken.Split('.').Length);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal(3600, response.ExpiresIn);
    }

    [Fact]
    public void GenerateToken_Produces_Validatable_Token_With_Expected_Claims()
    {
        var service = CreateService(out var settings);
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var tenantId = Guid.NewGuid();
        var roles = new[] { "Admin", "Operator" };

        var response = service.GenerateToken(userId, email, roles, tenantId);

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.MapInboundClaims = false;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var principal = tokenHandler.ValidateToken(response.AccessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudience = settings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        Assert.IsType<JwtSecurityToken>(validatedToken);

        var claims = principal.Claims.ToList();
        Assert.Equal(userId.ToString(), claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(email, claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal(tenantId.ToString(), claims.First(c => c.Type == "tenant_id").Value);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "Operator");

        var jwt = (JwtSecurityToken)validatedToken;
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}
