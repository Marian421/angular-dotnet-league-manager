using backend.Models;

namespace backend.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<List<RefreshToken>> GetActiveByFamilyAsync(Guid familyId);
        Task AddAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
