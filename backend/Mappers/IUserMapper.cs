using backend.DTOs;
using backend.Mappers.Shared;
using backend.Models;

namespace backend.Mappers;

public interface IUserMapper : 
IMapper<RegisterUserDTO, User>,
IMapper<User, GetUserDto>
{
}