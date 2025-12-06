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

        public async Task<Guid> CreateEventForGroupAsync(Guid groupId, CreateEventRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new NotFoundException("Группа не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;

            var isLeader = await _context.GroupsUsers
                .AnyAsync(gu => gu.StudentId == currentUserId && gu.GroupId == groupId);

            if (role != "admin" && !isLeader)
                throw new ForbiddenException("Только админ или руководитель группы");

            if (dto.StartDateTime <= DateTime.UtcNow)
                throw new BadRequestExceptions("Событие должно начинаться в будущем");

            if (dto.EndDateTime <= dto.StartDateTime)
                throw new BadRequestExceptions("Дата окончания должна быть позже даты начала");

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime,
                Location = dto.Location
            };

            await _context.Events.AddAsync(ev);
            await _context.EventsGroups.AddAsync(new EventGroup
            {
                EventId = ev.Id,
                GroupId = groupId
            });

            var studioId = await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => g.StudioId)
                .FirstOrDefaultAsync();
            await _context.EventsStudios.AddAsync(new EventStudio
            {
                EventId = ev.Id,
                StudioId = studioId
            });

            await _context.SaveChangesAsync();

            return ev.Id;
        }

        public async Task<Guid> CreateEventForStudioAsync(Guid studioId, CreateEventRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;

            var isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            if (role != "admin" && !isLeader)
                throw new ForbiddenException("Только руководитель данной студии может создавать события для неё");

            if (dto.StartDateTime <= DateTime.UtcNow)
                throw new BadRequestExceptions("Событие должно начинаться в будущем");

            if (dto.EndDateTime <= dto.StartDateTime)
                throw new BadRequestExceptions("Дата окончания должна быть позже даты начала");

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.EndDateTime,
                Location = dto.Location
            };

            await _context.Events.AddAsync(ev);
            await _context.EventsStudios.AddAsync(new EventStudio
            {
                EventId = ev.Id,
                StudioId = studioId
            });

            // var groupIds = await _context.Groups
            //     .Where(g => g.StudioId == studioId)
            //     .Select(g => g.Id)
            //     .ToListAsync();
            // foreach (var groupId in groupIds)
            // {
            //     await _context.EventsGroups.AddAsync(new EventGroup
            //     {
            //         EventId = ev.Id,
            //         GroupId = groupId
            //     });
            // }

            await _context.SaveChangesAsync();

            return ev.Id;
        }

        public async Task<List<EventResponseDTO>> GetUserEventsAsync(Guid userId, DateTime startDate,
         DateTime endDate, Guid currentUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            if (userId != currentUserId)
                throw new ForbiddenException("");

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

            var eventStudioIds = await _context.EventsStudios
                .Where(es => es.EventId == eventId)
                .Select(es => es.StudioId)
                .ToListAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isInGroup = await _context.GroupsUsers
                .AnyAsync(gu => gu.StudentId == currentUserId && eventGroupIds.Contains(gu.GroupId));

            var isInStudio = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && eventStudioIds.Contains(su.StudioId));

            var currentUserRole = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole == "admin" || isInGroup || isInStudio)
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

            var studioId = await _context.EventsStudios
                .Where(es => es.EventId == eventId)
                .Select(eg => eg.StudioId)
                .FirstOrDefaultAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            if (role != "admin" && !isLeader)
                throw new ForbiddenException("Нет прав на редактирование");

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

            var studioId = await _context.EventsStudios
                .Where(es => es.EventId == eventId)
                .Select(eg => eg.StudioId)
                .FirstOrDefaultAsync();

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "admin" && !isLeader)
                throw new ForbiddenException("Нет прав на удаление");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }

    }
}
