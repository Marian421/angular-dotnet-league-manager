using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Match>> GetMatches()
        {
            return _context.Matches.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Match> GetMatch(int id)
        {
            var match = _context.Matches.Find(id);

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        [HttpPost]
        public ActionResult<Match> CreateMatch(Match match)
        {
            _context.Matches.Add(match);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
        }
    }
}