using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BingehOS.Api;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.Parse(sub ?? Guid.Empty.ToString());
    }
}
