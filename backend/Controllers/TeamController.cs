using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using backend.DTOs.Teams;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _teamService.GetTeamsAsync();

            if (!teams.Any())
            {
                return NotFound();
            }

            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> PostTeam([FromBody] TeamCreateDto dto)
        {
            try
            {
                var createdTeam = await _teamService.CreateTeamAsync(dto);
                return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
