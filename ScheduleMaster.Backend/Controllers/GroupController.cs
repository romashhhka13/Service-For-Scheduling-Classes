using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/group")]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;

        public GroupController(GroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("{groupId}/student/{studentId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddStudent(Guid groupId, Guid studentId)
        {
            try
            {
                await _groupService.AddStudentToGroupAsync(groupId, studentId, User);
                return Ok();
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestExceptions ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{groupId}/student/{studentId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> RemoveStudent(Guid groupId, Guid studentId)
        {
            try
            {
                await _groupService.RemoveStudentFromGroupAsync(groupId, studentId, User);
                return Ok();
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestExceptions ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{groupId}/users")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetGroupUsers([FromRoute] Guid groupId)
        {
            try
            {
                var users = await _groupService.GetGroupUsersAsync(groupId, User);
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

        [HttpPost("{groupId}/event")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> CreateEventForGroup([FromRoute] Guid groupId, [FromBody] CreateEventRequestDTO createDTO)
        {
            try
            {
                var eventId = await _groupService.CreateEventForGroupAsync(groupId, createDTO, User);
                return Ok(eventId);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestExceptions ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        // *** TELEGRAM-BOT *** // 


    }
}
