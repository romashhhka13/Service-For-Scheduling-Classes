using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
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

        // GET: api/group
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        // GET: api/group/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new { message = "Group not found" });
            return Ok(group);
        }

        // POST: api/group
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Create([FromBody] CreateGroupDTO createDTO)
        {
            try
            {
                var group = await _groupService.CreateGroupAsync(createDTO);
                return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/group/{id}
        [HttpPatch("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupDTO updateDTO)
        {
            var group = await _groupService.UpdateGroupAsync(id, updateDTO);
            if (group == null)
                return NotFound(new { message = "Group not found" });
            return Ok(group);
        }

        // DELETE: api/group/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
                return NotFound(new { message = "Group not found" });
            return NoContent();
        }

        [HttpGet("details")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetGroupDetails()
        {
            var groups = await _groupService.GetGroupsWithDetailsAsync();
            return Ok(groups);
        }
    }
}
