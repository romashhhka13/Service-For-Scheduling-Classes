using Microsoft.AspNetCore.Authorization;
using ScheduleMaster.Services;
using ScheduleMaster.DTOs;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.Helpers;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDTO)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDTO);
                Response.Cookies.Append("auth_token", response.Token);
                return Ok(response);
            }
            catch (BadRequestExceptions ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        // Post: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDTO);
                Response.Cookies.Append("auth_token", response.Token);
                return Ok(response);
            }
            catch (UnauthorizedExceptions ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("check")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            return Ok(new { message = "Авторизован" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("auth_token", "");
            return Ok(new { message = "Logged out" });
        }
    }
}