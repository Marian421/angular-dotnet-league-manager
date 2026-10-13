using backend.DTOs;
using backend.Repositories;

namespace backend.Services
{
    /// <summary>
    /// Provides authentication operations: logging in and refreshing tokens.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokens;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository users,
            IPasswordService passwordService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokens,
            TimeProvider timeProvider,
            ILogger<AuthService> logger)
        {
            _users = users;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _refreshTokens = refreshTokens;
            _timeProvider = timeProvider;
            _logger = logger;
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

        /// <summary>
        /// Exchanges a refresh token for a new access token and a new refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token previously issued to the client.</param>
        /// <returns>
        /// A new pair of tokens; <c>null</c> if the token is unknown, already used or expired.
        /// </returns>
        public async Task<LoginResponseDto?> RefreshAsync(string refreshToken)
        {
            var current = await _refreshTokens.GetByHashAsync(_tokenService.HashRefreshToken(refreshToken));
            if (current == null)
            {
                _logger.LogWarning("Refresh refused: unknown token");
                return null;
            }

            if (!current.IsActive)
            {
                // A spent token came back, so two parties hold tokens from this login.
                // Cancel whatever is still active in the family.
                var family = await _refreshTokens.GetActiveByFamilyAsync(current.FamilyId);
                foreach (var token in family)
                {
                    token.IsActive = false;
                }
                await _refreshTokens.SaveChangesAsync();

                _logger.LogWarning(
                    "Refresh refused: token reuse for user {UserId}, family {FamilyId} cancelled",
                    current.UserId, current.FamilyId);
                return null;
            }

            if (current.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                _logger.LogWarning("Refresh refused: expired token for user {UserId}", current.UserId);
                return null;
            }

            var user = await _users.GetByIdAsync(current.UserId);
            if (user == null)
            {
                _logger.LogWarning("Refresh refused: user {UserId} no longer exists", current.UserId);
                return null;
            }

            // Rotate: the old token is spent and its replacement stays in the same family
            current.IsActive = false;
            var replacement = _tokenService.CreateRefreshToken(user.Id, current.FamilyId);
            await _refreshTokens.AddAsync(replacement.Entity);
            await _refreshTokens.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = _tokenService.CreateAccessToken(user),
                RefreshToken = replacement.Token,
            };
        }
    }
}
