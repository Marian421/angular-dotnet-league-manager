using backend.DTOs.Teams;
using backend.Models;

public interface ITeamService
{
    Task<IEnumerable<TeamSummaryDto>> GetTeamsAsync();
    Task<TeamSummaryDto?> GetTeamByIdAsync(int id);
    Task<TeamDetailsDto> CreateTeamAsync(TeamCreateDto dto);
}
