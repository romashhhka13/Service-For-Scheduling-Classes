using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Helpers;
using ScheduleMaster.Models;

namespace ScheduleMaster.Services
{
    public class StudioService
    {
        private readonly ScheduleMasterDbContext _context;

        public StudioService(ScheduleMasterDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateStudioAsync(CreateStudioRequestDTO dto, Guid currentUserId)
        {
            var studio = new Studio
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                StudioCategoryId = dto.CategoryId,
            };

            await _context.Studios.AddAsync(studio);

            await _context.StudiosUsers.AddAsync(new StudioUser
            {
                StudentId = currentUserId,
                StudioId = studio.Id,
                IsLeader = true
            });

            await _context.SaveChangesAsync();
            return studio.Id;
        }

        public async Task DeleteStudioAsync(Guid studioId, ClaimsPrincipal currentUser)
        {
            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isLeader = await _context.StudiosUsers.AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);
            if (!isLeader)
                throw new ForbiddenException("Только владелец может удалить студию"); // CreatorId

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudioAsync(Guid studioId, UpdateStudioRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var isLeader = await _context.StudiosUsers.AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);
            if (!isLeader)
                throw new ForbiddenException("Только владелец может удалить студию");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                studio.Title = dto.Title!;
            if (dto.CategoryId.HasValue)
                studio.StudioCategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
        }

        public async Task<StudioResponseDTO> GetStudiosByIdAsync(Guid studioId, ClaimsPrincipal currentUser)
        {

            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserIsLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId &&
                                su.StudioId == studioId &&
                                su.IsLeader);

            var memberCount = await _context.StudiosUsers
                .CountAsync(su => su.StudioId == studio.Id);

            return new StudioResponseDTO
            {
                Id = studio.Id,
                Title = studio.Title,
                CategoryId = studio.StudioCategoryId,
                MemberCount = memberCount,
                currentUserIsLeader = currentUserIsLeader
            };
        }


        public async Task<List<GetCategoriesResponseDTO>> GetCategoriesAsync()
        {
            var categories = await _context.StudiosCategories
                .Select(c => new { c.Id, c.Category })
                .ToListAsync();

            return categories.Select(c => new GetCategoriesResponseDTO
            {
                Id = c.Id,
                Category = c.Category
            }).ToList();
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
            // await _context.EventsStudios.AddAsync(new EventStudio
            // {
            //     EventId = ev.Id,
            //     StudioId = studioId
            // });

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


        public async Task<List<UserResponseDTO>> GetStudioUsersAsync(Guid studioId, ClaimsPrincipal currentUser)
        {
            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            bool isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            var studioUsers = await _context.StudiosUsers
                .Where(su => su.StudioId == studioId)
                .ToListAsync();

            var studentIds = studioUsers
                .Select(su => su.StudentId)
                .ToList();

            var students = await _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToListAsync();

            var userDtos = students.Select(u =>
            {
                var suLink = studioUsers.First(su => su.StudentId == u.Id);
                return new UserResponseDTO
                {
                    Id = u.Id,
                    Surname = u.Surname,
                    Name = u.Name,
                    MiddleName = u.MiddleName,
                    Email = u.Email,
                    Role = u.Role,
                    Faculty = u.Faculty,
                    studyGroup = u.GroupName,
                    isLeader = suLink.IsLeader   // ← индивидуальный флаг
                };
            }).ToList();

            return userDtos;
        }


        // *** TELEGRAM-BOT *** // 


    }
}
