using backend.Models;
using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository repo, IPasswordService passwordService)
        {
            _repo = repo;
            _passwordService = passwordService;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<User> RegisterUserAsync(RegisterUserDTO dto)
        {
            if (await _repo.GetByEmailAsync(dto.Email) != null)
            {
                throw new InvalidOperationException("There already exists an account with this email");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = "user",
            };

            user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return user;
        }

        public async Task<User?> LoginUserAsync(LoginUserDTO dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            var ok = _passwordService.VerifyPassword(user, user.PasswordHash, dto.Password);
            return ok ? user : null;
        }
    }
}

