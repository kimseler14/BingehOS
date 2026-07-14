using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record RoleDto(Guid Id, string Name, string? Description, bool IsSystem, IReadOnlyList<string> Permissions);

public record GetRolesQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<RoleDto>>;
public record GetRoleQuery(Guid Id) : IRequest<RoleDto?>;
public record CreateRoleCommand(string Name, string? Description, bool IsSystem = false) : IRequest<Guid>;
public record UpdateRoleCommand(Guid Id, string Name, string? Description) : IRequest<RoleDto>;
public record DeleteRoleCommand(Guid Id) : IRequest<Unit>;
public record AddPermissionToRoleCommand(Guid RoleId, Guid PermissionId) : IRequest<Unit>;
public record RemovePermissionFromRoleCommand(Guid RoleId, Guid PermissionId) : IRequest<Unit>;

public class GetRolesHandler : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDto>>
{
    private readonly AppDbContext _db;
    public GetRolesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<RoleDto>> Handle(GetRolesQuery q, CancellationToken ct)
    {
        var roles = await _db.Set<Role>()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.TenantId == _db.CurrentTenantId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .ToListAsync(ct);

        return roles.Select(Map).ToList();
    }

    private static RoleDto Map(Role role) => new(
        role.Id,
        role.Name,
        role.Description,
        role.IsSystem,
        role.RolePermissions
            .Where(rp => !rp.IsDeleted && rp.Permission != null && !rp.Permission.IsDeleted)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToList());
}

public class GetRoleHandler : IRequestHandler<GetRoleQuery, RoleDto?>
{
    private readonly AppDbContext _db;
    public GetRoleHandler(AppDbContext db) => _db = db;

    public async Task<RoleDto?> Handle(GetRoleQuery q, CancellationToken ct)
    {
        var role = await _db.Set<Role>()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == q.Id && r.TenantId == _db.CurrentTenantId && !r.IsDeleted, ct);

        return role == null ? null : new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.IsSystem,
            role.RolePermissions.Where(rp => !rp.IsDeleted && rp.Permission != null && !rp.Permission.IsDeleted)
                .Select(rp => rp.Permission!.Name)
                .Distinct()
                .ToList());
    }
}

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateRoleHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateRoleCommand cmd, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;
        var exists = await _db.Set<Role>().AnyAsync(r => r.TenantId == tenantId && r.Name == cmd.Name && !r.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException("Role already exists in this tenant.");

        var role = Role.Create(tenantId, cmd.Name, cmd.Description, cmd.IsSystem);
        _db.Set<Role>().Add(role);
        await _db.SaveChangesAsync(ct);
        return role.Id;
    }
}

public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly AppDbContext _db;
    public UpdateRoleHandler(AppDbContext db) => _db = db;

    public async Task<RoleDto> Handle(UpdateRoleCommand cmd, CancellationToken ct)
    {
        var role = await _db.Set<Role>()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == cmd.Id && r.TenantId == _db.CurrentTenantId && !r.IsDeleted, ct);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        role.Update(cmd.Name, cmd.Description);
        await _db.SaveChangesAsync(ct);
        return new RoleDto(role.Id, role.Name, role.Description, role.IsSystem, role.RolePermissions
            .Where(rp => !rp.IsDeleted && rp.Permission != null && !rp.Permission.IsDeleted)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToList());
    }
}

public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Unit>
{
    private readonly AppDbContext _db;
    public DeleteRoleHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteRoleCommand cmd, CancellationToken ct)
    {
        var role = await _db.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == cmd.Id && r.TenantId == _db.CurrentTenantId && !r.IsDeleted, ct);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        role.SoftDelete();
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

public class AddPermissionToRoleHandler : IRequestHandler<AddPermissionToRoleCommand, Unit>
{
    private readonly AppDbContext _db;
    public AddPermissionToRoleHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(AddPermissionToRoleCommand cmd, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;
        var role = await _db.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == cmd.RoleId && r.TenantId == tenantId && !r.IsDeleted, ct);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        var permission = await _db.Set<Permission>()
            .FirstOrDefaultAsync(p => p.Id == cmd.PermissionId && p.TenantId == tenantId && !p.IsDeleted, ct);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        var exists = await _db.Set<RolePermission>()
            .AnyAsync(rp => rp.RoleId == cmd.RoleId && rp.PermissionId == cmd.PermissionId && !rp.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException("Permission already assigned to role.");

        _db.Set<RolePermission>().Add(RolePermission.Create(tenantId, role.Id, permission.Id));
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

public class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand, Unit>
{
    private readonly AppDbContext _db;
    public RemovePermissionFromRoleHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(RemovePermissionFromRoleCommand cmd, CancellationToken ct)
    {
        var link = await _db.Set<RolePermission>()
            .FirstOrDefaultAsync(rp => rp.RoleId == cmd.RoleId && rp.PermissionId == cmd.PermissionId && rp.TenantId == _db.CurrentTenantId && !rp.IsDeleted, ct);
        if (link == null)
            throw new KeyNotFoundException("Permission assignment not found.");

        link.SoftDelete();
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
