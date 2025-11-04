using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
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

        // GET: api/studio
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var studios = await _studioService.GetAllStudiosAsync();
            return Ok(studios);
        }

        // GET: api/studio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudioById(Guid id)
        {
            var studio = await _studioService.GetStudioByIdAsync(id);
            if (studio == null)
                return NotFound(new { message = "Studio not found" });

            return Ok(studio);
        }

        // POST: api/studio
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudioDTO createDTO)
        {
            try
            {
                var studio = await _studioService.CreateStudioAsync(createDTO);
                return CreatedAtAction(nameof(GetStudioById), new { id = studio.Id }, studio); // 201
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Patch: api/studio/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudioDTO updateDTO)
        {
            var studio = await _studioService.UpdateStudioAsync(id, updateDTO);
            if (studio == null)
                return NotFound(new { message = "Studio not found" });

            return Ok(studio);
        }

        // DELETE: api/studio/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _studioService.DeleteStudioAsync(id);
            if (!result)
                return NotFound(new { message = "Studio not found" });

            return NoContent();
        }
    }
}
