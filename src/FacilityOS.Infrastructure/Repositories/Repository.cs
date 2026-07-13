using FacilityOS.Shared;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Infrastructure;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _ctx;
    public Repository(AppDbContext ctx) => _ctx = ctx;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task AddAsync(T entity, CancellationToken ct = default)
    {
        _ctx.Set<T>().Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
}
