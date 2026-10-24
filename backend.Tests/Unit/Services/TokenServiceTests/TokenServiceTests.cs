using System.Text;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace backend.Tests.Unit.Services;

public class TokenServiceTests
{
    private const string Key = "unit-tests-signing-key-0123456789abcdef";
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly JwtSettings _settings = new()
    {
        Key = Key,
        Issuer = "test-issuer",
        Audience = "test-audience",
        AccessTokenMinutes = 15,
        RefreshTokenDays = 90
    };

    private readonly User _user = new() { Id = 42, Name = "Alice", Email = "alice@example.com" };

    private TokenService BuildService()
    {
        return new TokenService(Options.Create(_settings), new FixedTimeProvider(Now));
    }

    private Task<TokenValidationResult> ValidateAsync(string token, string key)
    {
        return new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            // The token was issued at a fixed time in the past, so expiry is asserted separately
            ValidateLifetime = false
        });
    }

    [Fact]
    public void CreateAccessToken_Should_Contain_User_Id_And_Email()
    {
        // Arrange
        var service = BuildService();

        // Act
        var token = new JsonWebToken(service.CreateAccessToken(_user));

        // Assert
        token.Subject.Should().Be("42");
        token.GetClaim(JwtRegisteredClaimNames.Email).Value.Should().Be("alice@example.com");
    }

    [Fact]
    public void CreateAccessToken_Should_Expire_After_Configured_Minutes()
    {
        // Arrange
        var service = BuildService();

        // Act
        var token = new JsonWebToken(service.CreateAccessToken(_user));

        // Assert
        token.ValidTo.Should().Be(Now.UtcDateTime.AddMinutes(15));
    }

    [Fact]
    public async Task CreateAccessToken_Should_Be_Valid_For_The_Signing_Key()
    {
        // Arrange
        var service = BuildService();

        // Act
        var result = await ValidateAsync(service.CreateAccessToken(_user), Key);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAccessToken_Should_Be_Rejected_When_Validated_With_Another_Key()
    {
        // Arrange
        var service = BuildService();

        // Act
        var result = await ValidateAsync(service.CreateAccessToken(_user), "a-different-signing-key-0123456789abcdef");

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateRefreshToken_Should_Store_Only_The_Hash_Of_The_Token()
    {
        // Arrange
        var service = BuildService();

        // Act
        var issued = service.CreateRefreshToken(42, Guid.NewGuid());

        // Assert
        issued.Token.Should().NotBeNullOrEmpty();
        issued.Entity.TokenHash.Should().NotBe(issued.Token);
        issued.Entity.TokenHash.Should().Be(service.HashRefreshToken(issued.Token));
    }

    [Fact]
    public void CreateRefreshToken_Should_Fill_In_Owner_Family_And_Expiry()
    {
        // Arrange
        var service = BuildService();
        var familyId = Guid.NewGuid();

        // Act
        var issued = service.CreateRefreshToken(42, familyId);

        // Assert
        issued.Entity.UserId.Should().Be(42);
        issued.Entity.FamilyId.Should().Be(familyId);
        issued.Entity.IsActive.Should().BeTrue();
        issued.Entity.CreatedAt.Should().Be(Now.UtcDateTime);
        issued.Entity.ExpiresAt.Should().Be(Now.UtcDateTime.AddDays(90));
    }

    [Fact]
    public void CreateRefreshToken_Should_Return_A_Different_Token_Each_Time()
    {
        // Arrange
        var service = BuildService();

        // Act
        var first = service.CreateRefreshToken(42, Guid.NewGuid());
        var second = service.CreateRefreshToken(42, Guid.NewGuid());

        // Assert
        first.Token.Should().NotBe(second.Token);
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
