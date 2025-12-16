using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;
using ScheduleMaster.Helpers;
using System.Security.Claims;

namespace ScheduleMaster.Services
{
    public class UserService
    {
        private readonly ScheduleMasterDbContext _context;

        public UserService(ScheduleMasterDbContext context)
        {
            _context = context;
        }

        private async Task<bool> IsUserStudioLeaderAsync(Guid userId)
        {
            // var studioIdsOfStudent = await _context.StudiosUsers
            //     .Where(su => su.StudentId == studentUserId)
            //     .Select(su => su.StudioId)
            //     .ToListAsync();

            // var studiosLeadedByUser = await _context.StudiosUsers
            //     .Where(su => su.StudentId == leaderUserId && su.IsLeader)
            //     .Select(su => su.StudioId)
            //     .ToListAsync();

            // return studioIdsOfStudent.Intersect(studiosLeadedByUser).Any();

            var studiosLeadedByUser = await _context.StudiosUsers
                .Where(su => su.StudentId == userId && su.IsLeader)
                .Select(su => su.StudioId)
                .ToListAsync();

            return studiosLeadedByUser.Any();
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(ClaimsPrincipal currentUser)
        {

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var currentUserRole = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            bool isLeader = await IsUserStudioLeaderAsync(currentUserId);
            // if (!isLeader)
            //     throw new ForbiddenException("Нельзя получить другого пользователя");

            return new UserResponseDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                studyGroup = user.GroupName,
                isLeader = isLeader
            };
        }


        public async Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            if (!string.IsNullOrWhiteSpace(dto.Surname))
                user.Surname = dto.Surname;
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.MiddleName))
                user.MiddleName = dto.MiddleName;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Faculty))
                user.Faculty = dto.Faculty;
            if (!string.IsNullOrWhiteSpace(dto.GroupName))
                user.GroupName = dto.GroupName;

            await _context.SaveChangesAsync();

            bool isLeader = await IsUserStudioLeaderAsync(currentUserId);

            return new UserResponseDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                studyGroup = user.GroupName,
                isLeader = isLeader
            };
        }

        public async Task DeleteUserAsync(ClaimsPrincipal currentUser)
        {
            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StudioResponseDTO>> GetStudiosAsMemberAsync(ClaimsPrincipal currentUser)
        {

            var userId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

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
                CategoryId = s.StudioCategoryId,
                MemberCount = _context.StudiosUsers.Count(su => su.StudioId == s.Id)
            }).ToList();
        }

        public async Task<List<StudioResponseDTO>> GetStudiosAsLeaderAsync(ClaimsPrincipal currentUser)
        {
            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            // if (user == null)
            //     throw new NotFoundException("Пользователь не найден");

            var userId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);

            // if (currentUserId != userId)
            //     throw new ForbiddenException("Можно просматривать только свои студии");

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
                CategoryId = s.StudioCategoryId,
                MemberCount = _context.StudiosUsers.Count(su => su.StudioId == s.Id)
            }).ToList();
        }
    }
}