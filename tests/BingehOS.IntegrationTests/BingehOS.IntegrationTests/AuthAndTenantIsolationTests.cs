using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Asset.Application;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BingehOS.IntegrationTests;

public class AuthAndTenantIsolationTests : IClassFixture<TestContainerFixture>
{
    private static readonly Guid TenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly TestContainerFixture _fx;

    public AuthAndTenantIsolationTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task RegisterLoginAndPermissionPolicy_EnforceAccess()
    {
        await using var admin = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var email = $"user-{Guid.NewGuid():N}@example.com";

        var register = await admin.Client.PostAsJsonAsync(
            "/v1/auth/register",
            new { Email = email, Password = "SecurePass123!", FullName = "Regular User" });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        using var userClient = admin.CreateClient(TenantA);
        var login = await userClient.PostAsJsonAsync(
            "/v1/auth/login",
            new { Email = email, Password = "SecurePass123!" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var loginBody = await login.Content.ReadFromJsonAsync<JsonElement>();
        userClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loginBody.GetProperty("data").GetProperty("accessToken").GetString());

        var denied = await userClient.PostAsJsonAsync(
            "/v1/assets",
            new CreateAssetCommand("Denied Pump", "DENIED-1", "B1", AssetCriticality.High));
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

        var allowed = await admin.Client.PostAsJsonAsync(
            "/v1/assets",
            new CreateAssetCommand("Allowed Pump", "ALLOWED-1", "B1", AssetCriticality.High));
        Assert.Equal(HttpStatusCode.Created, allowed.StatusCode);
    }

    [Fact]
    public async Task TenantIsolation_HidesAndProtectsAnotherTenantsAsset()
    {
        await using var admin = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await admin.Client.PostAsJsonAsync(
            "/v1/assets",
            new CreateAssetCommand("Tenant A Pump", "TENANT-A-1", "B1", AssetCriticality.High));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var assetId = body.GetProperty("data").GetProperty("id").GetGuid();

        var tenantBEmail = $"tenant-b-{Guid.NewGuid():N}@example.com";
        await admin.SeedUserAsync(TenantB, tenantBEmail, "SecurePass123!", "Tenant B User");

        using var tenantB = admin.CreateClient(TenantB);
        var login = await tenantB.PostAsJsonAsync(
            "/v1/auth/login",
            new { Email = tenantBEmail, Password = "SecurePass123!" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var loginBody = await login.Content.ReadFromJsonAsync<JsonElement>();
        tenantB.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loginBody.GetProperty("data").GetProperty("accessToken").GetString());

        var get = await tenantB.GetAsync($"/v1/assets/{assetId}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);

        var update = await tenantB.PatchAsJsonAsync(
            $"/v1/assets/{assetId}",
            new UpdateAssetCommand(assetId, "Cross-tenant update", "B2", AssetCriticality.Critical));
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);

        var tenantAContext = CreateContext(TenantA);
        await using (tenantAContext)
        {
            Assert.NotNull(await tenantAContext.Assets.FirstOrDefaultAsync(a => a.Id == assetId));
        }

        var tenantBContext = CreateContext(TenantB);
        await using (tenantBContext)
        {
            Assert.Null(await tenantBContext.Assets.FirstOrDefaultAsync(a => a.Id == assetId));
        }
    }

    private AppDbContext CreateContext(Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_fx.ConnectionString)
            .Options;
        return new AppDbContext(options, new TestTenantProvider(tenantId));
    }

    private sealed class TestTenantProvider(Guid tenantId) : BingehOS.Infrastructure.ITenantProvider
    {
        public Guid CurrentTenantId { get; } = tenantId;
    }
}
