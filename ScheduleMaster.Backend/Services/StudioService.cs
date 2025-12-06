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

        public async Task<List<StudioResponseDTO>> GetStudiosAsMemberAsync(Guid userId, ClaimsPrincipal currentUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

            if (currentUserId != userId)
                throw new ForbiddenException("Можно просматривать только свои студии-участия");

            var studioIds = await _context.StudiosUsers
                .Where(su => su.StudentId == userId && !su.IsLeader)
                .Select(su => su.StudioId)
                .ToListAsync();

            var studios = await _context.Studios
                .Where(s => studioIds.Contains(s.Id))
                .ToListAsync();

            return studios.Select(s => new StudioResponseDTO
            {
                Id = s.Id,
                Title = s.Title,
                CategoryId = s.StudioCategoryId
            }).ToList();
        }

        public async Task<List<StudioResponseDTO>> GetStudiosAsLeaderAsync(Guid userId, ClaimsPrincipal currentUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

            if (currentUserId != userId)
                throw new ForbiddenException("Можно просматривать только свои студии");

            var studioIds = await _context.StudiosUsers
                .Where(su => su.StudentId == userId && su.IsLeader)
                .Select(su => su.StudioId)
                .ToListAsync();

            var studios = await _context.Studios
                .Where(s => studioIds.Contains(s.Id))
                .ToListAsync();

            return studios.Select(s => new StudioResponseDTO
            {
                Id = s.Id,
                Title = s.Title,
                CategoryId = s.StudioCategoryId
            }).ToList();
        }
    }
}
