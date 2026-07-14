using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record PermissionDto(Guid Id, string Name, string? Description);
public record GetPermissionsQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<PermissionDto>>;
public record GetPermissionQuery(Guid Id) : IRequest<PermissionDto?>;
public record CreatePermissionCommand(string Name, string? Description) : IRequest<Guid>;

public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    private readonly AppDbContext _db;
    public GetPermissionsHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PermissionDto>> Handle(GetPermissionsQuery q, CancellationToken ct)
    {
        return await _db.Set<Permission>()
            .Where(p => p.TenantId == _db.CurrentTenantId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .ToListAsync(ct);
    }
}

public class GetPermissionHandler : IRequestHandler<GetPermissionQuery, PermissionDto?>
{
    private readonly AppDbContext _db;
    public GetPermissionHandler(AppDbContext db) => _db = db;

    public async Task<PermissionDto?> Handle(GetPermissionQuery q, CancellationToken ct)
    {
        return await _db.Set<Permission>()
            .Where(p => p.Id == q.Id && p.TenantId == _db.CurrentTenantId && !p.IsDeleted)
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .FirstOrDefaultAsync(ct);
    }
}

public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreatePermissionHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreatePermissionCommand cmd, CancellationToken ct)
    {
        var tenantId = _db.CurrentTenantId;
        var exists = await _db.Set<Permission>().AnyAsync(p => p.TenantId == tenantId && p.Name == cmd.Name && !p.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException("Permission already exists in this tenant.");

        var permission = Permission.Create(tenantId, cmd.Name, cmd.Description);
        _db.Set<Permission>().Add(permission);
        await _db.SaveChangesAsync(ct);
        return permission.Id;
    }
}
