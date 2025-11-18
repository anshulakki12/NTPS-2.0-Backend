using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface ISpeciesMappingRepository
    {
        // Species Mapping methods
        Task<IEnumerable<SpeciesMapping>> GetAllSpeciesMappingsAsync();
        Task<SpeciesMapping> GetSpeciesMappingByIdAsync(int speciesMappingId);
        Task<SpeciesMapping> CreateSpeciesMappingAsync(SpeciesMapping speciesMapping);
        Task<SpeciesMapping> UpdateSpeciesMappingAsync(SpeciesMapping speciesMapping);
        Task<bool> DeleteSpeciesMappingAsync(int speciesMappingId);
        Task<bool> ToggleSpeciesMappingStatusAsync(int speciesMappingId, bool isActive);

        // Related data methods
        Task<IEnumerable<ForestProduce>> GetAllForestProducesAsync();
        Task<IEnumerable<MasterSpecies>> GetAllSpeciesAsync();
        Task<IEnumerable<MasterZone>> GetAllZonesAsync();
        Task<IEnumerable<MasterSpecies>> GetAvailableSpeciesForStateAndForestProduceAsync(int stateId, int forestProduceId);
    }
}
