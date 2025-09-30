namespace backend.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Position { get; set; }
        public int? TeamId { get; set; }
        public Team Team {get; set;}
    }
}