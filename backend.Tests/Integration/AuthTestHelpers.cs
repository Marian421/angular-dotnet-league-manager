using System.Net.Http.Headers;
using System.Net.Http.Json;
using backend.DTOs;

namespace backend.Tests.Integration;

public static class AuthTestHelpers
{
    // Logs in an already registered user and sends their access token with every later request
    public static async Task LoginAsync(this HttpClient client, string email, string password = "Password123")
    {
        var login = await client.PostAsJsonAsync("api/auth/login", new LoginUserDTO
        {
            Email = email,
            Password = password
        });
        login.EnsureSuccessStatusCode();
        var body = await login.Content.ReadFromJsonAsync<LoginResponseDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);
    }
}
