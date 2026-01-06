

namespace ScheduleMaster.Services.ExternalApi
{

    public interface IScheduleApiService
    {
        /// <summary>
        /// Получить все факультеты
        /// </summary>
        Task<List<Faculty>> GetFacultiesAsync();

        /// <summary>
        /// Получить группы по факультету
        /// </summary>
        Task<List<StudyGroup>> GetGroupsByFacultyAsync(int facultyId);
    }
}