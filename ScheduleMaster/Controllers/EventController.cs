using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/event")]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/event
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> GetUserEvents([FromRoute] Guid userId,
         [FromQuery] DateTime? startDate = null,
         [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var actualStart = startDate.HasValue
                    ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc)
                    : DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);

                var actualEnd = endDate.HasValue
                    ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc)
                    : DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(1), DateTimeKind.Utc);
                var events = await _eventService.GetUserEventsAsync(userId, actualStart, actualEnd, Guid.Parse(User.FindFirst("userId")?.Value!));
                return Ok(events);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> GetEventById([FromRoute] Guid id)
        {
            try
            {
                var ev = await _eventService.GetEventByIdAsync(id, User);
                return Ok(ev);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // POST: api/event/group/{group_id}
        [HttpPost("group/{groupId}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> CreateEventForGroup([FromRoute] Guid groupId, [FromBody] CreateEventRequestDTO createDTO)
        {
            try
            {
                var eventId = await _eventService.CreateEventForGroupAsync(groupId, createDTO, User);
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

        // POST: api/event/studio/{studio_id}
        [HttpPost("studio/{studioId}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> CreateEventForStudio([FromRoute] Guid studioId, [FromBody] CreateEventRequestDTO createDTO)
        {
            try
            {
                var eventId = await _eventService.CreateEventForStudioAsync(studioId, createDTO, User);
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

        // PUT: api/event/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> UpdateEvent([FromRoute] Guid id, [FromBody] UpdateEventRequestDTO updateDTO)
        {
            try
            {
                await _eventService.UpdateEventAsync(id, updateDTO, User);
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
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE: api/event/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteEvent([FromRoute] Guid id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id, User);
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
            catch (BadRequestExceptions ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
