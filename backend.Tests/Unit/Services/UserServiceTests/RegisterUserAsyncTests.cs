using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using backend.DTOs;
using backend.Tests.TestData.Fakes;

namespace backend.Tests.Unit.Services;

public class RegisterUserAsync
{
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

}




