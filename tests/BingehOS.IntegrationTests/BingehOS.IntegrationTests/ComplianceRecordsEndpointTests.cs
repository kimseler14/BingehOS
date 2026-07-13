using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Compliance.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class ComplianceRecordsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public ComplianceRecordsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/compliance-records",
            new CreateComplianceRecordCommand("ISO Audit", "Annual ISO 55001 audit", "Pending", DateTime.UtcNow.AddDays(30)));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/compliance-records/{id}",
            new UpdateComplianceRecordCommand(id, "ISO Audit Updated", "Annual ISO 55001 audit", "Completed", DateTime.UtcNow.AddDays(30)));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}
