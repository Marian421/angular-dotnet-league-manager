using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task AddAsync(Team team);
        Task SaveChangesAsync();
    }
}
