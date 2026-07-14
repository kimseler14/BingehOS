using BingehOS.Modules.Identity.Domain;
using Xunit;

namespace BingehOS.UnitTests.Identity;

public class IdentityManagementTests
{
    [Fact]
    public void User_Update_Deactivate_And_SoftDelete_Work()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "hash", "Original Name");

        user.Update("Updated Name");
        Assert.Equal("Updated Name", user.FullName);

        user.Deactivate();
        Assert.False(user.IsActive);

        user.SoftDelete();
        Assert.True(user.IsDeleted);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Role_Update_And_SoftDelete_Work()
    {
        var role = Role.Create(Guid.NewGuid(), "Operator", "Operator role");

        role.Update("Supervisor", "Supervisor role");
        Assert.Equal("Supervisor", role.Name);
        Assert.Equal("Supervisor role", role.Description);

        role.SoftDelete();
        Assert.True(role.IsDeleted);
    }

    [Fact]
    public void Permission_Create_Should_AssignTenantAndFields()
    {
        var tenantId = Guid.NewGuid();
        var permission = Permission.Create(tenantId, "users.write", "Manage users");

        Assert.Equal(tenantId, permission.TenantId);
        Assert.Equal("users.write", permission.Name);
        Assert.Equal("Manage users", permission.Description);
    }
}
