using backend.Models;
using backend.DTOs.Teams;
using backend.Repositories;
using backend.Mappers.Teams;

namespace backend.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ICurrentUser _currentUser;

        public TeamService(ITeamRepository repo, IUserRepository userRepo, ICurrentUser currentUser)
        {
            _repo = repo;
            _userRepo = userRepo;
            _currentUser = currentUser;
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

        public async Task<TeamDetailsDto> CreateTeamAsync(TeamCreateDto dto)
        {
            // The owner is the caller, taken from the token
            var ownerId = _currentUser.Id;

            // Check if Owner exists
            var ownerExists = await _userRepo.GetByIdAsync(ownerId);
            if (ownerExists is null)
            {
                throw new KeyNotFoundException($"User with id {ownerId} not found");
            }

            // Create the team entity
            var team = new Team
            {
                Name = dto.Name,
                MinAge = dto.MinAge,
                IsCompetitive = dto.IsCompetitive,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                Members = new List<TeamMember>()
            };

            // Add the entity in the database
            await _repo.AddAsync(team);
            await _repo.SaveChangesAsync();

            // Return the summary
            return TeamMapper.ToDetailsDto(team);
        }
    }
}

