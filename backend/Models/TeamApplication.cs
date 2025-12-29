namespace backend.Models
{
    public enum ApplicationStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class TeamApplication
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string DesiredPosition { get; set; }

        public ApplicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
