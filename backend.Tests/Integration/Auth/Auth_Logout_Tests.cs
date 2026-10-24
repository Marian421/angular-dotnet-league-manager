using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Auth;

public class Auth_Logout_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Auth_Logout_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task RegisterAsync()
    {
        var register = await _client.PostAsJsonAsync("api/users/register", new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        });
        register.EnsureSuccessStatusCode();
    }

    private async Task<LoginResponseDto> LoginAsync()
    {
        var login = await _client.PostAsJsonAsync("api/auth/login", new LoginUserDTO
        {
            Email = "alice@example.com",
            Password = "Password123"
        });
        login.EnsureSuccessStatusCode();

        return (await login.Content.ReadFromJsonAsync<LoginResponseDto>())!;
    }

    private Task<HttpResponseMessage> PostTokenAsync(string url, string refreshToken)
    {
        return _client.PostAsJsonAsync(url, new RefreshTokenDto { RefreshToken = refreshToken });
    }

    [Fact]
    public async Task Logout_Should_End_Only_The_Login_It_Was_Called_For()
    {
        // Arrange: two logins, e.g. laptop and phone
        await _factory.ResetDatabaseAsync();
        await RegisterAsync();
        var laptop = await LoginAsync();
        var phone = await LoginAsync();

        // Act
        var logout = await PostTokenAsync("api/auth/logout", laptop.RefreshToken);

        // Assert
        logout.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await PostTokenAsync("api/auth/refresh", laptop.RefreshToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await PostTokenAsync("api/auth/refresh", phone.RefreshToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_Should_Return_NoContent_For_Unknown_Token()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        // Act
        var logout = await PostTokenAsync("api/auth/logout", "not-a-token-we-issued");

        // Assert
        logout.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task LogoutAll_Should_End_Every_Login_Of_The_User()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        await RegisterAsync();
        var laptop = await LoginAsync();
        var phone = await LoginAsync();

        // Act
        var logout = await PostTokenAsync("api/auth/logout-all", laptop.RefreshToken);

        // Assert
        logout.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await PostTokenAsync("api/auth/refresh", laptop.RefreshToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await PostTokenAsync("api/auth/refresh", phone.RefreshToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LogoutAll_Should_Do_Nothing_When_Called_With_A_Used_Token()
    {
        // Arrange: the laptop token is rotated, so the original is spent
        await _factory.ResetDatabaseAsync();
        await RegisterAsync();
        var laptop = await LoginAsync();
        var phone = await LoginAsync();
        (await PostTokenAsync("api/auth/refresh", laptop.RefreshToken)).EnsureSuccessStatusCode();

        // Act
        var logout = await PostTokenAsync("api/auth/logout-all", laptop.RefreshToken);

        // Assert
        logout.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await PostTokenAsync("api/auth/refresh", phone.RefreshToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
