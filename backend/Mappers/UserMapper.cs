using backend.DTOs;
using backend.Models;

namespace backend.Mappers;

public class UserMapper : IUserMapper
{
    public User Map(RegisterUserDTO dto)
    {
        return new()
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = "user",
        };
    }

    public GetUserDto Map(User model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
        };
    }
}