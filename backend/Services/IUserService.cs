using backend.DTOs;
using backend.Models;

public interface IUserService
{
    Task<IEnumerable<GetUserDto>> GetUsersAsync();
    Task<GetUserDto?> GetUserByIdAsync(int id);
    Task<int> RegisterUserAsync(RegisterUserDTO dto);
    Task<int?> LoginUserAsync(LoginUserDTO dto);
}
