using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface ITransportModeRepository
    {
        Task<IEnumerable<MasterTransportMode>> GetAllAsync();
        Task<MasterTransportMode?> GetByIdAsync(int id);
        Task<MasterTransportMode> CreateAsync(MasterTransportMode transportMode);
        Task<MasterTransportMode> UpdateAsync(MasterTransportMode transportMode);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string transportMode, int? excludeId = null);
        Task<bool> ToggleStatusAsync(int id, bool isActive, string modifiedBy);
    }
}
