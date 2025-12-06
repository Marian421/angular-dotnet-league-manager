using Moq;
using backend.Repositories;
using backend.Services;
using backend.Mappers;

namespace backend.Tests.Unit.Services;

public class UserServiceBuilder
{
    public Mock<IUserRepository> Repo { get; } = new();
    public Mock<IPasswordService> Password { get; } = new();
    public Mock<IUserMapper> Mapper { get; } = new();

    public UserService Build()
    {
        return new UserService(Repo.Object, Password.Object, Mapper.Object);
    }
}
