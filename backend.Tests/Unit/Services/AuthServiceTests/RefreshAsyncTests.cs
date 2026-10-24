using backend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace backend.Tests.Unit.Services;

public class AuthServiceRefreshTests
{
    private const string Token = "client-refresh-token";
    private readonly User _user = new() { Id = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "hash1" };
    private readonly Guid _familyId = Guid.NewGuid();

    private RefreshToken StoredToken(AuthServiceBuilder builder, bool isActive = true, int expiresInDays = 30)
    {
        return new RefreshToken
        {
            Id = 10,
            UserId = _user.Id,
            FamilyId = _familyId,
            IsActive = isActive,
            ExpiresAt = builder.Now.UtcDateTime.AddDays(expiresInDays)
        };
    }

    [Fact]
    public async Task RefreshAsync_Should_Return_Null_When_Token_Is_Unknown()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        builder.RefreshTokens.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken?)null);
        var service = builder.Build();

        // Act
        var result = await service.RefreshAsync(Token);

        // Assert
        result.Should().BeNull();
        builder.RefreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_Should_Return_Null_When_Token_Is_Expired()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var stored = StoredToken(builder, expiresInDays: -1);
        builder.WithStoredRefreshToken(Token, stored).WithUserById(_user);
        var service = builder.Build();

        // Act
        var result = await service.RefreshAsync(Token);

        // Assert
        result.Should().BeNull();
        stored.IsActive.Should().BeTrue();
        builder.RefreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_Should_Cancel_The_Family_When_Token_Was_Already_Used()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var spent = StoredToken(builder, isActive: false);
        var newest = new RefreshToken { Id = 11, UserId = _user.Id, FamilyId = _familyId, IsActive = true };
        builder.WithStoredRefreshToken(Token, spent).WithUserById(_user);
        builder.RefreshTokens.Setup(r => r.GetActiveByFamilyAsync(_familyId)).ReturnsAsync(new List<RefreshToken> { newest });
        var service = builder.Build();

        // Act
        var result = await service.RefreshAsync(Token);

        // Assert
        result.Should().BeNull();
        newest.IsActive.Should().BeFalse();
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Once);
        builder.RefreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_Should_Rotate_The_Token_Within_The_Same_Family()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var stored = StoredToken(builder);
        builder.WithStoredRefreshToken(Token, stored).WithUserById(_user);
        var service = builder.Build();

        // Act
        var result = await service.RefreshAsync(Token);

        // Assert
        result!.AccessToken.Should().Be(AuthServiceBuilder.FakeToken);
        result.RefreshToken.Should().Be(AuthServiceBuilder.FakeRefreshToken);
        stored.IsActive.Should().BeFalse();

        builder.Token.Verify(t => t.CreateRefreshToken(_user.Id, _familyId), Times.Once);
        builder.RefreshTokens.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => t.FamilyId == _familyId)), Times.Once);
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
