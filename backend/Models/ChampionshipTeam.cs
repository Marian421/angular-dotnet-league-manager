namespace backend.Models
{
    public class ChampionshipTeam
    {
        public int ChampionshipId { get; set; }
        public Championship Championship { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
