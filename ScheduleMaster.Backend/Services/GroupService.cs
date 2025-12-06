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


    }
}