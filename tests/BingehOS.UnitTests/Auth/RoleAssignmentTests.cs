using BingehOS.Modules.Identity.Domain;
using Xunit;

namespace BingehOS.UnitTests.Auth;

public class RoleAssignmentTests
{
    [Fact]
    public void UserRole_Create_Should_SetAssignedAtToUtcNow()
    {
        var tenant = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;
        var userRole = UserRole.Create(tenant, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var after = DateTimeOffset.UtcNow;

        Assert.True(userRole.AssignedAt >= before);
        Assert.True(userRole.AssignedAt <= after);
    }

    [Fact]
    public void Role_Create_DefaultIsSystemFalse()
    {
        var tenant = Guid.NewGuid();
        var role = Role.Create(tenant, "Regular", "Regular role");

        Assert.False(role.IsSystem);
    }

    [Fact]
    public void User_Activate_Deactivate_Should_ToggleIsActive()
    {
        var user = User.Create(Guid.NewGuid(), "toggle@example.com", "hash", "Toggle User");
        Assert.True(user.IsActive);

        user.Deactivate();
        Assert.False(user.IsActive);

        user.Activate();
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Permission_MultiplePermissions_CanBeCreated()
    {
        var tenant = Guid.NewGuid();
        var p1 = Permission.Create(tenant, "assets.read", "Read assets");
        var p2 = Permission.Create(tenant, "assets.write", "Write assets");
        var p3 = Permission.Create(tenant, "work_orders.read", "Read work orders");

        Assert.Equal(tenant, p1.TenantId);
        Assert.Equal(tenant, p2.TenantId);
        Assert.Equal(tenant, p3.TenantId);
        Assert.NotEqual(p1.Id, p2.Id);
        Assert.NotEqual(p2.Id, p3.Id);
    }
}
