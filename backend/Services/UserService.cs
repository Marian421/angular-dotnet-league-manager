using backend.Models;
using backend.DTOs;
using backend.Repositories;
using backend.Mappers;

namespace backend.Services
{
    /// <summary>
    /// Provides user-related operations such as registration, login, and retrieval.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _passwordService;
        private readonly IUserMapper _userMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="repo">Repository used to perform CRUD operations for users.</param>
        /// <param name="passwordService">Service used for hashing and verifying passwords.</param>
        /// <param name="userMapper">Mapper used for converting between models and DTOs.</param>
        public UserService(IUserRepository repo, IPasswordService passwordService, IUserMapper userMapper)
        {
            _repo = repo;
            _passwordService = passwordService;
            _userMapper = userMapper;
        }

        /// <summary>
        /// Retrieves all users from the system.
        /// </summary>
        /// <returns>A collection of <see cref="GetUserDto"/> representing all users.</returns>
        public async Task<IEnumerable<GetUserDto>> GetUsersAsync()
        {
            var models = await _repo.GetAllAsync();
            return models.Select(_userMapper.Map);
        }

        /// <summary>
        /// Retrieves a user by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>
        /// A <see cref="GetUserDto"/> if found, otherwise <c>null</c>.
        /// </returns>
        public async Task<GetUserDto?> GetUserByIdAsync(int id)
        {
            var model = await _repo.GetByIdAsync(id);
            return model != null ? _userMapper.Map(model) : null;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="dto">The registration data including email and password.</param>
        /// <returns>
        /// The ID of the newly created user.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an account with the same email already exists.
        /// </exception>
        public async Task<int> RegisterUserAsync(RegisterUserDTO dto)
        {
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

        /// <summary>
        /// Attempts to authenticate a user using email and password.
        /// </summary>
        /// <param name="dto">The login credentials.</param>
        /// <returns>
        /// The user ID if authentication succeeds; otherwise <c>null</c>.
        /// </returns>
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

