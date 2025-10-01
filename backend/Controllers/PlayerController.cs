using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlayersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Player>> GetPlayers()
        {
            return _context.Players.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Player> GetPlayer(int id)
        {
            var player = _context.Players.Find(id);

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        [HttpPost]
        public ActionResult<Player> Createplayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }
    }
}