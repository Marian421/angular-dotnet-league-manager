using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using backend.DTOs;

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
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

        var mockPassword = new Mock<IPasswordService>();
        mockPassword.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password))
                    .Returns(true);

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);

        mockRepo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        mockPassword.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Return_Null_When_User_Not_Found()
    {
        // Arrange
        var dto = new LoginUserDTO { Email = "unknown@example.com", Password = "whatever" };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

        var mockPassword = new Mock<IPasswordService>();

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();

        mockRepo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        // VerifyPassword should never be called because user is null
        mockPassword.Verify(p => p.VerifyPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Return_Null_When_Password_Is_Incorrect()
    {
        // Arrange
        var dto = new LoginUserDTO { Email = "bob@example.com", Password = "wrong" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

        var mockPassword = new Mock<IPasswordService>();
        mockPassword.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password))
                    .Returns(false);

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        result.Should().BeNull();

        mockRepo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        mockPassword.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_Should_Propagate_Exception_From_PasswordService()
    {
        // Arrange
        var dto = new LoginUserDTO { Email = "alice@example.com", Password = "password" };
        var user = fakeUsers.First(u => u.Email == dto.Email);

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

        var mockPassword = new Mock<IPasswordService>();
        mockPassword.Setup(p => p.VerifyPassword(user, user.PasswordHash, dto.Password))
                    .Throws(new System.InvalidOperationException("password service failed"));

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var act = () => service.LoginUserAsync(dto);

        // Assert: exception bubbles up
        await act.Should().ThrowAsync<System.InvalidOperationException>()
                 .WithMessage("password service failed");

        mockRepo.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once);
        mockPassword.Verify(p => p.VerifyPassword(user, user.PasswordHash, dto.Password), Times.Once);
    }

    // Optional: parameterized test for a couple of variations (happy + wrong password)
    [Theory]
    [InlineData("alice@example.com", "password", true)]
    [InlineData("alice@example.com", "wrong", false)]
    public async Task LoginUserAsync_Theory_Calls_VerifyPassword_And_Returns_Appropriate_Result(string email, string password, bool expectedSuccess)
    {
        // Arrange
        var dto = new LoginUserDTO { Email = email, Password = password };
        var user = fakeUsers.First(u => u.Email == email);

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        var mockPassword = new Mock<IPasswordService>();
        mockPassword.Setup(p => p.VerifyPassword(user, user.PasswordHash, password)).Returns(expectedSuccess);

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var result = await service.LoginUserAsync(dto);

        // Assert
        if (expectedSuccess)
            result.Should().NotBeNull().And.BeEquivalentTo(user);
        else
            result.Should().BeNull();

        mockRepo.Verify(r => r.GetByEmailAsync(email), Times.Once);
        mockPassword.Verify(p => p.VerifyPassword(user, user.PasswordHash, password), Times.Once);
    }
}
