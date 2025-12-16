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
        [HttpGet()]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(User);
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



        // PUT: api/user/{id}
        [HttpPut()]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateUserById([FromBody] UpdateUserRequestDTO dto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(dto, User);
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
        [HttpDelete()]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                await _userService.DeleteUserAsync(User);
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

        [HttpGet("studios_as_member")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStudiosAsMember()
        {
            try
            {
                var studios = await _userService.GetStudiosAsMemberAsync(User);
                return Ok(studios);
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

        [HttpGet("studios_as_leader")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetStudiosAsLeader()
        {
            try
            {
                var studios = await _userService.GetStudiosAsLeaderAsync(User);
                return Ok(studios);
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

