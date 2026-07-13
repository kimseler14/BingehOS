using BingehOS.Infrastructure.Audit;
using BingehOS.Modules.Audit.Domain;
using BingehOS.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BingehOS.Infrastructure;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly AppDbContext _ctx;

    public AuditInterceptor(AppDbContext ctx) => _ctx = ctx;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChanges(eventData, result);

        var tenantId = _ctx.CurrentTenantId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.Entity is AuditLog)
                continue;

            var entityName = entry.Metadata.Name;
            var entityId = entry.Entity.Id;

            switch (entry.State)
            {
                case EntityState.Added:
                    var createLog = AuditLog.Create(
                        tenantId,
                        entityName,
                        entityId,
                        AuditAction.Created,
                        null,
                        null,
                        System.Text.Json.JsonSerializer.Serialize(entry.Entity));
                    context.Entry(createLog).State = EntityState.Added;
                    break;

                case EntityState.Modified:
                    var oldValues = System.Text.Json.JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    var newValues = System.Text.Json.JsonSerializer.Serialize(entry.Entity);
                    var updateLog = AuditLog.Create(
                        tenantId,
                        entityName,
                        entityId,
                        AuditAction.Updated,
                        null,
                        oldValues,
                        newValues);
                    context.Entry(updateLog).State = EntityState.Added;
                    break;

                case EntityState.Deleted:
                    var deleteLog = AuditLog.Create(
                        tenantId,
                        entityName,
                        entityId,
                        AuditAction.Deleted,
                        null,
                        System.Text.Json.JsonSerializer.Serialize(entry.Entity),
                        null);
                    context.Entry(deleteLog).State = EntityState.Added;
                    break;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}