using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
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

        public async Task<IEnumerable<StudioDTO>> GetAllStudiosAsync()
        {
            return await _context.Studios.Select(
                s => new StudioDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    AdministratorId = s.AdministratorId
                })
                .ToListAsync();
        }

        public async Task<StudioDTO?> GetStudioByIdAsync(Guid id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null) return null;

            return new StudioDTO
            {
                Id = studio.Id,
                Name = studio.Name,
                Category = studio.Category,
                AdministratorId = studio.AdministratorId
            };
        }

        public async Task<StudioDTO> CreateStudioAsync(CreateStudioDTO createDTO)
        {
            var studio = new Studio
            {
                Id = Guid.NewGuid(),
                Name = createDTO.Name,
                Category = createDTO.Category,
                AdministratorId = createDTO.AdministratorId
            };

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();

            return new StudioDTO
            {
                Id = studio.Id,
                Name = studio.Name,
                Category = studio.Category,
                AdministratorId = studio.AdministratorId
            };
        }

        public async Task<StudioDTO?> UpdateStudioAsync(Guid id, UpdateStudioDTO updateDTO)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null) return null;

            if (!string.IsNullOrEmpty(updateDTO.Name))
                studio.Name = updateDTO.Name;

            if (!string.IsNullOrEmpty(updateDTO.Category))
                studio.Category = updateDTO.Category;

            await _context.SaveChangesAsync();

            return new StudioDTO
            {
                Id = studio.Id,
                Name = studio.Name,
                Category = studio.Category,
                AdministratorId = studio.AdministratorId
            };
        }

        public async Task<bool> DeleteStudioAsync(Guid id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null) return false;

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
