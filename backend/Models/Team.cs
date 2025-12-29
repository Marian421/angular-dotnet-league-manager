namespace backend.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Owner / Manager
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        // Members
        public List<TeamMember> Members { get; set; }

        // Matches (home + away inferred)
        public List<Match> Matches { get; set; }

        // Championships
        public List<ChampionshipTeam> Championships { get; set; }

        // Metadata for Find Team
        public bool IsCompetitive { get; set; }
        public int? MinAge { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

