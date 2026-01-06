using Microsoft.AspNetCore.Mvc;
using ScheduleMaster.Services.ExternalApi;

namespace ScheduleMaster.Controllers;

[ApiController]
[Route("api/gubkin")]
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
    [HttpGet("schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFaculties()
    {
        try
        {
            // _logger.LogInformation("Запрос всех факультетов");

            var faculties = await _scheduleApiService.GetFacultiesAsync();

            if (faculties == null || faculties.Count == 0)
            {
                // _logger.LogWarning("Факультеты не найдены");
                return Ok(new { message = "Факультеты не найдены", data = new List<object>() });
            }

            // _logger.LogInformation($"Успешно получено {faculties.Count} факультетов");
            return Ok(new { message = "Факультеты успешно получены", data = faculties });
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "Ошибка при получении факультетов");
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
            if (facultyId <= 0)
            {
                // _logger.LogWarning($"Некорректный ID факультета: {facultyId}");
                return BadRequest(new { error = "ID факультета должен быть положительным числом" });
            }

            // _logger.LogInformation($"Запрос групп для факультета {facultyId}");

            var groups = await _scheduleApiService.GetGroupsByFacultyAsync(facultyId);

            if (groups == null || groups.Count == 0)
            {
                // _logger.LogWarning($"Группы для факультета {facultyId} не найдены");
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
