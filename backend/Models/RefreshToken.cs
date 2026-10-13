namespace backend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // SHA-256 of the token. The token itself is never stored.
        public string TokenHash { get; set; }

        // Shared by every token descended from the same login
        public Guid FamilyId { get; set; }

        public bool IsActive { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
