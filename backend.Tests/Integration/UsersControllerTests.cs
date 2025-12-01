using System.Net;
using System.Net.Http.Json;
using backend.DTOs;
using backend.Models;
using FluentAssertions;
using Xunit;

namespace backend.Tests.Integration;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Create_User()
    {
        var dto = new RegisterUserDTO
        {
            Name = "John",
            Email = "john@example.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/users/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser = await response.Content.ReadFromJsonAsync<User>();

        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_For_Invalid_Credentials()
    {
        var dto = new LoginUserDTO
        {
            Email = "unknown@example.com",
            Password = "wrongpass"
        };

        var response = await _client.PostAsJsonAsync("/api/users/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
