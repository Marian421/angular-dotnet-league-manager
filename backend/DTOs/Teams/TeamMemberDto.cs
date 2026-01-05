using backend.Models;

namespace backend.DTOs.Teams
{
    public class TeamMemberDto
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public TeamRole Role { get; set; }
    }
}
