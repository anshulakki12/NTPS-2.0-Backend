using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IMasterModuleRepository
    {
        Task<MasterModule> CreateModuleAsync(CreateModuleDto createDto);
        Task<MasterModule?> GetModuleByIdAsync(int moduleId);
        Task<MasterModule?> GetModuleByCodeAsync(string moduleCode);
        Task<IEnumerable<MasterModule>> GetAllModulesAsync();
        Task<IEnumerable<MasterModule>> GetActiveModulesAsync();
        Task<MasterModule> UpdateModuleAsync(MasterModule module);
        Task<bool> DeleteModuleAsync(int moduleId);
    }
}
