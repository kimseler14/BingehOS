using BingehOS.Modules.Audit.Domain;
using Xunit;

namespace BingehOS.UnitTests.Audit;

public class AuditLogTests
{
    [Fact]
    public void AuditLog_Create_Sets_Expected_Fields()
    {
        var tenant = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var log = AuditLog.Create(tenant, "WorkOrder", entityId, AuditAction.Created, "admin@system", null, "{\"Name\":\"Test WO\"}");

        Assert.Equal(tenant, log.TenantId);
        Assert.Equal("WorkOrder", log.EntityName);
        Assert.Equal(entityId, log.EntityId);
        Assert.Equal(AuditAction.Created, log.Action);
        Assert.Equal("admin@system", log.ChangedBy);
        Assert.NotNull(log.NewValues);
        Assert.Null(log.OldValues);
    }

    [Fact]
    public void AuditLog_Update_Sets_Both_Old_And_New_Values()
    {
        var tenant = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var log = AuditLog.Create(tenant, "Asset", entityId, AuditAction.Updated, "admin@system", "{\"Name\":\"Old\"}", "{\"Name\":\"New\"}");

        Assert.Equal(AuditAction.Updated, log.Action);
        Assert.NotNull(log.OldValues);
        Assert.NotNull(log.NewValues);
    }

    [Fact]
    public void AuditLog_Delete_Sets_Only_Old_Values()
    {
        var tenant = Guid.NewGuid();
        var entityId = Guid.NewGuid();
        var log = AuditLog.Create(tenant, "Asset", entityId, AuditAction.Deleted, "admin@system", "{\"Name\":\"Deleted Asset\"}", null);

        Assert.Equal(AuditAction.Deleted, log.Action);
        Assert.NotNull(log.OldValues);
        Assert.Null(log.NewValues);
    }

    [Fact]
    public void AuditLog_ChangedAt_Is_UtcNow()
    {
        var tenant = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;
        var log = AuditLog.Create(tenant, "Asset", Guid.NewGuid(), AuditAction.Created, null, null, "{}");
        var after = DateTimeOffset.UtcNow;

        Assert.True(log.ChangedAt >= before);
        Assert.True(log.ChangedAt <= after);
    }

    [Fact]
    public void AuditLog_All_Actions_Have_Distinct_Values()
    {
        Assert.NotEqual(AuditAction.Created, AuditAction.Updated);
        Assert.NotEqual(AuditAction.Updated, AuditAction.Deleted);
        Assert.NotEqual(AuditAction.Created, AuditAction.Deleted);
    }
}
