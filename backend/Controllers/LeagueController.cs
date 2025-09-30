using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LeaguesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaguesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<League>> GetLeagues()
        {
            return _context.Leagues.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<League> GetLeague(int id)
        {
            var league = _context.Leagues.Find(id);

            if (league == null)
            {
                return NotFound();
            }

            return league;
        }

        [HttpPost]
        public ActionResult<League> CreateLeague(League league)
        {
            _context.Leagues.Add(league);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetLeague), new { id = league.Id }, league);
        }


    }

}