using BingehOS.Infrastructure;
using BingehOS.Infrastructure.Queries;
using BingehOS.Modules.Identity.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Identity.Application;

public record GetUsersQuery(int Skip = 0, int Take = 20) : IRequest<IReadOnlyList<UserDto>>;
public record GetUserQuery(Guid Id) : IRequest<UserDto?>;
public record UpdateUserCommand(Guid Id, string FullName, bool IsActive) : IRequest<UserDto>;
public record DeleteUserCommand(Guid Id) : IRequest<Unit>;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly AppDbContext _db;
    public GetUsersHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery q, CancellationToken ct)
    {
        var users = await _db.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.TenantId == _db.CurrentTenantId && !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ApplyPaging(q.Skip, q.Take)
            .ToListAsync(ct);

        return users.Select(ToDto).ToList();
    }

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        IsActive = user.IsActive,
        Roles = user.UserRoles
            .Where(ur => !ur.IsDeleted && ur.Role != null && !ur.Role.IsDeleted)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToList()
    };
}

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly AppDbContext _db;
    public GetUserHandler(AppDbContext db) => _db = db;

    public async Task<UserDto?> Handle(GetUserQuery q, CancellationToken ct)
    {
        var user = await _db.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == q.Id && u.TenantId == _db.CurrentTenantId && !u.IsDeleted, ct);

        return user == null ? null : Map(user);
    }

    private static UserDto Map(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        IsActive = user.IsActive,
        Roles = user.UserRoles
            .Where(ur => !ur.IsDeleted && ur.Role != null && !ur.Role.IsDeleted)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToList()
    };
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly AppDbContext _db;
    public UpdateUserHandler(AppDbContext db) => _db = db;

    public async Task<UserDto> Handle(UpdateUserCommand cmd, CancellationToken ct)
    {
        var user = await _db.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == cmd.Id && u.TenantId == _db.CurrentTenantId && !u.IsDeleted, ct);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Update(cmd.FullName);
        if (cmd.IsActive) user.Activate(); else user.Deactivate();
        await _db.SaveChangesAsync(ct);

        return Map(user);
    }

    private static UserDto Map(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        IsActive = user.IsActive,
        Roles = user.UserRoles
            .Where(ur => !ur.IsDeleted && ur.Role != null && !ur.Role.IsDeleted)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToList()
    };
}

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly AppDbContext _db;
    public DeleteUserHandler(AppDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteUserCommand cmd, CancellationToken ct)
    {
        var user = await _db.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == cmd.Id && u.TenantId == _db.CurrentTenantId && !u.IsDeleted, ct);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.SoftDelete();
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
