using OfficerService.DTOs;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IApplyTpNocRepository
    {
        Task<List<ForestProduceDto>> GetForestProducesByStateAsync(int stateId);
        Task<List<SpeciesDto>> GetSpeciesByStateAndForestProduceAsync(int stateId, int forestProduceId);
        Task<List<SpeciesMappingsDto>> GetSpeciesMappingsByStateAsync(int stateId);
        Task<List<District>> GetDistrictsByState(int stateId);
        Task<List<SubDistrict>> GetSubDistrictsByDistrict(int districtId);
    }
}
