using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/studio")]
    public class StudioController : ControllerBase
    {
        private readonly StudioService _studioService;

        public StudioController(StudioService studioService)
        {
            _studioService = studioService;
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> CreateStudio([FromBody] CreateStudioRequestDTO dto)
        {
            try
            {
                var studioId = await _studioService.CreateStudioAsync(dto, Guid.Parse(User.FindFirst("userId")?.Value!));
                return Ok(studioId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{studioId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteStudio([FromRoute] Guid studioId)
        {
            try
            {
                await _studioService.DeleteStudioAsync(studioId, User);
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
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("{studioId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateStudio([FromRoute] Guid studioId, [FromBody] UpdateStudioRequestDTO dto)
        {
            try
            {
                await _studioService.UpdateStudioAsync(studioId, dto, User);
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
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{studioId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStudioById([FromRoute] Guid studioId)
        {
            try
            {
                var studio = await _studioService.GetStudiosByIdAsync(studioId, User);
                return Ok(studio);
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

        [HttpGet("{studioId}/users")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStudioUsers([FromRoute] Guid studioId)
        {
            try
            {
                var users = await _studioService.GetStudioUsersAsync(studioId, User);
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

        // POST: api/event/studio/{studio_id}
        [HttpPost("{studioId}/event")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> CreateEventForStudio([FromRoute] Guid studioId, [FromBody] CreateEventRequestDTO createDTO)
        {
            try
            {
                var eventId = await _studioService.CreateEventForStudioAsync(studioId, createDTO, User);
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


        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _studioService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // *** TELEGRAM-BOT *** // 


    }
}
