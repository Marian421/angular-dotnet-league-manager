using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDTO dto)
        {
            var tokens = await _authService.LoginAsync(dto);
            if (tokens == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshTokenDto dto)
        {
            var tokens = await _authService.RefreshAsync(dto.RefreshToken);
            if (tokens == null)
            {
                // Deliberately no detail: the caller must not learn why it was refused
                return Unauthorized();
            }

            return Ok(tokens);
        }
    }
}
