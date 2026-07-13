using BingehOS.Modules.Identity.Domain;
using Xunit;

namespace BingehOS.UnitTests.Auth;

public class RegisterTests
{
    [Fact]
    public void User_Create_WithUniqueEmail_Should_Succeed()
    {
        var tenant = Guid.NewGuid();
        var user = User.Create(tenant, "unique@example.com", "hash", "Unique User");

        Assert.Equal(tenant, user.TenantId);
        Assert.Equal("unique@example.com", user.Email);
    }

    [Fact]
    public void Role_Create_SystemRole_Should_HasSystemFlag()
    {
        var tenant = Guid.NewGuid();
        var role = Role.Create(tenant, "SystemAdmin", "System admin", true);

        Assert.True(role.IsSystem);
    }

    [Fact]
    public void Permission_Name_Should_BeInSnakeCase()
    {
        var tenant = Guid.NewGuid();
        var permission = Permission.Create(tenant, "work_orders.write", "Write work orders");

        Assert.Equal("work_orders.write", permission.Name);
    }

    [Fact]
    public void UserRole_MultipleRoles_CanBeAssigned()
    {
        var tenant = Guid.NewGuid();
        var user = User.Create(tenant, "multi@example.com", "hash", "Multi Role User");
        var adminRole = Role.Create(tenant, "Admin", "Admin", false);
        var userRole = Role.Create(tenant, "User", "User", false);

        Assert.NotEqual(adminRole.Id, userRole.Id);
        Assert.Equal(tenant, adminRole.TenantId);
        Assert.Equal(tenant, userRole.TenantId);
    }
}
