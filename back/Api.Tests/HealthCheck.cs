using System.Net;

namespace Api.Tests;

[Collection("API integration")]
public class HealthEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task Health_returns_ok()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
