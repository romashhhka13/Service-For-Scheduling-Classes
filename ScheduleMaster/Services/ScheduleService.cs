using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;

namespace ScheduleMaster.Services
{
    public class ScheduleService
    {
        private readonly ScheduleMasterDbContext _context;

        public ScheduleService(ScheduleMasterDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleDTO>> GetAllSchedulesAsync()
        {
            return await _context.Schedules.Select(
                schedule => new ScheduleDTO
                {
                    Id = schedule.Id,
                    StudioId = schedule.StudioId,
                    GroupId = schedule.GroupId,
                    StartDateTime = schedule.StartDateTime,
                    EndDateTime = schedule.EndDateTime,
                    Location = schedule.Location,
                    WeekType = schedule.WeekType,
                    IsRecurring = schedule.IsRecurring
                })
                .ToListAsync();
        }

        public async Task<ScheduleDTO?> GetScheduleByIdAsync(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return null;

            return new ScheduleDTO
            {
                Id = schedule.Id,
                StudioId = schedule.StudioId,
                GroupId = schedule.GroupId,
                StartDateTime = schedule.StartDateTime,
                EndDateTime = schedule.EndDateTime,
                Location = schedule.Location,
                WeekType = schedule.WeekType,
                IsRecurring = schedule.IsRecurring
            };
        }

        public async Task<ScheduleDTO> CreateScheduleAsync(CreateScheduleDTO createDTO)
        {
            // Проверка существования студии
            var studioExists = await _context.Studios.AnyAsync(studio => studio.Id == createDTO.StudioId);
            if (!studioExists)
                throw new Exception($"Студия с ID '{createDTO.StudioId}' не найдена");

            if (createDTO.GroupId != null)
            {
                // Проверка существования группы
                var groupExists = await _context.Groups.AnyAsync(group => group.Id == createDTO.GroupId);
                if (!groupExists)
                    throw new Exception($"Группа с ID '{createDTO.GroupId}' не найдена");
            }

            // Валидация времени
            if (createDTO.EndDateTime <= createDTO.StartDateTime)
                throw new Exception("Время окончания должно быть позже времени начала");

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                StudioId = createDTO.StudioId,
                GroupId = createDTO.GroupId,
                StartDateTime = createDTO.StartDateTime,
                EndDateTime = createDTO.EndDateTime,
                Location = createDTO.Location,
                WeekType = createDTO.WeekType,
                IsRecurring = createDTO.IsRecurring
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new ScheduleDTO
            {
                Id = schedule.Id,
                StudioId = schedule.StudioId,
                GroupId = schedule.GroupId,
                StartDateTime = schedule.StartDateTime,
                EndDateTime = schedule.EndDateTime,
                Location = schedule.Location,
                WeekType = schedule.WeekType,
                IsRecurring = schedule.IsRecurring
            };
        }

        public async Task<ScheduleDTO?> UpdateScheduleAsync(Guid id, UpdateScheduleDTO updateDTO)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return null;

            if (updateDTO.StartDateTime.HasValue)
                schedule.StartDateTime = updateDTO.StartDateTime.Value;

            if (updateDTO.EndDateTime.HasValue)
                schedule.EndDateTime = updateDTO.EndDateTime.Value;

            if (schedule.EndDateTime <= schedule.StartDateTime)
                throw new Exception("Время окончания должно быть позже времени начала");

            if (!string.IsNullOrEmpty(updateDTO.Location))
                schedule.Location = updateDTO.Location;

            if (!string.IsNullOrEmpty(updateDTO.WeekType))
                schedule.WeekType = updateDTO.WeekType;

            if (updateDTO.IsRecurring.HasValue)
                schedule.IsRecurring = updateDTO.IsRecurring.Value;

            await _context.SaveChangesAsync();

            return new ScheduleDTO
            {
                Id = schedule.Id,
                StudioId = schedule.StudioId,
                GroupId = schedule.GroupId,
                StartDateTime = schedule.StartDateTime,
                EndDateTime = schedule.EndDateTime,
                Location = schedule.Location,
                WeekType = schedule.WeekType,
                IsRecurring = schedule.IsRecurring
            };
        }

        public async Task<bool> DeleteScheduleAsync(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        // LINQ запрос: получить расписания для конкретной группы
        public async Task<List<ScheduleDTO>?> GetSchedulesByGroupIdAsync(Guid groupId)
        {
            // var groupExists = await _context.Groups.AnyAsync(group => group.Id == groupId);
            // if (!groupExists)
            //     throw new Exception($"Группа с ID '{groupId}' не найдена");

            return await _context.Schedules
                .Where(schedule => schedule.GroupId == groupId)
                .Select(schedule => new ScheduleDTO
                {
                    Id = schedule.Id,
                    StudioId = schedule.StudioId,
                    GroupId = schedule.GroupId,
                    StartDateTime = schedule.StartDateTime,
                    EndDateTime = schedule.EndDateTime,
                    Location = schedule.Location,
                    WeekType = schedule.WeekType,
                    IsRecurring = schedule.IsRecurring
                })
                .ToListAsync();
        }
    }
}
