using System.Net;
using System.Net.Http.Json;
using Application.Features.User.Login;
using Application.Features.User.Register;

namespace Api.Tests;

[Collection("API integration")]
public class UserEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task RegisterUser()
    {
        using var client = factory.CreateClient();

        var request = new RegisterUserCommand(
            "testUser12",
            "test@example.com",
            "StrongPassword!1");

        var response = await client.PostAsJsonAsync(
            "/api/users/register", request);

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK, body);
    }

    [Fact]
    public async Task LoginUser()
    {
        using var client = factory.CreateClient();
        var request = new LoginUserCommand("string", "String!1");
        
        var response = await client.PostAsJsonAsync(
            "/api/users/login", request);
        
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK, body);
    }
}
