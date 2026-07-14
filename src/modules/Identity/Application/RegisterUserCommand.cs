using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Security;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record RegisterUserRequest(string Email, string Password, string FullName);
public record RegisterUserResponse(Guid UserId, string Email, string FullName);

public record RegisterUserCommand(RegisterUserRequest Request, Guid CreatedByUserId) : IRequest<RegisterUserResponse>;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(AppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;

        var exists = await _db.Set<User>()
            .AnyAsync(u => u.TenantId == tenantId && u.Email == cmd.Request.Email && !u.IsDeleted, ct);

        if (exists)
            throw new InvalidOperationException("Email already exists in this tenant.");

        var passwordHash = _passwordHasher.Hash(cmd.Request.Password);
        var user = User.Create(tenantId, cmd.Request.Email, passwordHash, cmd.Request.FullName);
        _db.Set<User>().Add(user);

        var defaultRole = await _db.Set<Role>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == "User" && !r.IsDeleted, ct);

        if (defaultRole != null)
        {
            _db.Set<UserRole>().Add(UserRole.Create(tenantId, user.Id, defaultRole.Id, cmd.CreatedByUserId));
        }

        await _db.SaveChangesAsync(ct);

        return new RegisterUserResponse(user.Id, user.Email, user.FullName);
    }
}
