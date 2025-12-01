using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using backend.Models;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Users;

public class Users_CreateUser_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Users_CreateUser_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_Should_Register_User_Succesfully()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/users/register", dto);
        var user = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        user.Name.Should().BeEquivalentTo("Alice");
    }
}

