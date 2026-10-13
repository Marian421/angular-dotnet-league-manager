using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using backend.DTOs;
using backend.Mappers;

namespace backend.Tests.Unit.Services;

public class AuthServiceLoginTests
{
    private readonly List<User> fakeUsers = new()
    {
        new User { Id = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "hash1" },
        new User { Id = 2, Name = "Bob", Email = "bob@example.com", PasswordHash = "hash2" }
    };

    [Fact]
    public async Task LoginAsync_Should_Return_Tokens_When_Credentials_Are_Correct()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password)).Returns(true);

        var service = builder.Build();

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result!.AccessToken.Should().Be(AuthServiceBuilder.FakeToken);
        result.RefreshToken.Should().Be(AuthServiceBuilder.FakeRefreshToken);

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
        builder.Token.Verify(t => t.CreateAccessToken(user), Times.Once);
        builder.RefreshTokens.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => t.UserId == user.Id)), Times.Once);
        builder.RefreshTokens.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_User_Not_Found()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var dto = new LoginUserDTO { Email = "unknown@example.com", Password = "whatever" };

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var service = builder.Build();

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_Password_Is_Incorrect()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var dto = new LoginUserDTO { Email = "bob@example.com", Password = "wrong" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password)).Returns(false);

        var service = builder.Build();

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
        builder.Token.Verify(t => t.CreateAccessToken(It.IsAny<User>()), Times.Never);
        builder.RefreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Propagate_Exception_From_PasswordService()
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password))
                       .Throws(new InvalidOperationException("password service failed"));

        var service = builder.Build();

        // Act
        var act = () => service.LoginAsync(dto);

        // Assert: exception bubbles up
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("password service failed");

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Theory]
    [InlineData("alice@example.com", "password", true)]
    [InlineData("alice@example.com", "wrong", false)]
    public async Task LoginAsync_Theory_Calls_VerifyPassword_And_Returns_Appropriate_Result(string email, string password, bool expectedSuccess)
    {
        // Arrange
        var builder = new AuthServiceBuilder();
        var dto = new LoginUserDTO { Email = email, Password = password };
        var user = fakeUsers.First(u => u.Email == email);

        builder.Repo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, password)).Returns(expectedSuccess);

        var service = builder.Build();

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        if (expectedSuccess)
            result.Should().NotBeNull();
        else
            result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, password), Times.Once);
    }
}

