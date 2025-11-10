using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/schedule
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        // GET: api/schedule/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(Guid id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found" });
            return Ok(schedule);
        }

        // POST: api/schedule
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDTO createDTO)
        {
            try
            {
                var schedule = await _scheduleService.CreateScheduleAsync(createDTO);
                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Patch: api/schedule/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleDTO updateDTO)
        {
            try
            {
                var schedule = await _scheduleService.UpdateScheduleAsync(id, updateDTO);
                if (schedule == null)
                    return NotFound(new { message = "Schedule not found" });
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/schedule/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result)
                return NotFound(new { message = "Schedule not found" });
            return NoContent();
        }

        // GET: api/schedule/by-group/{groupId}
        [HttpGet("by-group/{groupId}")]
        public async Task<IActionResult> GetByGroup(Guid groupId)
        {
            var schedules = await _scheduleService.GetSchedulesByGroupIdAsync(groupId);
            return Ok(schedules);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetScheduleDetails()
        {
            var details = await _scheduleService.GetScheduleDetailsAsync();
            return Ok(details);
        }

    }
}
