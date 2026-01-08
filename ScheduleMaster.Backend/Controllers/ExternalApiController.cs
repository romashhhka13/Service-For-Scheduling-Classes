using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.Services.ExternalApi;

namespace ScheduleMaster.Controllers;

[ApiController]
[Route("api/schedule")]
public class ExternalApiController : ControllerBase
{
    private readonly IScheduleApiService _scheduleApiService;
    // private readonly ILogger<ExternalApiController> _logger;

    public ExternalApiController(
        IScheduleApiService scheduleApiService,
        ILogger<ExternalApiController> logger)
    {
        _scheduleApiService = scheduleApiService;
        // _logger = logger;
    }

    /// <summary>
    /// Получить все доступные факультеты
    /// Данные кешируются в Redis на 24 часа
    /// </summary>
    /// <returns>Список факультетов</returns>
    [HttpGet("faculties")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFaculties()
    {
        try
        {
            var faculties = await _scheduleApiService.GetFacultiesAsync();

            if (faculties == null || faculties.Count == 0)
            {
                return Ok(new { message = "Факультеты не найдены", data = new List<object>() });
            }
            return Ok(new { message = "Факультеты успешно получены", data = faculties });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка при получении факультетов", details = ex.Message });
        }
    }

    /// <summary>
    /// Получить группы по ID факультета
    /// Данные кешируются в Redis на 24 часа
    /// </summary>
    /// <param name="facultyId">ID факультета</param>
    /// <returns>Список групп факультета</returns>
    [HttpGet("faculties/{facultyId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupsByFaculty(int facultyId)
    {
        try
        {

            var groups = await _scheduleApiService.GetGroupsByFacultyAsync(facultyId);

            if (groups == null || groups.Count == 0)
            {
                return Ok(new { message = $"Группы для факультета {facultyId} не найдены", data = new List<object>() });
            }

            // _logger.LogInformation($"Успешно получено {groups.Count} групп для факультета {facultyId}");
            return Ok(new { message = "Группы успешно получены", data = groups });
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, $"Ошибка при получении групп для факультета {facultyId}");
            return StatusCode(500, new { error = "Ошибка при получении групп", details = ex.Message });
        }
    }
}
