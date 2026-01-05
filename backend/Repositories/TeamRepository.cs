using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
