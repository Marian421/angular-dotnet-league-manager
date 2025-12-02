using backend.DTOs;
using backend.Mappers.Shared;
using backend.Models;

namespace backend.Mappers;

public interface IUserMapper : 
ICreateMapper<User, RegisterUserDTO>,
IGetMapper<User, GetUserDto>
{
}