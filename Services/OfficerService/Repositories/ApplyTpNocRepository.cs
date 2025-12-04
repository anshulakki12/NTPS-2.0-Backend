using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class ApplyTpNocRepository : IApplyTpNocRepository
    {
        private readonly AppDbContext _context;

        public ApplyTpNocRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ForestProduceDto>> GetForestProducesByStateAsync(int stateCode)
        {
            var forestProduces = await _context.SpeciesMapping
                .Where(sm => sm.IsActive && sm.StateId == stateCode)
                .Include(sm => sm.ForestProduce)
                .Select(sm => new ForestProduceDto
                {
                    ForestProduceId = sm.ForestProduce.ForestProduceId,
                    Name = sm.ForestProduce.Name,
                    QuantityType = sm.ForestProduce.QuantityType,
                    WeightType = sm.ForestProduce.WeightType,
                    Category = sm.ForestProduce.Category,
                    RequiresLog = sm.ForestProduce.RequiresLog
                })
                .Distinct()
                .ToListAsync();

            return forestProduces;
        }


        public async Task<List<SpeciesDto>> GetSpeciesByStateAndForestProduceAsync(int stateId, int forestProduceId)
        {
            var species = await _context.SpeciesMapping
                .Where(sm => sm.StateId == stateId &&
                           sm.ForestProduceId == forestProduceId &&
                           sm.IsActive)
                .Include(sm => sm.MasterSpecies)
                .Select(sm => new SpeciesDto
                {
                    SpeciesID = sm.MasterSpecies.SpeciesID,
                    Name = sm.MasterSpecies.Name,
                    CategoryId=sm.CategoryId
                })
                .Distinct()
                .ToListAsync();

            return species;
        }

        public async Task<List<SpeciesMappingsDto>> GetSpeciesMappingsByStateAsync(int stateId)
        {
            var mappings = await _context.SpeciesMapping
                .Where(sm => sm.StateId == stateId && sm.IsActive)
                .Include(sm => sm.ForestProduce)
                .Include(sm => sm.MasterSpecies)
                .Select(sm => new SpeciesMappingsDto
                {
                    ForestProduceId = sm.ForestProduceId,
                    ForestProduceName = sm.ForestProduce.Name,
                    SpeciesId = sm.SpeciesId,
                    SpeciesName = sm.MasterSpecies.Name
                })
                .ToListAsync();

            return mappings;
        }

        public async Task<List<District>> GetDistrictsByState(int stateCode)
        {
            return await _context.Districts
                .Where(d => d.StCode == stateCode)
                .OrderBy(d => d.DistName)
                .ToListAsync();
        }

        public async Task<List<SubDistrict>> GetSubDistrictsByDistrict(int districtId)
        {
            return await _context.SubDistrict
                .Where(sd => sd.DistCode == districtId)
                .OrderBy(sd => sd.SubDistName)
                .ToListAsync();
        }

    }
}
