using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using backend.Models;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Users;

public class Users_GetAll_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Users_GetAll_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_Should_Return_List_Of_Users()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        await _client.PostAsJsonAsync("/api/users/register", new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        });

        await _client.PostAsJsonAsync("/api/users/register", new RegisterUserDTO
        {
            Name = "Bob",
            Email = "bob@example.com",
            Password = "Password123"
        });

        // Act
        var response = await _client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<User>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterOrEqualTo(2);

        users.Should().ContainSingle(u => u.Email == "alice@example.com");
        users.Should().ContainSingle(u => u.Email == "bob@example.com");
    }
}

