namespace backend.DTOs.Teams;

public class TeamCreateDto
{
    public string Name { get; set; }
    public bool IsCompetitive { get; set; }
    public int? MinAge { get; set; }

    public int OwnerId { get; set; }

    public List<int> MemberIds { get; set; } = new();
}
