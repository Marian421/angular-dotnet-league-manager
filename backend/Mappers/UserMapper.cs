using backend.DTOs;
using backend.Models;

namespace backend.Mappers;

public class UserMapper : IUserMapper
{
    public User Map(RegisterUserDTO source)
    {
        return new()
        {
            Name = source.Name,
            Email = source.Email,
            Role = "user",
        };
    }

    public GetUserDto Map(User source)
    {
        return new()
        {
            Id = source.Id,
            Name = source.Name,
        };
    }
}