using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Personnel.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class SgkRecordsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public SgkRecordsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/sgk-records",
            new CreateSgkRecordCommand(Guid.NewGuid(), "SGK-123", "PRF-001", "NACE-01", DateTime.UtcNow));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, id);
    }
}
