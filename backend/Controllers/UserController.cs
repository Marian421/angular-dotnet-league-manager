using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using backend.DTOs;
using Microsoft.OpenApi.Validations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/users

    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;


        public UsersController(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser(RegisterUserDTO dto)
        {

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return Conflict(new { message = "There already exists an account with this email" });
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = "User"
            };

            user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var ok = _passwordService.VerifyPassword(user, user.PasswordHash, dto.Password);
            if (!ok)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { message = "Login succesfull", userId = user.Id });
        }
    }
}