using BingehOS.Infrastructure;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record AssignRoleRequest(Guid UserId, Guid RoleId);
public record AssignRoleResponse(Guid UserId, Guid RoleId);

public record AssignRoleCommand(AssignRoleRequest Request, Guid AssignedByUserId) : IRequest<AssignRoleResponse>;

public class AssignRoleHandler : IRequestHandler<AssignRoleCommand, AssignRoleResponse>
{
    private readonly AppDbContext _db;

    public AssignRoleHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AssignRoleResponse> Handle(AssignRoleCommand cmd, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;

        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == cmd.Request.UserId && u.TenantId == tenantId, ct);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var role = await _db.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == cmd.Request.RoleId && r.TenantId == tenantId, ct);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        var exists = await _db.Set<UserRole>()
            .AnyAsync(ur => ur.UserId == cmd.Request.UserId && ur.RoleId == cmd.Request.RoleId, ct);

        if (exists)
            throw new InvalidOperationException("Role already assigned to user.");

        _db.Set<UserRole>().Add(UserRole.Create(tenantId, user.Id, role.Id, cmd.AssignedByUserId));
        await _db.SaveChangesAsync(ct);

        return new AssignRoleResponse(user.Id, role.Id);
    }
}
