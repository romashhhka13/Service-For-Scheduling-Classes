using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;
using ScheduleMaster.Helpers;

namespace ScheduleMaster.Services
{
    public class UserService
    {
        private readonly ScheduleMasterDbContext _context;

        public UserService(ScheduleMasterDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users.Select(
                user => new UserDTO
                {
                    Id = user.Id,
                    Surname = user.Surname,
                    Name = user.Name,
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    Role = user.Role,
                    Faculty = user.Faculty,
                    GroupName = user.GroupName,
                    Course = user.Course
                }).ToListAsync();
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
                Course = user.Course
            };
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createDTO)
        {
            // Проверка email
            var userExist = await _context.Users.AnyAsync(user => user.Email == createDTO.Email);
            if (userExist)
                throw new Exception($"Пользователь с email '{createDTO.Email}' уже существует");

            var hashedPassword = PasswordHasher.Generate(createDTO.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Surname = createDTO.Surname,
                Name = createDTO.Name,
                MiddleName = createDTO.MiddleName,
                Email = createDTO.Email,
                PasswordHash = hashedPassword,
                Role = createDTO.Role,
                Faculty = createDTO.Faculty,
                GroupName = createDTO.GroupName,
                Course = createDTO.Course
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
                Course = user.Course
            };
        }

        public async Task<UserDTO?> UpdateUserAsync(Guid id, UpdateUserDTO updateDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(updateDTO.Surname))
                user.Surname = updateDTO.Surname;

            if (!string.IsNullOrEmpty(updateDTO.Name))
                user.Name = updateDTO.Name;

            if (!string.IsNullOrEmpty(updateDTO.MiddleName))
                user.MiddleName = updateDTO.MiddleName;

            if (!string.IsNullOrEmpty(updateDTO.Email))
                user.Email = updateDTO.Email;

            if (!string.IsNullOrEmpty(updateDTO.Role))
                user.Role = updateDTO.Role;

            if (!string.IsNullOrEmpty(updateDTO.Faculty))
                user.Faculty = updateDTO.Faculty;

            if (!string.IsNullOrEmpty(updateDTO.GroupName))
                user.GroupName = updateDTO.GroupName;

            if (updateDTO.Course.HasValue)
                user.Course = updateDTO.Course;

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                Role = user.Role,
                Faculty = user.Faculty,
                GroupName = user.GroupName,
                Course = user.Course
            };
        }

        // public async Task<bool> UpdatePasswordAsync(Guid id, UpdatePasswordDTO updatePasswordDTO)
        // {

        // }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // UserService.cs
        public async Task<List<StudentScheduleDTO>> GetStudentScheduleAsync(Guid studentId)
        {
            var userExist = await _context.Users.AnyAsync(user => user.Id == studentId);
            if (!userExist)
                throw new Exception($"Пользователя не существует");

            return await (from gm in _context.GroupMemberships
                          join g in _context.Groups on gm.GroupId equals g.Id
                          join schedule in _context.Schedules on g.Id equals schedule.GroupId
                          join studio in _context.Studios on schedule.StudioId equals studio.Id
                          where gm.StudentId == studentId
                          select new StudentScheduleDTO
                          {
                              ScheduleId = schedule.Id,
                              StudioName = studio.Name,
                              GroupName = g.Name,
                              StartDateTime = schedule.StartDateTime,
                              EndDateTime = schedule.EndDateTime,
                              Location = schedule.Location,
                              WeekType = schedule.WeekType
                          })
                          .OrderBy(s => s.StartDateTime)
                          .ToListAsync();
        }


    }
}