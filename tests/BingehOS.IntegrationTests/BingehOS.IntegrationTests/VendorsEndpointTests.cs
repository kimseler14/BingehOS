using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BingehOS.Modules.Vendor.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class VendorsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public VendorsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_And_Update_ReturnsOk()
    {
        using var client = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);

        var create = await client.PostAsJsonAsync("/v1/vendors",
            new CreateVendorCommand("Acme Ltd", "1234567890", "info@acme.test", "+90 555 000 00 00", true));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var body = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("data").GetProperty("id").GetGuid();

        var patch = await client.PatchAsJsonAsync($"/v1/vendors/{id}",
            new UpdateVendorCommand(id, "Acme Renamed", "info@acme.test", "+90 555 000 00 00", false));
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
    }
}
