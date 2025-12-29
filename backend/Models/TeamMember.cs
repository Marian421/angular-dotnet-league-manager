namespace backend.Models
{
    public enum TeamRole
    {
        Player,
        Captain
    }

    public enum MemberStatus
    {
        Active,
        Pending,
        Left
    }

    public class TeamMember
    {
        public int Id { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        // Nullable → manual players supported
        public int? UserId { get; set; }
        public User? User { get; set; }

        public string DisplayName { get; set; }

        public string Position { get; set; }

        public TeamRole Role { get; set; }
        public MemberStatus Status { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
    }
}
