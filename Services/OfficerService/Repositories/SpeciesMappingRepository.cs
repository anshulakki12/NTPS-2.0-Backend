using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class SpeciesMappingRepository : ISpeciesMappingRepository
    {
        private readonly AppDbContext _context;

        public SpeciesMappingRepository(AppDbContext context)
        {
            _context = context;
        }

        // Species Mapping methods
        public async Task<IEnumerable<SpeciesMapping>> GetAllSpeciesMappingsAsync()
        {
            return await _context.SpeciesMapping
                .Include(sm => sm.ForestProduce)
                .Include(sm => sm.MasterSpecies)
                .Include(sm => sm.State)
                .Include(sm => sm.MasterWorkFlow)
                .Include(sm => sm.MasterZone)
                .Where(sm => sm.IsActive)
                .OrderBy(sm => sm.CreatedOn)
                .ToListAsync();
        }

        public async Task<SpeciesMapping> GetSpeciesMappingByIdAsync(int speciesMappingId)
        {
            return await _context.SpeciesMapping
                .Include(sm => sm.ForestProduce)
                .Include(sm => sm.MasterSpecies)
                .Include(sm => sm.State)
                .Include(sm => sm.MasterWorkFlow)
                .Include(sm => sm.MasterZone)
                .FirstOrDefaultAsync(sm => sm.SpeciesMappingId == speciesMappingId);
        }

        public async Task<SpeciesMapping> CreateSpeciesMappingAsync(SpeciesMapping speciesMapping)
        {
            // Check if mapping already exists
            var existingMapping = await _context.SpeciesMapping
                .FirstOrDefaultAsync(sm =>
                    sm.ForestProduceId == speciesMapping.ForestProduceId &&
                    sm.SpeciesId == speciesMapping.SpeciesId &&
                    sm.StateId == speciesMapping.StateId &&
                    sm.WorkFlowId == speciesMapping.WorkFlowId &&
                    sm.ZoneId == speciesMapping.ZoneId &&
                    sm.CategoryId == speciesMapping.CategoryId);

            if (existingMapping != null)
            {
                throw new InvalidOperationException("Species mapping already exists with the same combination.");
            }

            speciesMapping.CreatedOn = DateTime.Now;
            _context.SpeciesMapping.Add(speciesMapping);
            await _context.SaveChangesAsync();
            return speciesMapping;
        }

        public async Task<SpeciesMapping> UpdateSpeciesMappingAsync(SpeciesMapping speciesMapping)
        {
            // Check if mapping already exists (excluding current)
            var existingMapping = await _context.SpeciesMapping
                .FirstOrDefaultAsync(sm =>
                    sm.ForestProduceId == speciesMapping.ForestProduceId &&
                    sm.SpeciesId == speciesMapping.SpeciesId &&
                    sm.StateId == speciesMapping.StateId &&
                    sm.WorkFlowId == speciesMapping.WorkFlowId &&
                    sm.ZoneId == speciesMapping.ZoneId &&
                    sm.CategoryId == speciesMapping.CategoryId &&
                    sm.SpeciesMappingId != speciesMapping.SpeciesMappingId);

            if (existingMapping != null)
            {
                throw new InvalidOperationException("Species mapping already exists with the same combination.");
            }

            _context.SpeciesMapping.Update(speciesMapping);
            await _context.SaveChangesAsync();
            return speciesMapping;
        }

        public async Task<bool> DeleteSpeciesMappingAsync(int speciesMappingId)
        {
            var speciesMapping = await _context.SpeciesMapping.FindAsync(speciesMappingId);
            if (speciesMapping == null) return false;

            _context.SpeciesMapping.Remove(speciesMapping);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleSpeciesMappingStatusAsync(int speciesMappingId, bool isActive)
        {
            var speciesMapping = await _context.SpeciesMapping.FindAsync(speciesMappingId);
            if (speciesMapping == null) return false;

            speciesMapping.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // Related data methods
        public async Task<IEnumerable<ForestProduce>> GetAllForestProducesAsync()
        {
            return await _context.ForestProduce
                .Where(fp => fp.RequiresLog == "Y") // Only active ones or filter as needed
                .OrderBy(fp => fp.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterSpecies>> GetAllSpeciesAsync()
        {
            return await _context.MasterSpecies
                .OrderBy(ms => ms.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterZone>> GetAllZonesAsync()
        {
            return await _context.MasterZones
                .Include(mz => mz.State)
                .Where(mz => mz.IsActive)
                .OrderBy(mz => mz.ZoneName)
                .ToListAsync();
        }

        // Add this method to the repository
        //public async Task<IEnumerable<MasterSpecies>> GetAvailableSpeciesForStateAndForestProduceAsync(int stateId, int forestProduceId)
        //{
        //    // Get species already mapped to this state and forest produce
        //    var mappedSpeciesIds = await _context.SpeciesMapping
        //        .Where(sm => sm.StateId == stateId &&
        //                    sm.ForestProduceId == forestProduceId &&
        //                    sm.IsActive)
        //        .Select(sm => sm.SpeciesId)
        //        .ToListAsync();

        //    // Return species that are not mapped to this state and forest produce
        //    return await _context.MasterSpecies
        //        .Where(ms => !mappedSpeciesIds.Contains(ms.SpeciesID))
        //        .OrderBy(ms => ms.Name)
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<MasterSpecies>> GetAvailableSpeciesForStateAndForestProduceAsync(int stateId, int forestProduceId)
        {
            var query =
                from ms in _context.MasterSpecies
                join sm in _context.SpeciesMapping
                    on new { SpeciesId = ms.SpeciesID, StateId = stateId, ForestProduceId = forestProduceId, IsActive = true }
                    equals new { SpeciesId = sm.SpeciesId, StateId = sm.StateId, ForestProduceId = sm.ForestProduceId, IsActive = sm.IsActive }
                    into mapped
                from sm in mapped.DefaultIfEmpty() // LEFT JOIN
                where sm == null // Only species with no mapping
                orderby ms.Name
                select ms;

            return await query.ToListAsync();
        }

    }
}