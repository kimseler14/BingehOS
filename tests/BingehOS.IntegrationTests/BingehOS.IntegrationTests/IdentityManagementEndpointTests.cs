using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Identity.Application;
using Xunit;

namespace BingehOS.IntegrationTests;

public class IdentityManagementEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public IdentityManagementEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Users_Roles_And_Permissions_Can_Be_Managed()
    {
        await using var admin = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var permissionName = $"identity.manage.{Guid.NewGuid():N}";
        var createPermission = await admin.Client.PostAsJsonAsync(
            "/v1/permissions",
            new CreatePermissionCommand(permissionName, "Temporary identity permission"));
        Assert.Equal(HttpStatusCode.Created, createPermission.StatusCode);
        var createdPermissionBody = await createPermission.Content.ReadFromJsonAsync<JsonElement>();
        var tempPermissionId = createdPermissionBody.GetProperty("data").GetProperty("id").GetGuid();

        var permissions = await admin.Client.GetAsync("/v1/permissions?take=200");
        Assert.Equal(HttpStatusCode.OK, permissions.StatusCode);
        var permissionsBody = await permissions.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            permissionsBody.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("name").GetString() == permissionName);

        var createRole = await admin.Client.PostAsJsonAsync(
            "/v1/roles",
            new CreateRoleCommand("IdentityManager", "Identity management role"));
        Assert.Equal(HttpStatusCode.Created, createRole.StatusCode);
        var roleBody = await createRole.Content.ReadFromJsonAsync<JsonElement>();
        var roleId = roleBody.GetProperty("data").GetProperty("id").GetGuid();

        var seedEmail = $"identity-{Guid.NewGuid():N}@example.com";
        await admin.SeedUserAsync(Guid.Parse("11111111-1111-1111-1111-111111111111"), seedEmail, "SecurePass123!", "Identity User");

        var usersList = await admin.Client.GetAsync("/v1/users");
        Assert.Equal(HttpStatusCode.OK, usersList.StatusCode);
        var usersListBody = await usersList.Content.ReadFromJsonAsync<JsonElement>();
        var userId = usersListBody.GetProperty("data").EnumerateArray()
            .First(item => item.GetProperty("email").GetString() == seedEmail)
            .GetProperty("id").GetGuid();

        var assignRole = await admin.Client.PostAsJsonAsync(
            "/v1/auth/assign-role",
            new { UserId = userId, RoleId = roleId });
        Assert.Equal(HttpStatusCode.OK, assignRole.StatusCode);

        var getUser = await admin.Client.GetAsync($"/v1/users/{userId}");
        Assert.Equal(HttpStatusCode.OK, getUser.StatusCode);

        var patchUser = await admin.Client.PatchAsJsonAsync(
            $"/v1/users/{userId}",
            new UpdateUserCommand(userId, "Identity User Updated", false));
        Assert.Equal(HttpStatusCode.OK, patchUser.StatusCode);

        var rolesList = await admin.Client.GetAsync("/v1/roles");
        Assert.Equal(HttpStatusCode.OK, rolesList.StatusCode);
        var rolesListBody = await rolesList.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            rolesListBody.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("id").GetGuid() == roleId);

        var roleGet = await admin.Client.GetAsync($"/v1/roles/{roleId}");
        Assert.Equal(HttpStatusCode.OK, roleGet.StatusCode);

        var updateRole = await admin.Client.PatchAsJsonAsync(
            $"/v1/roles/{roleId}",
            new UpdateRoleCommand(roleId, "IdentityManagerRenamed", "Updated identity role"));
        Assert.Equal(HttpStatusCode.OK, updateRole.StatusCode);

        var userClient = admin.CreateClient(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        userClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                await admin.CreateBearerTokenAsync(Guid.Parse("11111111-1111-1111-1111-111111111111"), userId));

        var forbiddenUsers = await userClient.GetAsync("/v1/users");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenUsers.StatusCode);

        var forbiddenRoles = await userClient.GetAsync("/v1/roles");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenRoles.StatusCode);

        var forbiddenPermissions = await userClient.GetAsync("/v1/permissions");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenPermissions.StatusCode);

        var forbidden = await userClient.PostAsJsonAsync(
            "/v1/roles",
            new CreateRoleCommand("ShouldFail", "Should not be allowed"));
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);

        var adminAccessPermission = permissionsBody.GetProperty("data").EnumerateArray()
            .First(item => item.GetProperty("name").GetString() == "admin.access")
            .GetProperty("id").GetGuid();

        var grantAdminAccess = await admin.Client.PostAsync(
            $"/v1/roles/{roleId}/permissions/{adminAccessPermission}",
            null);
        Assert.Equal(HttpStatusCode.NoContent, grantAdminAccess.StatusCode);

        var allowed = await userClient.PostAsJsonAsync(
            "/v1/roles",
            new CreateRoleCommand("NowAllowed", "Now allowed"));
        Assert.Equal(HttpStatusCode.Created, allowed.StatusCode);

        var deleteUser = await admin.Client.DeleteAsync($"/v1/users/{userId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteUser.StatusCode);

        var deletedUser = await admin.Client.GetAsync($"/v1/users/{userId}");
        Assert.Equal(HttpStatusCode.NotFound, deletedUser.StatusCode);

        var removePermission = await admin.Client.DeleteAsync($"/v1/roles/{roleId}/permissions/{adminAccessPermission}");
        Assert.Equal(HttpStatusCode.NoContent, removePermission.StatusCode);

        var deleteRole = await admin.Client.DeleteAsync($"/v1/roles/{roleId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRole.StatusCode);

        var deletedRole = await admin.Client.GetAsync($"/v1/roles/{roleId}");
        Assert.Equal(HttpStatusCode.NotFound, deletedRole.StatusCode);
    }
}
