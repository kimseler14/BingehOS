using FacilityOS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FacilityOS.Infrastructure;

public class TenantInterceptor : SaveChangesInterceptor
{
    private readonly AppDbContext _ctx;
    public TenantInterceptor(AppDbContext ctx) => _ctx = ctx;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        foreach (var entry in _ctx.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.TenantId == Guid.Empty)
                    entry.Entity.TenantId = _ctx.CurrentTenantId;
                if (entry.Entity.TenantId == Guid.Empty)
                    throw new InvalidOperationException("TenantId must be set before insert.");
            }
            entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
        return base.SavingChanges(eventData, result);
    }
}
