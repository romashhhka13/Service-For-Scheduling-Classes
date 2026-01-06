using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Models;

namespace ScheduleMaster.Services
{
    public class EventService
    {
        private readonly ScheduleMasterDbContext _context;

        public EventService(ScheduleMasterDbContext context)
        {
            _context = context;
        }


        public async Task<List<EventResponseDTO>> GetUserEventsAsync(DateTime startDate,
         DateTime endDate, ClaimsPrincipal User)
        {
            Guid userId = Guid.Parse(User.FindFirst("userId")?.Value!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var groupIds = await _context.GroupsUsers
                .Where(gu => gu.StudentId == userId)
                .Select(gu => gu.GroupId)
                .ToListAsync();

            // var studioIds = await _context.StudiosUsers
            //     .Where(su => su.StudentId == userId)
            //     .Select(su => su.StudioId)
            //     .ToListAsync();

            var eventGroupIds = await _context.EventsGroups
                .Where(eg => groupIds.Contains(eg.GroupId))
                .Select(eg => eg.EventId)
                .ToListAsync();

            // var eventStudioIds = await _context.EventsStudios
            //     .Where(es => studioIds.Contains(es.StudioId))
            //     .Select(es => es.EventId)
            //     .ToListAsync();

            // var allEventIds = eventGroupIds.Union(eventStudioIds).Distinct().ToList();

            var events = await _context.Events
                .Where(e => eventGroupIds.Contains(e.Id) &&
                            e.StartDateTime >= startDate &&
                            e.StartDateTime < endDate)
                .ToListAsync();

            return events.Select(e => new EventResponseDTO
            {
                Id = e.Id,
                Title = e.Title,
                StartDateTime = e.StartDateTime,
                EndDateTime = e.EndDateTime,
                Location = e.Location
            }).ToList();
        }

        public async Task<EventResponseDTO> GetEventByIdAsync(Guid eventId, ClaimsPrincipal currentUser)
        {

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null)
                throw new NotFoundException("Событие не найдено");

            var eventGroupIds = await _context.EventsGroups
                .Where(eg => eg.EventId == eventId)
                .Select(eg => eg.GroupId)
                .ToListAsync();

            // var eventStudioIds = await _context.EventsStudios
            //     .Where(es => es.EventId == eventId)
            //     .Select(es => es.StudioId)
            //     .ToListAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isInGroup = await _context.GroupsUsers
                .AnyAsync(gu => gu.StudentId == currentUserId && eventGroupIds.Contains(gu.GroupId));

            // var isInStudio = await _context.StudiosUsers
            //     .AnyAsync(su => su.StudentId == currentUserId && eventStudioIds.Contains(su.StudioId));

            var currentUserRole = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole == "admin" || isInGroup /*|| isInStudio*/)
            {
                return new EventResponseDTO
                {
                    Id = ev.Id,
                    Title = ev.Title,
                    StartDateTime = ev.StartDateTime,
                    EndDateTime = ev.EndDateTime,
                    Location = ev.Location
                };
            }
            else
            {
                throw new ForbiddenException("Нет прав на просмотр события");
            }

        }


        public async Task<Guid> UpdateEventAsync(Guid eventId, UpdateEventRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null)
                throw new NotFoundException("Событие не найдено");

            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;

            if (ev.StartDateTime <= DateTime.UtcNow)
                throw new ForbiddenException("Нельзя редактировать прошедшее или текущее событие");

            // var studioId = await _context.EventsStudios
            //     .Where(es => es.EventId == eventId)
            //     .Select(eg => eg.StudioId)
            //     .FirstOrDefaultAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            // var isLeader = await _context.StudiosUsers
            //     .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            // if (role != "admin" && !isLeader)
            //     throw new ForbiddenException("Нет прав на редактирование");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                ev.Title = dto.Title!;
            if (dto.StartDateTime.HasValue)
                ev.StartDateTime = dto.StartDateTime.Value;
            if (dto.EndDateTime.HasValue)
                ev.EndDateTime = dto.EndDateTime.Value;
            if (!string.IsNullOrWhiteSpace(dto.Location))
                ev.Location = dto.Location;

            await _context.SaveChangesAsync();

            return ev.Id;
        }


        public async Task DeleteEventAsync(Guid eventId, ClaimsPrincipal currentUser)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null)
                throw new NotFoundException("Событие не найдено");

            if (ev.StartDateTime <= DateTime.UtcNow)
                throw new BadRequestExceptions("Нельзя удалить прошедшее или текущее событие");

            // var studioId = await _context.EventsStudios
            //     .Where(es => es.EventId == eventId)
            //     .Select(eg => eg.StudioId)
            //     .FirstOrDefaultAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            // var isLeader = await _context.StudiosUsers
            //     .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "admin"/* && !isLeader*/)
                throw new ForbiddenException("Нет прав на удаление");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }


        // *** TELEGRAM-BOT *** // 

    }
}
