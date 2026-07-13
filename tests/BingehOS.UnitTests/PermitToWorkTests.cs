using BingehOS.Modules.HSE.Domain;

namespace BingehOS.UnitTests;

public class PermitToWorkTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var permit = PermitToWork.Create(tenant, "Hot Work", "Welding permit", Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(tenant, permit.TenantId);
        Assert.Equal("Hot Work", permit.Title);
        Assert.Equal("Pending", permit.Status);
    }

    [Fact]
    public void Approve_Sets_Status()
    {
        var permit = PermitToWork.Create(Guid.NewGuid(), "Hot Work", "Welding permit", Guid.NewGuid(), Guid.NewGuid());
        permit.Approve(Guid.NewGuid());
        Assert.Equal("Approved", permit.Status);
    }

    [Fact]
    public void Reject_Sets_Status()
    {
        var permit = PermitToWork.Create(Guid.NewGuid(), "Hot Work", "Welding permit", Guid.NewGuid(), Guid.NewGuid());
        permit.Reject(Guid.NewGuid(), "Unsafe conditions");
        Assert.Equal("Rejected", permit.Status);
        Assert.Equal("Unsafe conditions", permit.RejectionReason);
    }
}
