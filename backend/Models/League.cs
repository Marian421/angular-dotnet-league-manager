namespace backend.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Team> Teams { get; set; }
        public List<Match> Matches { get; set; }
    }
}