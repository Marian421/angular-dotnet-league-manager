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
    public async Task RegisterUserAsync_Should_Register_User_When_Email_Is_Available()
    {
        // Arrange
        var builder = new UserServiceBuilder();

        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "Secret123"
        };

        // mapper must return a new user INSTANCE
        var mappedUser = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = "user"
        };

        builder.Repo
          .Setup(r => r.GetByEmailAsync(dto.Email))
          .ReturnsAsync((User?)null);

        builder.Mapper
          .Setup(m => m.Map(dto))
          .Returns(mappedUser);

        // Simulate hash function:
        builder.Password
          .Setup(p => p.HashPassword(mappedUser, dto.Password))
          .Returns("hashed_pw");

        // Simulate EF assigning an ID when calling AddAsync
        builder.Repo
          .Setup(r => r.AddAsync(It.IsAny<User>()))
          .Callback<User>(u => u.Id = 99)     // EF-like behavior
          .Returns(Task.CompletedTask);

        builder.Repo
          .Setup(r => r.SaveChangesAsync())
          .Returns(Task.CompletedTask);

        var service = builder.Build();

        // Act
        var result = await service.RegisterUserAsync(dto);

        // Assert
        result.Should().Be(99);                 // returned id
        mappedUser.PasswordHash.Should().Be("hashed_pw");

        builder.Repo.Verify(r => r.AddAsync(mappedUser), Times.Once);
        builder.Repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }



    [Fact]
    public async Task RegisterUserAsync_Should_Throw_When_Email_Already_Exists()
    {
        // Arrange
        var builder = new UserServiceBuilder();

        var dto = new RegisterUserDTO
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "mypassword"
        };

        builder.Repo.Setup(r => r.GetByEmailAsync(dto.Email))
          .ReturnsAsync(new User { Email = dto.Email });

        var service = builder.Build();
        // Act
        var act = () => service.RegisterUserAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
          .WithMessage("There already exists an account with this email");

        // Ensure repo is NOT called for creation
        builder.Repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        builder.Repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}




