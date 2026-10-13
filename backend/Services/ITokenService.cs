using backend.Models;

namespace backend.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        IssuedRefreshToken CreateRefreshToken(int userId, Guid familyId);
        string HashRefreshToken(string token);
    }

    // Token goes to the client; Entity (holding only the hash) goes to the database
    public record IssuedRefreshToken(string Token, RefreshToken Entity);
}
