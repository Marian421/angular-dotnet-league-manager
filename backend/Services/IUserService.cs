using backend.DTOs;
using backend.Models;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> RegisterUserAsync(RegisterUserDTO dto);
    Task<User?> LoginUserAsync(LoginUserDTO dto);
}
