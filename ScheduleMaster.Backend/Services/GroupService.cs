using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Models;

namespace ScheduleMaster.Services
{
    public class GroupService
    {
        private readonly ScheduleMasterDbContext _context;

        public GroupService(ScheduleMasterDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentToGroupAsync(Guid groupId, Guid studentId, ClaimsPrincipal currentUser)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new NotFoundException("Группа не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

            var isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == group.StudioId && su.IsLeader);
            if (!isLeader)
                throw new ForbiddenException("Только руководитель может добавлять участников");

            var exists = await _context.GroupsUsers
                .AnyAsync(gu => gu.GroupId == groupId && gu.StudentId == studentId);
            if (exists)
                throw new BadRequestExceptions("Пользователь уже в группе");

            await _context.GroupsUsers.AddAsync(new GroupUser
            {
                GroupId = groupId,
                StudentId = studentId
            });
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStudentFromGroupAsync(Guid groupId, Guid studentId, ClaimsPrincipal currentUser)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new NotFoundException("Группа не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

            var isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == group.StudioId && su.IsLeader);
            if (!isLeader)
                throw new ForbiddenException("Только руководитель может удалять участников");

            var groupUser = await _context.GroupsUsers
                .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.StudentId == studentId);
            if (groupUser == null)
                throw new NotFoundException("Пользователь не найден в группе");

            _context.GroupsUsers.Remove(groupUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserResponseDTO>> GetGroupUsersAsync(Guid groupId, ClaimsPrincipal currentUser)
        {

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new NotFoundException("Группа не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var leaderStudioIds = await _context.StudiosUsers
                .Where(su => su.StudentId == currentUserId && su.IsLeader)
                .Select(su => su.StudioId)
                .ToListAsync();

            var groupStudioId = await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => g.StudioId)
                .FirstOrDefaultAsync();

            var isLeaderOfGroup = leaderStudioIds.Contains(groupStudioId);
            if (!isLeaderOfGroup)
                throw new ForbiddenException("Нет прав для просмотра группы");

            var studentIds = await _context.GroupsUsers
                .Where(gu => gu.GroupId == groupId)
                .Select(gu => gu.StudentId)
                .ToListAsync();

            var students = await _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToListAsync();

            var userDtos = students.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Surname = u.Surname,
                Name = u.Name,
                MiddleName = u.MiddleName,
                Email = u.Email,
                Role = u.Role,
                Faculty = u.Faculty,
                studyGroup = u.GroupName,
                isLeader = isLeaderOfGroup
            }).ToList();

            return userDtos;
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
            // await _context.EventsStudios.AddAsync(new EventStudio
            // {
            //     EventId = ev.Id,
            //     StudioId = studioId
            // });

            await _context.SaveChangesAsync();

            return ev.Id;
        }



        // *** TELEGRAM-BOT *** // 

    }
}