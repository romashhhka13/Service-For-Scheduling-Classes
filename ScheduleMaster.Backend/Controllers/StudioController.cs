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

        [HttpDelete("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteStudio([FromRoute] Guid id)
        {
            try
            {
                await _studioService.DeleteStudioAsync(id, User);
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

        [HttpPut("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateStudio([FromRoute] Guid id, [FromBody] UpdateStudioRequestDTO dto)
        {
            try
            {
                await _studioService.UpdateStudioAsync(id, dto, User);
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

        [HttpGet("user/member/{userId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStudiosAsMember(Guid userId)
        {
            try
            {
                var studios = await _studioService.GetStudiosAsMemberAsync(userId, User);
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

        [HttpGet("user/leader/{userId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetStudiosAsLeader(Guid userId)
        {
            try
            {
                var studios = await _studioService.GetStudiosAsLeaderAsync(userId, User);
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
