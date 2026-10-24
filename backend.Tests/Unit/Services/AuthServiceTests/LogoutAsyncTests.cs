using backend.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace backend.Tests.Unit.Services;

public class AuthServiceLogoutTests
{
    private const string Token = "client-refresh-token";
    private const int UserId = 1;
    private readonly Guid _familyId = Guid.NewGuid();

    private RefreshToken StoredToken(AuthServiceBuilder builder, bool isActive = true, int expiresInDays = 30)
    {
        return new RefreshToken
        {
            Id = 10,
            UserId = UserId,
            FamilyId = _familyId,
            IsActive = isActive,
            ExpiresAt = builder.Now.UtcDateTime.AddDays(expiresInDays)
        };
    }

    [Fact]
    public async Task LogoutAsync_Should_Deactivate_The_Tokens_Family()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var stored = StoredToken(builder);
        builder.WithStoredRefreshToken(Token, stored);
        builder.RefreshTokens.Setup(r => r.GetActiveByFamilyAsync(_familyId)).ReturnsAsync(new List<RefreshToken> { stored });
        var service = builder.Build();

        // Act
        await service.LogoutAsync(Token);

        // Assert
        stored.IsActive.Should().BeFalse();
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Once);
        builder.RefreshTokens.Verify(r => r.GetActiveByUserAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_Should_Do_Nothing_When_Token_Is_Unknown()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        builder.RefreshTokens.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken?)null);
        var service = builder.Build();

        // Act
        await service.LogoutAsync(Token);

        // Assert
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task LogoutAllAsync_Should_Deactivate_Every_Active_Token_Of_The_User()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var stored = StoredToken(builder);
        var otherDevice = new RefreshToken { Id = 11, UserId = UserId, FamilyId = Guid.NewGuid(), IsActive = true };
        builder.WithStoredRefreshToken(Token, stored);
        builder.RefreshTokens.Setup(r => r.GetActiveByUserAsync(UserId)).ReturnsAsync(new List<RefreshToken> { stored, otherDevice });
        var service = builder.Build();

        // Act
        await service.LogoutAllAsync(Token);

        // Assert
        stored.IsActive.Should().BeFalse();
        otherDevice.IsActive.Should().BeFalse();
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(false, 30)] // already used
    [InlineData(true, -1)]  // expired
    public async Task LogoutAllAsync_Should_Do_Nothing_When_Token_Is_Not_Current(bool isActive, int expiresInDays)
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        builder.WithStoredRefreshToken(Token, StoredToken(builder, isActive, expiresInDays));
        var service = builder.Build();

        // Act
        await service.LogoutAllAsync(Token);

        // Assert
        builder.RefreshTokens.Verify(r => r.GetActiveByUserAsync(It.IsAny<int>()), Times.Never);
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
