using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.DTOs;
using backend.DTOs.Teams;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Teams;

public class Teams_CreateTeam_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Teams_CreateTeam_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static TeamCreateDto NewTeam()
    {
        return new TeamCreateDto { Name = "Lions", IsCompetitive = true, MinAge = 18 };
    }

    // Registers a user and logs in, returning the user's id and access token
    private async Task<(int Id, string Token)> RegisterAndLoginAsync(string name, string email)
    {
        var register = await _client.PostAsJsonAsync("api/users/register", new RegisterUserDTO
        {
            Name = name,
            Email = email,
            Password = "Password123"
        });
        register.EnsureSuccessStatusCode();
        var user = await register.Content.ReadFromJsonAsync<GetUserDto>();

        var login = await _client.PostAsJsonAsync("api/users/login", new LoginUserDTO
        {
            Email = email,
            Password = "Password123"
        });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<LoginResponseDto>();

        return (user!.Id, body!.AccessToken);
    }

    private static HttpRequestMessage CreateTeamRequest(string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/teams")
        {
            Content = JsonContent.Create(NewTeam())
        };
        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    [Fact]
    public async Task CreateTeam_Should_Return_Unauthorized_Without_Token()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        // Act
        var response = await _client.SendAsync(CreateTeamRequest(null));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTeam_Should_Return_Unauthorized_With_Tampered_Token()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var (_, token) = await RegisterAndLoginAsync("Alice", "alice@example.com");
        var tampered = token[..^3] + (token.EndsWith("AAA") ? "BBB" : "AAA");

        // Act
        var response = await _client.SendAsync(CreateTeamRequest(tampered));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTeam_Should_Set_Owner_To_The_Caller()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        await RegisterAndLoginAsync("Alice", "alice@example.com");
        var (bobId, bobToken) = await RegisterAndLoginAsync("Bob", "bob@example.com");

        // Act
        var response = await _client.SendAsync(CreateTeamRequest(bobToken));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var team = await response.Content.ReadFromJsonAsync<TeamDetailsDto>();
        team!.Owner.Id.Should().Be(bobId);
        team.Owner.Name.Should().Be("Bob");
    }
}
