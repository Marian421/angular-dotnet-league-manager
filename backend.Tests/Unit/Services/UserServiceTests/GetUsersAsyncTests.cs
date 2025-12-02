using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using backend.Tests.TestData.Fakes;
using backend.Mappers;

namespace backend.Tests.Unit.Services;

public class GetUsersAsync
{
    // xunit fiecare test creaza clasa de testare, aici e GetUsersAsync
    // e mai bine sa pui asa ca sa nu te repeti si e mai usor de actualizat dependentele.
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordService> _passwordServiceMock = new();
    private readonly Mock<IUserMapper> _userMapperMock = new();

    [Fact]
    public async Task GetUsersAsync_Should_Return_All_Users()
    {
        // Arrange
        List<User> fakeUsers = UserFactory.GetFakeUsers();
        _userRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(fakeUsers);

        var service = CreateUserService();

        // Act
        var result = await service.GetUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(fakeUsers);
    }

    [Fact]
    public async Task GetUsersAsync_Should_Return_Empty_List_When_No_Users()
    {
        _userRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>());

        var service = CreateUserService();

        var result = await service.GetUsersAsync();

        result.Should().BeEmpty();
    }

    private UserService CreateUserService()
    {
        return new UserService(
            _userRepositoryMock.Object, 
            _passwordServiceMock.Object,
            _userMapperMock.Object);
    }
}

