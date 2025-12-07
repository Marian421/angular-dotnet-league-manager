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
        var service = new UserServiceBuilder()
          .WithFakeUserById(testId)
          .MapUsersToDtos()
          .Build();

        // Act
        var result = await service.GetUserByIdAsync(testId);
        var expectedUser = UserFactory.GetUserById(testId);

        // Assert
        if (expectedUser == null)
        {
            result.Should().BeNull();
        }
        else
        {
            var expectedDto = new GetUserDto
            {
                Id = expectedUser.Id,
                Name = expectedUser.Name,
                Email = expectedUser.Email
            };
            result.Should().BeEquivalentTo(expectedDto);
        }
    }
}

