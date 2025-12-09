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

public class UserServiceLoginTests
{
    private readonly List<User> fakeUsers = new()
    {
        new User { Id = 1, Name = "Alice", Email = "alice@example.com", PasswordHash = "hash1" },
        new User { Id = 2, Name = "Bob", Email = "bob@example.com", PasswordHash = "hash2" }
    };

    [Fact]
    public async Task LoginUserAsync_Should_Return_User_When_Credentials_Are_Correct()
    {
        // Arrange
        var builder = new UserServiceBuilder();
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password)).Returns(true);

        var service = builder.Build();

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().NotBeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Return_Null_When_User_Not_Found()
    {
        // Arrange
        var builder = new UserServiceBuilder();
        var dto = new LoginUserDTO { Email = "unknown@example.com", Password = "whatever" };

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        var service = builder.Build();

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Return_Null_When_Password_Is_Incorrect()
    {
        // Arrange
        var builder = new UserServiceBuilder();
        var dto = new LoginUserDTO { Email = "bob@example.com", Password = "wrong" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password)).Returns(false);

        var service = builder.Build();

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Propagate_Exception_From_PasswordService()
    {
        // Arrange
        var builder = new UserServiceBuilder();
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password))
                       .Throws(new InvalidOperationException("password service failed"));

        var service = builder.Build();

        // Act
        var act = () => service.LoginUserAsync(dto);

        // Assert: exception bubbles up
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("password service failed");

        builder.Repo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Theory]
    [InlineData("alice@example.com", "password", true)]
    [InlineData("alice@example.com", "wrong", false)]
    public async Task LoginUserAsync_Theory_Calls_VerifyPassword_And_Returns_Appropriate_Result(string email, string password, bool expectedSuccess)
    {
        // Arrange
        var builder = new UserServiceBuilder();
        var dto = new LoginUserDTO { Email = email, Password = password };
        var user = fakeUsers.First(u => u.Email == email);

        builder.Repo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
        builder.Password.Setup(p => p.VerifyPassword(user, user.PasswordHash, password)).Returns(expectedSuccess);

        var service = builder.Build();

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        if (expectedSuccess)
            result.Should().NotBeNull();
        else
            result.Should().BeNull();

        builder.Repo.Verify(r => r.GetByEmailAsync(email), Times.Once);
        builder.Password.Verify(p => p.VerifyPassword(user, user.PasswordHash, password), Times.Once);
    }
}

