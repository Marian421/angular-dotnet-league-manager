using Moq;
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

    public const string FakeToken = "fake-access-token";
    public const string FakeRefreshToken = "fake-refresh-token";

    public AuthServiceBuilder()
    {
        Token.Setup(t => t.CreateAccessToken(It.IsAny<User>())).Returns(FakeToken);
        Token.Setup(t => t.CreateRefreshToken(It.IsAny<int>(), It.IsAny<Guid>()))
             .Returns((int userId, Guid familyId) => new IssuedRefreshToken(
                 FakeRefreshToken,
                 new RefreshToken { UserId = userId, FamilyId = familyId, TokenHash = "fake-hash", IsActive = true }));
    }

    /// <summary>
    /// Builds the AuthService with all configured mocks.
    /// </summary>
    public AuthService Build()
    {
        return new AuthService(Repo.Object, Password.Object, Token.Object, RefreshTokens.Object);
    }
}
