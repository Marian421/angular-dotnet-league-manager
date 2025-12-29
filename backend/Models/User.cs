namespace backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Relationships
        public List<TeamMember> TeamMemberships { get; set; }
        public List<Team> ManagedTeams { get; set; }
        public List<Championship> OwnedChampionships { get; set; }
    }
}

