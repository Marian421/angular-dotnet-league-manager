namespace backend.Models
{
    public class Championship
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Ownership
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        // Participants
        public List<ChampionshipTeam> Teams { get; set; }
        public List<Match> Matches { get; set; }
    }
}

