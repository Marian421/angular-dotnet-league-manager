using backend.DTOs.Teams;
using backend.Models;

namespace backend.Mappers.Teams;

public class TeamMapper
{
    public static TeamSummaryDto ToDto(Team team)
    {
        return new TeamSummaryDto
        {
            Id = team.Id,
            Name = team.Name,
            IsCompetitive = team.IsCompetitive
        };
    }

    public static TeamDetailsDto ToDetailsDto(Team team)
    {
        return new TeamDetailsDto
        {
            Id = team.Id,
            Name = team.Name,
            IsCompetitive = team.IsCompetitive,
            MinAge = team.MinAge,
            CreatedAt = team.CreatedAt,
            Owner = new OwnerDto
            {
                Id = team.Owner.Id,
                Name = team.Owner.Name
            },
            Members = team.Members
            .Select(m => new TeamMemberDto
            {
                UserId = m.UserId,
                UserName = m.User.Name,
                Role = m.Role
            }).ToList()
        };
    }
}
