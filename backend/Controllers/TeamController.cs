using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
            return _context.Teams.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Team> GetTeam(int id)
        {
            var team = _context.Teams.Find(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        [HttpPost]
        public ActionResult<Team> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

    }
}