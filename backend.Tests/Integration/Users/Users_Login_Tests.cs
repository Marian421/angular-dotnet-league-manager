using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using backend.Models;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Users;

public class Users_Login_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Users_Login_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Be_Succesfull_If_Credentials_Are_Correct()
    {
        // Arrange
        // 1. Create user
        await _factory.ResetDatabaseAsync();
        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        };
        var createUser = await _client.PostAsJsonAsync("api/users/register", dto);
        createUser.EnsureSuccessStatusCode();
        // 2. Make login DTO
        var loginDto = new LoginUserDTO
        {
            Email = "alice@example.com",
            Password = "Password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/users/login", loginDto);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Login_Should_Fail_If_Invalid_Credentials()
    {
        // Arrange
        // 1. Create user
        await _factory.ResetDatabaseAsync();
        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        };
        var createUser = await _client.PostAsJsonAsync("api/users/register", dto);
        createUser.EnsureSuccessStatusCode();
        // 2. Make login DTO
        var loginDto = new LoginUserDTO
        {
            Email = "alice@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"api/users/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

