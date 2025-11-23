using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;

namespace ScheduleMaster.Services
{
    public class GroupService
    {
        // private readonly ScheduleMasterDbContext _context;

        // public GroupService(ScheduleMasterDbContext context)
        // {
        //     _context = context;
        // }

        // public async Task<List<GroupDTO>> GetAllGroupsAsync()
        // {
        //     return await _context.Groups.Select(
        //         group => new GroupDTO
        //         {
        //             Id = group.Id,
        //             StudioId = group.StudioId,
        //             Name = group.Name
        //         })
        //         .ToListAsync();
        // }

        // public async Task<GroupDTO?> GetGroupByIdAsync(Guid id)
        // {
        //     var group = await _context.Groups.FindAsync(id);
        //     if (group == null) return null;

        //     return new GroupDTO
        //     {
        //         Id = group.Id,
        //         StudioId = group.StudioId,
        //         Name = group.Name
        //     };
        // }

        // public async Task<GroupDTO> CreateGroupAsync(CreateGroupDTO createDTO)
        // {
        //     var studioExists = await _context.Studios.AnyAsync(s => s.Id == createDTO.StudioId);
        //     if (!studioExists)
        //         throw new Exception($"Студия с ID '{createDTO.StudioId}' не найдена");

        //     var group = new Group
        //     {
        //         Id = Guid.NewGuid(),
        //         StudioId = createDTO.StudioId,
        //         Name = createDTO.Name
        //     };

        //     _context.Groups.Add(group);
        //     await _context.SaveChangesAsync();

        //     return new GroupDTO
        //     {
        //         Id = group.Id,
        //         StudioId = group.StudioId,
        //         Name = group.Name
        //     };
        // }

        // public async Task<GroupDTO?> UpdateGroupAsync(Guid id, UpdateGroupDTO updateDTO)
        // {
        //     var group = await _context.Groups.FindAsync(id);
        //     if (group == null) return null;

        //     if (!string.IsNullOrEmpty(updateDTO.Name))
        //         group.Name = updateDTO.Name;

        //     await _context.SaveChangesAsync();

        //     return new GroupDTO
        //     {
        //         Id = group.Id,
        //         StudioId = group.StudioId,
        //         Name = group.Name
        //     };
        // }

        // public async Task<bool> DeleteGroupAsync(Guid id)
        // {
        //     var group = await _context.Groups.FindAsync(id);
        //     if (group == null) return false;

        //     _context.Groups.Remove(group);
        //     await _context.SaveChangesAsync();
        //     return true;
        // }

        // public async Task<List<GroupWithDetailsDTO>> GetGroupsWithDetailsAsync()
        // {
        //     return await (from g in _context.Groups
        //                   join studio in _context.Studios on g.StudioId equals studio.Id
        //                   select new GroupWithDetailsDTO
        //                   {
        //                       GroupId = g.Id,
        //                       GroupName = g.Name,
        //                       StudioName = studio.Name,
        //                       MemberCount = _context.GroupMemberships.Count(gm => gm.GroupId == g.Id),
        //                       ScheduleCount = _context.Schedules.Count(s => s.GroupId == g.Id)
        //                   })
        //                   .ToListAsync();
        // }
    }
}