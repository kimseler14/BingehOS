using BingehOS.Infrastructure.Security;
using BingehOS.Modules.Identity.Domain;
using Xunit;

namespace BingehOS.UnitTests.Auth;

public class LoginTests
{
    [Fact]
    public void PasswordHasher_Hash_Then_Verify_ReturnsTrue()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("SecurePass123!");
        Assert.True(hasher.Verify("SecurePass123!", hash));
        Assert.False(hasher.Verify("WrongPass", hash));
    }

    [Fact]
    public void User_Create_Sets_Expected_Fields()
    {
        var tenant = Guid.NewGuid();
        var user = User.Create(tenant, "test@example.com", "hash", "Test User");

        Assert.Equal(tenant, user.TenantId);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Test User", user.FullName);
        Assert.Equal("hash", user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.Equal(AuthProvider.Local, user.AuthProvider);
    }

    [Fact]
    public void User_Deactivate_Sets_IsActive_False()
    {
        var user = User.Create(Guid.NewGuid(), "test@example.com", "hash", "Test User");
        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Role_Create_Sets_Expected_Fields()
    {
        var tenant = Guid.NewGuid();
        var role = Role.Create(tenant, "Admin", "Administrator", true);

        Assert.Equal(tenant, role.TenantId);
        Assert.Equal("Admin", role.Name);
        Assert.Equal("Administrator", role.Description);
        Assert.True(role.IsSystem);
    }

    [Fact]
    public void Permission_Create_Sets_Expected_Fields()
    {
        var tenant = Guid.NewGuid();
        var permission = Permission.Create(tenant, "assets.read", "Read assets");

        Assert.Equal(tenant, permission.TenantId);
        Assert.Equal("assets.read", permission.Name);
        Assert.Equal("Read assets", permission.Description);
    }

    [Fact]
    public void UserRole_Create_Sets_Expected_Fields()
    {
        var tenant = Guid.NewGuid();
        var userRole = UserRole.Create(tenant, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(tenant, userRole.TenantId);
        Assert.NotEqual(Guid.Empty, userRole.UserId);
        Assert.NotEqual(Guid.Empty, userRole.RoleId);
        Assert.NotEqual(Guid.Empty, userRole.AssignedByUserId);
        Assert.True(userRole.AssignedAt <= DateTimeOffset.UtcNow);
    }
}
