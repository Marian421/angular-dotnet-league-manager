using backend.Models;
using backend.DTOs;
using backend.Repositories;
using backend.Mappers;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _passwordService;
        private readonly IUserMapper _userMapper;

        public UserService(IUserRepository repo, IPasswordService passwordService, IUserMapper userMapper)
        {
            _repo = repo;
            _passwordService = passwordService;
            _userMapper = userMapper;
        }

        public async Task<IEnumerable<GetUserDto>> GetUsersAsync()
        {
            var models = await _repo.GetAllAsync();
            return models.Select(_userMapper.Map);
        }

        public async Task<GetUserDto?> GetUserByIdAsync(int id)
        {
            var model = await _repo.GetByIdAsync(id);
            return model != null ? _userMapper.Map(model) : null;
        }

        public async Task<int> RegisterUserAsync(RegisterUserDTO dto)
        {
            // ar trebui mutata validarea intr-o interfata ca mapper
            if (await _repo.GetByEmailAsync(dto.Email) != null)
            {
                throw new InvalidOperationException("There already exists an account with this email");
            }

            var user = _userMapper.Map(dto);

            user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return user.Id;
        }

        public async Task<int?> LoginUserAsync(LoginUserDTO dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var ok = _passwordService.VerifyPassword(user, user.PasswordHash, dto.Password);
            return ok ? user.Id : null;
        }
    }
}

