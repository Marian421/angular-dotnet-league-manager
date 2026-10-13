using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    /// <summary>
    /// Provides authentication operations: logging in and issuing tokens.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokens;

        public AuthService(IUserRepository users, IPasswordService passwordService, ITokenService tokenService, IRefreshTokenRepository refreshTokens)
        {
            _users = users;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _refreshTokens = refreshTokens;
        }

        /// <summary>
        /// Attempts to authenticate a user using email and password.
        /// </summary>
        /// <param name="dto">The login credentials.</param>
        /// <returns>
        /// An access token and a refresh token if authentication succeeds; otherwise <c>null</c>.
        /// </returns>
        public async Task<LoginResponseDto?> LoginAsync(LoginUserDTO dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var ok = _passwordService.VerifyPassword(user, user.PasswordHash, dto.Password);
            if (!ok)
            {
                return null;
            }

            // Every login starts a new token family
            var refreshToken = _tokenService.CreateRefreshToken(user.Id, Guid.NewGuid());
            await _refreshTokens.AddAsync(refreshToken.Entity);
            await _refreshTokens.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = _tokenService.CreateAccessToken(user),
                RefreshToken = refreshToken.Token,
            };
        }
    }
}
