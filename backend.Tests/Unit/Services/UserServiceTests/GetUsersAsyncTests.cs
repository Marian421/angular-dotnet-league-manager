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

public class GetUsersAsync
{
    [Fact]
    public async Task GetUsersAsync_Should_Return_All_Users()
    {
        // Arrange
        List<User> fakeUsers = UserFactory.GetFakeUsers();
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(fakeUsers);

        var service = new UserService(mockRepo.Object, null!);

        // Act
        var result = await service.GetUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(fakeUsers);
    }

    [Fact]
    public async Task GetUsersAsync_Should_Return_Empty_List_When_No_Users()
    {
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>());

        var service = new UserService(mockRepo.Object, null!);

        var result = await service.GetUsersAsync();

        result.Should().BeEmpty();
    }
}

