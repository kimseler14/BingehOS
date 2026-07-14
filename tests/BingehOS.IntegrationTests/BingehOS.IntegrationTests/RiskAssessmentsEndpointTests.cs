using System.Net;
using System.Net.Http.Json;
using BingehOS.Modules.HSE.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BingehOS.IntegrationTests;

public class RiskAssessmentsEndpointTests : IClassFixture<TestContainerFixture>
{
    private readonly TestContainerFixture _fx;
    public RiskAssessmentsEndpointTests(TestContainerFixture fx) => _fx = fx;

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        await using var auth = await TestAuthHelper.GetAuthenticatedClientAsync(_fx);
        var client = auth.Client;

        var create = await client.PostAsJsonAsync("/v1/risk-assessments",
            new CreateRiskAssessmentCommand("Fire Risk", "Assess fire hazard", "High", Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
    }
}
