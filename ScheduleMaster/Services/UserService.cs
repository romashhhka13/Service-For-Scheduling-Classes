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

        private async Task<bool> IsUserStudioLeaderAsync(Guid leaderUserId, Guid studentUserId)
        {
            var studioIdsOfStudent = await _context.StudiosUsers
                .Where(su => su.StudentId == studentUserId)
                .Select(su => su.StudioId)
                .ToListAsync();

            var studiosLeadedByUser = await _context.StudiosUsers
                .Where(su => su.StudentId == leaderUserId && su.IsLeader)
                .Select(su => su.StudioId)
                .ToListAsync();

            return studioIdsOfStudent.Intersect(studiosLeadedByUser).Any(); ;
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(Guid userId, ClaimsPrincipal currentUser)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            var currentUserRole = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole == "admin" || currentUserId == userId)
            {
                return new UserResponseDTO
                {
                    Id = user.Id,
                    Surname = user.Surname,
                    Name = user.Name,
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    Role = user.Role,
                    Faculty = user.Faculty,
                    GroupName = user.GroupName,
                };
            }

            bool isLeader = await IsUserStudioLeaderAsync(currentUserId, userId);
            if (!isLeader)
                throw new ForbiddenException("Нельзя получить другого пользователя");

            return new UserResponseDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
            };
        }

        public async Task<List<UserResponseDTO>> GetStudioUsersAsync(Guid studioId, ClaimsPrincipal currentUser)
        {
            var studio = await _context.Studios.FirstOrDefaultAsync(s => s.Id == studioId);
            if (studio == null)
                throw new NotFoundException("Студия не найдена");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            bool isLeader = await _context.StudiosUsers
                .AnyAsync(su => su.StudentId == currentUserId && su.StudioId == studioId && su.IsLeader);

            if (!isLeader)
                throw new ForbiddenException("Нет прав для просмотра студентов студии");

            var studentIds = await _context.StudiosUsers
                .Where(su => su.StudioId == studioId && !su.IsLeader)
                .Select(su => su.StudentId)
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
                GroupName = u.GroupName
            }).ToList();

            return userDtos;
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
                GroupName = u.GroupName
            }).ToList();

            return userDtos;
        }


        public async Task<UserResponseDTO> UpdateUserAsync(Guid userId, UpdateUserRequestDTO dto, ClaimsPrincipal currentUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            var currentUserId = Guid.Parse(currentUser.FindFirst("userId")?.Value!);
            if (currentUserId != userId)
                throw new ForbiddenException("Обновлять профиль может только владелец или админ");

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

            return new UserResponseDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
            };
        }

        public async Task DeleteUserAsync(Guid userId, Guid currentUserId)
        {

            if (userId != currentUserId)
                throw new ForbiddenException("Нельзя удалить другого пользователя");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("Пользователь не найден");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}