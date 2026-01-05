using backend.Models;
using backend.DTOs.Teams;
using backend.Repositories;
using backend.Mappers.Teams;

namespace backend.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repo;

        public TeamService(ITeamRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TeamSummaryDto>> GetTeamsAsync()
        {
            var teams = await _repo.GetTeamsAsync();

            return teams.Select(TeamMapper.ToDto);
        }

        public async Task<TeamSummaryDto> GetTeamByIdAsync(int id)
        {
            var team = await _repo.GetTeamByIdAsync(id);

            if (team is null)
            {
                return null;
            }

            return TeamMapper.ToDto(team);
        }
    }
}

