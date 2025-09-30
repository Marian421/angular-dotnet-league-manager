namespace backend.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int LeagueId { get; set; }
        public League League {get; set; }

        public int CoachId { get; set; }
        public User Coach { get; set; }

        public List<Player> Players { get; set; }
        public List<Match> HomeMatches { get; set; }
        public List<Match> AwayMatches { get; set; }
    }
}