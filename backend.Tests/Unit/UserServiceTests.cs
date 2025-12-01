using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using Xunit;
using backend.DTOs;

namespace backend.Tests.Unit;

public class UserServiceTests
{
    private readonly List<User> fakeUsers = new()
    {
        new User { Id = 1, Name = "Alice", Email = "alice@example.com" },
        new User { Id = 2, Name = "Bob", Email = "bob@example.com" }
    };






    [Fact]
    public async Task RegisterUserAsync_Should_Register_User_When_Email_Not_Exists()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var mockPassword = new Mock<IPasswordService>();

        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "mypassword"
        };

        // Email does not exist
        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
          .ReturnsAsync((User?)null);

        // Password hashing
        mockPassword.Setup(p => p.HashPassword(It.IsAny<User>(), dto.Password))
          .Returns("hashed_pw");

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var result = await service.RegisterUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Email.Should().Be(dto.Email);
        result.PasswordHash.Should().Be("hashed_pw");

        mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task RegisterUserAsync_Should_Throw_When_Email_Already_Exists()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var mockPassword = new Mock<IPasswordService>();

        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "mypassword"
        };

        mockRepo.Setup(r => r.GetByEmailAsync(dto.Email))
          .ReturnsAsync(new User { Email = dto.Email });

        var service = new UserService(mockRepo.Object, mockPassword.Object);

        // Act
        var act = () => service.RegisterUserAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
          .WithMessage("There already exists an account with this email");

        // Ensure repo is NOT called for creation
        mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public void HashPassword_Should_Return_Different_Value_Than_Input()
    {
        // Arrange
        var service = new PasswordService();
        var user = new User { Email = "test@test.com" };

        // Act
        var result = service.HashPassword(user, "mypassword");

        // Assert
        result.Should().NotBe("mypassword");
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Valid_Password()
    {
        var service = new PasswordService();
        var user = new User { Email = "test@test.com" };

        var hash = service.HashPassword(user, "mypassword");

        service.VerifyPassword(user, hash, "mypassword")
               .Should().BeTrue();
    }
}
