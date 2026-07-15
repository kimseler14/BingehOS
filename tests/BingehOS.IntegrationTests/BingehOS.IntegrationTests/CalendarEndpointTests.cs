using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BingehOS.IntegrationTests;

public class CalendarEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;

    public CalendarEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Holidays_ReturnsBadRequest_ForInvalidYear()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var response = await auth.Client.GetAsync("/v1/calendar/holidays?year=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(body.GetProperty("success").GetBoolean());
        Assert.Contains("year", body.GetProperty("error").GetString(), StringComparison.OrdinalIgnoreCase);
    }
}
