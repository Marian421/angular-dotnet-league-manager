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
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetUsers()
        {
            var dtos = await _userService.GetUsersAsync();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
        {
            var dto = await _userService.GetUserByIdAsync(id);
            return dto != null ? Ok(dto) : NotFound();
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser(RegisterUserDTO dto)
        {
            try
            {
                // e mai ok sa returnezi doar id user ca sa poti schimba dto-ul mai tarziu
                var id = await _userService.RegisterUserAsync(dto);
                var getDto = await _userService.GetUserByIdAsync(id);
                return CreatedAtAction(nameof(GetUser), new { id = id }, getDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserDTO dto)
        {
            var id = await _userService.LoginUserAsync(dto);
            if (id == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { message = "Login succesful", userId = (int)id });
        }
    }
}
