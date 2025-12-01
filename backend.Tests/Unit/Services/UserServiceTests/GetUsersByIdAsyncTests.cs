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

public class GetUsersByIdAsync
{
    [Theory]
    [InlineData(1)] // existing user
    [InlineData(2)] // existing user
    [InlineData(3)] // non-existing user
    public async Task GetUsersByIdAsync_Should_Return_Correct_Result(int testId)
    {
        // Arrange
        List<User> fakeUsers = UserFactory.GetFakeUsers();
        var mockRepo = new Mock<IUserRepository>();
        var expectedUser = UserFactory.GetUserById(testId);
        mockRepo
            .Setup(r => r.GetByIdAsync(testId))
            .ReturnsAsync(expectedUser);

        var service = new UserService(mockRepo.Object, null!);

        // Act
        var result = await service.GetUserByIdAsync(testId);

        // Assert
        if (expectedUser is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}

