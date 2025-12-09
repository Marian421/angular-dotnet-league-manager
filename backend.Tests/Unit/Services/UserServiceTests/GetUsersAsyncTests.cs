using backend.Models;
using backend.Services;
using backend.Repositories;
using FluentAssertions;
using Moq;
using backend.Tests.TestData.Fakes;
using backend.Mappers;
using backend.DTOs;

namespace backend.Tests.Unit.Services;

public class GetUsersAsync
{
    [Fact]
    public async Task GetUsersAsync_Should_Return_All_Users()
    {
        // Arrange

        var service = new UserServiceBuilder()
          .WithFakeUsers()
          .MapUsersToDtos()
          .Build();

        var fakeUsers = UserFactory.GetFakeUsersDtos();

        // Act
        var result = await service.GetUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(fakeUsers);
    }

    [Fact]
    public async Task GetUsersAsync_Should_Return_Empty_List_When_No_Users()
    {
        // Arrange

        var service = new UserServiceBuilder()
          .SetupDefaultMocks()
          .MapUsersToDtos()
          .Build();

        // Act

        var result = await service.GetUsersAsync();

        // Assert
        result
          .Should()
          .BeEquivalentTo(Enumerable.Empty<GetUserDto>());
    }
}

