using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using backend.Repositories;
using backend.Services;
using backend.Models;

namespace backend.Tests.Unit.Services;

/// <summary>
/// Builder for creating an AuthService with pre-configured mocks for unit testing.
/// </summary>
public class AuthServiceBuilder
{
    public Mock<IUserRepository> Repo { get; } = new();
    public Mock<IPasswordService> Password { get; } = new();
    public Mock<ITokenService> Token { get; } = new();
    public Mock<IRefreshTokenRepository> RefreshTokens { get; } = new();

    public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    public const string FakeToken = "fake-access-token";
    public const string FakeRefreshToken = "fake-refresh-token";

    public AuthServiceBuilder()
    {
        Token.Setup(t => t.CreateAccessToken(It.IsAny<User>())).Returns(FakeToken);
        Token.Setup(t => t.CreateRefreshToken(It.IsAny<int>(), It.IsAny<Guid>()))
             .Returns((int userId, Guid familyId) => new IssuedRefreshToken(
                 FakeRefreshToken,
                 new RefreshToken { UserId = userId, FamilyId = familyId, TokenHash = "fake-hash", IsActive = true }));
        Token.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns((string token) => $"hash-of-{token}");
    }

    /// <summary>
    /// Configures the repository mock to find a stored refresh token by the hash of <paramref name="token"/>.
    /// </summary>
    public AuthServiceBuilder WithStoredRefreshToken(string token, RefreshToken stored)
    {
        stored.TokenHash = $"hash-of-{token}";
        RefreshTokens.Setup(r => r.GetByHashAsync(stored.TokenHash)).ReturnsAsync(stored);
        return this;
    }

    /// <summary>
    /// Configures the repository mock to return a specific user by ID.
    /// </summary>
    public AuthServiceBuilder WithUserById(User user)
    {
        Repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        return this;
    }

    /// <summary>
    /// Builds the AuthService with all configured mocks.
    /// </summary>
    public AuthService Build()
    {
        return new AuthService(
            Repo.Object,
            Password.Object,
            Token.Object,
            RefreshTokens.Object,
            new FixedTimeProvider(Now),
            NullLogger<AuthService>.Instance);
    }

    private class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now)
        {
            _now = now;
        }

        public override DateTimeOffset GetUtcNow() => _now;
    }
}
