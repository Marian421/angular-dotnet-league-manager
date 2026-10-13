using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.DTOs;
using backend.DTOs.Teams;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Auth;

public class Auth_Refresh_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Auth_Refresh_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<LoginResponseDto> RegisterAndLoginAsync()
    {
        var register = await _client.PostAsJsonAsync("api/users/register", new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        });
        register.EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("api/auth/login", new LoginUserDTO
        {
            Email = "alice@example.com",
            Password = "Password123"
        });
        login.EnsureSuccessStatusCode();

        return (await login.Content.ReadFromJsonAsync<LoginResponseDto>())!;
    }

    private Task<HttpResponseMessage> RefreshAsync(string refreshToken)
    {
        return _client.PostAsJsonAsync("api/auth/refresh", new RefreshTokenDto { RefreshToken = refreshToken });
    }

    [Fact]
    public async Task Refresh_Should_Return_A_New_Pair_Of_Working_Tokens()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var login = await RegisterAndLoginAsync();

        // Act
        var response = await RefreshAsync(login.RefreshToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshed = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        refreshed!.RefreshToken.Should().NotBe(login.RefreshToken);

        // The new access token is accepted by a protected endpoint
        var createTeam = new HttpRequestMessage(HttpMethod.Post, "api/teams")
        {
            Content = JsonContent.Create(new TeamCreateDto { Name = "Lions", IsCompetitive = true, MinAge = 18 })
        };
        createTeam.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
        var created = await _client.SendAsync(createTeam);
        created.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Refresh_Should_Return_Unauthorized_For_Unknown_Token()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        // Act
        var response = await RefreshAsync("not-a-token-we-issued");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await response.Content.ReadAsStringAsync()).Should().NotContain("token");
    }

    [Fact]
    public async Task Refresh_Should_Cancel_The_Whole_Login_When_A_Used_Token_Is_Replayed()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var login = await RegisterAndLoginAsync();
        var first = await RefreshAsync(login.RefreshToken);
        var newest = await first.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Act: the original token is presented a second time
        var replay = await RefreshAsync(login.RefreshToken);

        // Assert: the replay is refused, and so is the token that replaced it
        replay.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var afterReplay = await RefreshAsync(newest!.RefreshToken);
        afterReplay.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_Should_Not_Affect_Another_Login_Of_The_Same_User()
    {
        // Arrange: two logins, e.g. laptop and phone
        await _factory.ResetDatabaseAsync();
        var laptop = await RegisterAndLoginAsync();
        var phoneLogin = await _client.PostAsJsonAsync("api/auth/login", new LoginUserDTO
        {
            Email = "alice@example.com",
            Password = "Password123"
        });
        var phone = await phoneLogin.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Act: the laptop token is used, then replayed
        await RefreshAsync(laptop.RefreshToken);
        await RefreshAsync(laptop.RefreshToken);

        // Assert: the phone can still refresh
        var phoneRefresh = await RefreshAsync(phone!.RefreshToken);
        phoneRefresh.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
