using backend.Models;

namespace backend.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
