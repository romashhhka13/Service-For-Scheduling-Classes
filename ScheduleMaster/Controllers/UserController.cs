using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id, User);
                return Ok(user);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message }); // Forbidd
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("studio/{studioId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStudioUsers([FromRoute] Guid studioId)
        {
            try
            {
                var users = await _userService.GetStudioUsersAsync(studioId, User);
                return Ok(users);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка получения" });
            }
        }

        [HttpGet("group/{groupId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetGroupUsers([FromRoute] Guid groupId)
        {
            try
            {
                var users = await _userService.GetGroupUsersAsync(groupId, User);
                return Ok(users);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ошибка получения" });
            }
        }


        // PUT: api/user/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateUserById([FromRoute] Guid id, [FromBody] UpdateUserRequestDTO dto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, dto, User);
                return Ok(user);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            try
            {
                var currentUserId = User.FindFirst("userId")?.Value;
                await _userService.DeleteUserAsync(id, new Guid(currentUserId!));
                return NoContent();
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
    }
}

