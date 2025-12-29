namespace backend.Models
{
    public enum MatchType
    {
        Friendly,
        Championship
    }

    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        // Teams
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        // Optional championship
        public int? ChampionshipId { get; set; }
        public Championship? Championship { get; set; }

        public MatchType Type { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}

