using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Authorization;

public class Endpoints_RequireLogin_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Endpoints_RequireLogin_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("GET", "api/users")]
    [InlineData("GET", "api/users/1")]
    [InlineData("GET", "api/teams")]
    [InlineData("GET", "api/teams/1")]
    [InlineData("POST", "api/teams")]
    [InlineData("GET", "api/matches")]
    [InlineData("GET", "api/matches/1")]
    [InlineData("POST", "api/matches")]
    public async Task Endpoint_Should_Return_Unauthorized_Without_Token(string method, string url)
    {
        // Arrange
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (method == "POST")
        {
            request.Content = JsonContent.Create(new { });
        }

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_Should_Work_Without_Token()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        // Act
        var response = await _client.PostAsJsonAsync("api/users/register", new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
