using Moq;
using backend.Repositories;
using backend.Services;
using backend.Mappers;
using backend.Models;
using backend.DTOs;
using backend.Tests.TestData.Fakes;

namespace backend.Tests.Unit.Services;


/// <summary>
/// Builder for creating a UserService with pre-configured mocks for unit testing.
/// Supports using fake users from UserFactory and fluent configuration of dependencies.
/// </summary>
public class UserServiceBuilder
{
    public Mock<IUserRepository> Repo { get; } = new();
    public Mock<IPasswordService> Password { get; } = new();
    public Mock<IUserMapper> Mapper { get; } = new();

    /// <summary>
    /// Configures the repository mock to return a specific list of users.
    /// </summary>
    public UserServiceBuilder WithUsers(List<User> users)
    {
        Repo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        return this;
    }

    /// <summary>
    /// Configures the repository mock to return a specific user by ID.
    /// </summary>
    public UserServiceBuilder WithUserById(User user)
    {
        Repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        return this;
    }

    /// <summary>
    /// Configures the repository mock to return a specific user by email.
    /// </summary>
    public UserServiceBuilder WithUserByEmail(User user)
    {
        Repo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        return this;
    }

    /// <summary>
    /// Configures the password service mock to return a verification result.
    /// </summary>
    public UserServiceBuilder WithPasswordCheck(bool result)
    {
        Password.Setup(p => p.VerifyPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns(result);
        return this;
    }

    /// <summary>
    /// Configures the mapper mock to map any user to a GetUserDto.
    /// </summary>
    public UserServiceBuilder MapUsersToDtos()
    {
        Mapper.Setup(m => m.Map(It.IsAny<User>()))
              .Returns((User u) => new GetUserDto { Id = u.Id, Name = u.Name });
        return this;
    }

    /// <summary>
    /// Sets up default mocks for all dependencies.
    /// </summary>
    public UserServiceBuilder SetupDefaultMocks()
    {
        Repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        Password.Setup(p => p.VerifyPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        Mapper.Setup(m => m.Map(It.IsAny<User>()))
              .Returns((User u) => new GetUserDto { Id = u.Id, Name = u.Name });
        return this;
    }

    /// <summary>
    /// Configures the repository to return all fake users from UserFactory.
    /// </summary>
    public UserServiceBuilder WithFakeUsers()
    {
        return WithUsers(UserFactory.GetFakeUsers());
    }

    /// <summary>
    /// Configures the repository to return a fake user by ID from UserFactory.
    /// </summary>
    public UserServiceBuilder WithFakeUserById(int id)
    {
        var user = UserFactory.GetUserById(id);
        if (user != null) WithUserById(user);
        return this;
    }

    /// <summary>
    /// Configures the repository to return a fake user by email from UserFactory.
    /// </summary>
    public UserServiceBuilder WithFakeUserByEmail(string email)
    {
        var user = UserFactory.GetUserByEmail(email);
        WithUserByEmail(user);
        return this;
    }

    /// <summary>
    /// Builds the UserService with all configured mocks.
    /// </summary>
    public UserService Build()
    {
        return new UserService(Repo.Object, Password.Object, Mapper.Object);
    }
}

