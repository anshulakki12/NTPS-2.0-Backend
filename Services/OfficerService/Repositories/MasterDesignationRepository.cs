using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class MasterDesignationRepository : IMasterDesignationRepository
    {
        private readonly AppDbContext _context;

        public MasterDesignationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterDesignation> CreateDesignationAsync(CreateDesignationDto createDesignationDto)
        {
            // Check if designation already exists
            var existingDesignation = await GetDesignationByNameAsync(createDesignationDto.DesignationName);
            if (existingDesignation != null)
            {
                throw new InvalidOperationException($"Designation '{createDesignationDto.DesignationName}' already exists.");
            }

            var designation = new MasterDesignation
            {
                DesignationName = createDesignationDto.DesignationName,
                Rank = createDesignationDto.Rank,
                Abbreviation = createDesignationDto.Abbreviation,
                IsActive = createDesignationDto.IsActive,
                CreatedBy = createDesignationDto.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.MasterDesignations.Add(designation);
            await _context.SaveChangesAsync();

            return designation;
        }

        public async Task<MasterDesignation?> GetDesignationByIdAsync(int designationId)
        {
            return await _context.MasterDesignations
                .FirstOrDefaultAsync(d => d.DesignationId == designationId);
        }

        public async Task<MasterDesignation?> GetDesignationByNameAsync(string designationName)
        {
            return await _context.MasterDesignations
                .FirstOrDefaultAsync(d => d.DesignationName.ToLower() == designationName.ToLower());
        }

        public async Task<IEnumerable<MasterDesignation>> GetAllDesignationsAsync()
        {
            return await _context.MasterDesignations
                .OrderBy(d => d.Rank)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterDesignation>> GetActiveDesignationsAsync()
        {
            return await _context.MasterDesignations
                .Where(d => d.IsActive)
                .OrderBy(d => d.Rank)
                .ToListAsync();
        }

        public async Task<MasterDesignation> UpdateDesignationAsync(MasterDesignation designation)
        {
            _context.MasterDesignations.Update(designation);
            await _context.SaveChangesAsync();
            return designation;
        }

        public async Task<bool> DeleteDesignationAsync(int designationId)
        {
            var designation = await GetDesignationByIdAsync(designationId);
            if (designation == null)
                return false;

            // Check if designation is being used by any officers
            var isDesignationInUse = await _context.OfficerRegistrations
                .AnyAsync(o => o.DesignationId == designationId);

            if (isDesignationInUse)
            {
                throw new InvalidOperationException("Cannot delete designation as it is assigned to one or more officers.");
            }

            _context.MasterDesignations.Remove(designation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesignationExistsAsync(string designationName)
        {
            return await _context.MasterDesignations
                .AnyAsync(d => d.DesignationName.ToLower() == designationName.ToLower());
        }
    }
}
