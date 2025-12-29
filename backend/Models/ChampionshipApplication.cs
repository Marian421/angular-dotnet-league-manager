namespace backend.Models
{
    public enum ChampionshipApplicationStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class ChampionshipApplication
    {
        public int Id { get; set; }

        public int ChampionshipId { get; set; }
        public Championship Championship { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public ChampionshipApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
