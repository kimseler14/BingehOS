using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Compliance.Application;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BingehOS.IntegrationTests;

public class CalibrationsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public CalibrationsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_Get_List_And_Update_ReturnsCalibrationRecord()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;
        var assetId = Guid.NewGuid();
        var calibratedAt = new DateTimeOffset(2026, 1, 15, 8, 30, 0, TimeSpan.Zero);

        var create = await client.PostAsJsonAsync(
            "/v1/calibrations",
            new CreateCalibrationRecordCommand(
                assetId,
                calibratedAt,
                calibratedAt.AddMonths(12),
                "Uygun"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var createBody = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = createBody.GetProperty("data").GetProperty("id").GetGuid();

        var get = await client.GetAsync($"/v1/calibrations/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var getBody = await get.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(assetId, getBody.GetProperty("data").GetProperty("assetId").GetGuid());

        var list = await client.GetAsync($"/v1/calibrations?assetId={assetId}");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        var listBody = await list.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(
            listBody.GetProperty("data").EnumerateArray(),
            item => item.GetProperty("id").GetGuid() == id);

        var update = await client.PatchAsJsonAsync(
            $"/v1/calibrations/{id}",
            new UpdateCalibrationRecordCommand(
                id,
                assetId,
                calibratedAt,
                calibratedAt.AddMonths(6),
                "Şartlı uygun"));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var updateBody = await update.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(
            "Şartlı uygun",
            updateBody.GetProperty("data").GetProperty("result").GetString());
    }

    [Fact]
    public async Task SeededTurkishJobPlans_AreIdempotent()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var names = new[]
        {
            "Asansör Aylık Bakım",
            "Yangın Söndürme Sistemleri Periyodik Bakımı",
            "Jeneratör Aylık Bakım",
            "HVAC Mevsimsel Bakım"
        };

        var firstResponse = await auth.Client.GetAsync(
            "/v1/job-plan-templates?skip=0&take=100");
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstBody = await firstResponse.Content.ReadFromJsonAsync<JsonElement>();
        var firstCount = firstBody.GetProperty("data")
            .EnumerateArray()
            .Count(item => names.Contains(item.GetProperty("name").GetString()));
        Assert.Equal(4, firstCount);

        var migrationOptions = new DbContextOptionsBuilder<BingehOS.Infrastructure.AppDbContext>()
            .UseNpgsql(_fx.AdminConnectionString)
            .Options;
        await using (var migrationDb = new BingehOS.Infrastructure.AppDbContext(
                         migrationOptions,
                         new FixedTenantProvider(Guid.Empty)))
        {
            await migrationDb.Database.MigrateAsync();
        }

        var secondResponse = await auth.Client.GetAsync(
            "/v1/job-plan-templates?skip=0&take=100");
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var secondBody = await secondResponse.Content.ReadFromJsonAsync<JsonElement>();
        var secondCount = secondBody.GetProperty("data")
            .EnumerateArray()
            .Count(item => names.Contains(item.GetProperty("name").GetString()));
        Assert.Equal(firstCount, secondCount);
    }

    private sealed class FixedTenantProvider(Guid tenantId)
        : BingehOS.Infrastructure.ITenantProvider
    {
        public Guid CurrentTenantId { get; } = tenantId;
    }
}
