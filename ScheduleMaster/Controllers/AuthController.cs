using ScheduleMaster.Services;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;
using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Core.Tokens;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Post: api/auth/register 
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO registerDTO)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Post: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginDTO)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDTO);
                Response.Cookies.Append("cookie", response.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}