using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.DTOs;
using ScheduleMaster.Services;

namespace ScheduleMaster.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        // private readonly UserService _userService;

        // public UserController(UserService userService)
        // {
        //     _userService = userService;
        // }

        // // GET: api/user
        // [HttpGet]
        // [Authorize(Roles = "admin")]
        // public async Task<IActionResult> GetAll()
        // {
        //     var users = await _userService.GetAllUsersAsync();
        //     return Ok(users);
        // }

        // // GET: api/user/{id}
        // [HttpGet("{id}")]
        // [Authorize(Roles = "admin, user")]
        // public async Task<IActionResult> GetUserById(Guid id)
        // {
        //     var user = await _userService.GetUserByIdAsync(id);
        //     if (user == null)
        //         return NotFound(new { message = "User not found!" });

        //     return Ok(user);
        // }

        // // POST: api/user
        // [HttpPost]
        // [Authorize(Roles = "admin")]
        // public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createDTO)
        // {
        //     try
        //     {
        //         var user = await _userService.CreateUserAsync(createDTO);
        //         return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // // Patch: api/user/{id}
        // [HttpPatch("{id}")]
        // [Authorize(Roles = "admin, user")]
        // public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO updateDTO)
        // {
        //     var studio = await _userService.UpdateUserAsync(id, updateDTO);
        //     if (studio == null)
        //         return NotFound(new { message = "User not found" });

        //     return Ok(studio);
        // }

        // // PUT: api/user/{id}
        // // [HttpPut("{id}")]
        // // public async Task<IActionResult> UpdatePassword()
        // // {

        // // }


        // // DELETE: api/studio/{id}
        // [HttpDelete("{id}")]
        // [Authorize(Roles = "admin")]
        // public async Task<IActionResult> Delete(Guid id)
        // {
        //     var result = await _userService.DeleteUserAsync(id);
        //     if (!result)
        //         return NotFound(new { message = "User not found" });

        //     return NoContent();
        // }

        // [HttpGet("{id}/schedule")]
        // [Authorize(Roles = "admin, user")]
        // public async Task<IActionResult> GetUserSchedule(Guid id)
        // {
        //     try
        //     {
        //         var schedule = await _userService.GetStudentScheduleAsync(id);
        //         return Ok(schedule);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }
    }
}

