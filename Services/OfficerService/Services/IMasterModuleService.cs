using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IMasterModuleService
    {
        Task<ModuleDto> CreateModuleAsync(CreateModuleDto createDto);
        Task<ModuleDto?> GetModuleByIdAsync(int moduleId);
        Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
        Task<IEnumerable<ModuleDto>> GetActiveModulesAsync();
        Task<ModuleDto> UpdateModuleAsync(int moduleId, UpdateModuleDto updateDto);
        Task<ModuleDto> ActivateModuleAsync(int moduleId, string activatedBy);
        Task<ModuleDto> DeactivateModuleAsync(int moduleId, string deactivatedBy);
        Task<bool> DeleteModuleAsync(int moduleId);
    }
}
