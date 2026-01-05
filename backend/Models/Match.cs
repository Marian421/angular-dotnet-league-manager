using Microsoft.EntityFrameworkCore;

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
        public int TeamAId { get; set; }
        public Team TeamA { get; set; }

        public int TeamBId { get; set; }
        public Team TeamB { get; set; }

        // Location
        public MatchLocation Location { get; set; } = new();

        // Optional championship
        public int? ChampionshipId { get; set; }
        public Championship? Championship { get; set; }

        public MatchType Type { get; set; }

        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
    }

    [Owned]
    public class MatchLocation
    {
        public string City { get; set; }
        public string Venue { get; set; }
        public string Notes { get; set; }
    }
}

