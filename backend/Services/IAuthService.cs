using backend.DTOs;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginUserDTO dto);
        Task<LoginResponseDto?> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task LogoutAllAsync(string refreshToken);
    }
}
