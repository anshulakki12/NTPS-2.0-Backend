using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IMasterDesignationRepository
    {
        Task<MasterDesignation> CreateDesignationAsync(CreateDesignationDto createDesignationDto);
        Task<MasterDesignation?> GetDesignationByIdAsync(int designationId);
        Task<MasterDesignation?> GetDesignationByNameAsync(string designationName);
        Task<IEnumerable<MasterDesignation>> GetAllDesignationsAsync();
        Task<IEnumerable<MasterDesignation>> GetActiveDesignationsAsync();
        Task<MasterDesignation> UpdateDesignationAsync(MasterDesignation designation);
        Task<bool> DeleteDesignationAsync(int designationId);
        Task<bool> DesignationExistsAsync(string designationName);
    }
}
