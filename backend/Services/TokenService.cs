using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly TimeProvider _timeProvider;

        public TokenService(IOptions<JwtSettings> settings, TimeProvider timeProvider)
        {
            _settings = settings.Value;
            _timeProvider = timeProvider;
        }

        public string CreateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                }),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(_settings.AccessTokenMinutes),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }

        public IssuedRefreshToken CreateRefreshToken(int userId, Guid familyId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashRefreshToken(token),
                FamilyId = familyId,
                IsActive = true,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_settings.RefreshTokenDays),
            };

            return new IssuedRefreshToken(token, entity);
        }

        public string HashRefreshToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }
    }
}
