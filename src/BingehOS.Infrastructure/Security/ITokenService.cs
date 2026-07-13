using System.Security.Claims;

namespace BingehOS.Infrastructure.Security;

public interface ITokenService
{
    TokenResponse GenerateToken(Guid userId, string email, IReadOnlyCollection<string> roles, Guid tenantId);
}
