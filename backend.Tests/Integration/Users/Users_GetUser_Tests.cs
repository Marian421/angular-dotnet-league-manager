using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using backend.Models;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration.Users;

public class Users_GetUser_Tests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public Users_GetUser_Tests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_Should_Return_User_If_Id_Is_Correct()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("api/users/register", dto);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>();
        int id = user!.Id;

        // Act
        var getResponse = await _client.GetAsync($"api/users/{id}");
        getResponse.EnsureSuccessStatusCode();
        var getUser = await getResponse.Content.ReadFromJsonAsync<User>();

        getUser.Should().NotBeNull();
        getUser!.Id.Should().Be(id);
    }
}

