using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Security;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string AccessToken, string TokenType, int ExpiresIn, Guid UserId, string Email, string FullName, IReadOnlyList<string> Roles);

public record LoginQuery(LoginRequest Request) : IRequest<LoginResponse>;

public class LoginHandler : IRequestHandler<LoginQuery, LoginResponse>
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginHandler(AppDbContext db, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> Handle(LoginQuery q, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;

        var user = await _db.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Email == q.Request.Email && !u.IsDeleted, ct);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_passwordHasher.Verify(q.Request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roleNames = user.UserRoles
            .Where(ur => !ur.IsDeleted && ur.Role != null && !ur.Role.IsDeleted)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToList();
        var token = _tokenService.GenerateToken(user.Id, user.Email, roleNames, tenantId);

        return new LoginResponse(
            token.AccessToken,
            token.TokenType,
            token.ExpiresIn,
            user.Id,
            user.Email,
            user.FullName,
            roleNames);
    }
}
