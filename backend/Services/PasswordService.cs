using Microsoft.AspNetCore.Identity;
using backend.Models;

namespace backend.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<User> _hasher;

        public PasswordService()
        {
            _hasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);

            return result == PasswordVerificationResult.Success
                   ||result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}