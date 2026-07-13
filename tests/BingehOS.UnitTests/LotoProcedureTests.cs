using BingehOS.Modules.HSE.Domain;

namespace BingehOS.UnitTests;

public class LotoProcedureTests
{
    [Fact]
    public void Create_Sets_Fields()
    {
        var tenant = Guid.NewGuid();
        var permitId = Guid.NewGuid();
        var loto = LotoProcedure.Create(tenant, "[\"Lock\", \"Tag\"]", permitId);
        Assert.Equal(tenant, loto.TenantId);
        Assert.False(loto.IsVerified);
        Assert.Equal(permitId, loto.PermitToWorkId);
    }

    [Fact]
    public void Verify_Sets_Verified()
    {
        var loto = LotoProcedure.Create(Guid.NewGuid(), "[\"Lock\", \"Tag\"]", Guid.NewGuid());
        loto.Verify(Guid.NewGuid());
        Assert.True(loto.IsVerified);
        Assert.NotNull(loto.VerifiedBy);
        Assert.NotNull(loto.VerifiedAt);
    }
}
