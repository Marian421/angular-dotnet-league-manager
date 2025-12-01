using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/users

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;


        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return user is null ? Ok(user) : NotFound();

        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser(RegisterUserDTO dto)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(dto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserDTO dto)
        {
            var user = await _userService.LoginUserAsync(dto);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { message = "Login succesful", userId = user.Id });
        }
    }
}
