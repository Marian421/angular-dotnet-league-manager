using backend.Models;

namespace backend.DTOs.Teams;

public class TeamDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public OwnerDto Owner { get; set; }

    public bool IsCompetitive { get; set; }
    public int? MinAge { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<TeamMemberDto> Members { get; set; }
}
