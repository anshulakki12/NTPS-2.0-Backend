using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterModuleService : IMasterModuleService
    {
        private readonly IMasterModuleRepository _moduleRepository;

        public MasterModuleService(IMasterModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createDto)
        {
            var module = await _moduleRepository.CreateModuleAsync(createDto);
            return MapToDto(module);
        }

        public async Task<ModuleDto?> GetModuleByIdAsync(int moduleId)
        {
            var module = await _moduleRepository.GetModuleByIdAsync(moduleId);
            return module != null ? MapToDto(module) : null;
        }

        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            var modules = await _moduleRepository.GetAllModulesAsync();
            return modules.Select(MapToDto);
        }

        public async Task<IEnumerable<ModuleDto>> GetActiveModulesAsync()
        {
            var modules = await _moduleRepository.GetActiveModulesAsync();
            return modules.Select(MapToDto);
        }

        public async Task<ModuleDto> UpdateModuleAsync(int moduleId, UpdateModuleDto updateDto)
        {
            var existingModule = await _moduleRepository.GetModuleByIdAsync(moduleId);
            if (existingModule == null)
            {
                throw new InvalidOperationException($"Module with ID {moduleId} not found.");
            }

            // Check if another module with the same code exists (excluding current module)
            if (existingModule.ModuleCode != updateDto.ModuleCode)
            {
                var duplicateModule = await _moduleRepository.GetModuleByCodeAsync(updateDto.ModuleCode);
                if (duplicateModule != null && duplicateModule.ModuleId != moduleId)
                {
                    throw new InvalidOperationException($"Module with code '{updateDto.ModuleCode}' already exists.");
                }
            }

            // Track activation/deactivation status changes
            var wasActive = existingModule.IsActive;
            var willBeActive = updateDto.IsActive;

            // Update properties
            existingModule.ModuleName = updateDto.ModuleName;
            existingModule.ModuleCode = updateDto.ModuleCode;
            existingModule.Description = updateDto.Description;
            existingModule.Icon = updateDto.Icon;
            existingModule.RoutePath = updateDto.RoutePath;
            existingModule.DisplayOrder = updateDto.DisplayOrder;
            existingModule.IsActive = updateDto.IsActive;
            existingModule.ModifiedDate = DateTime.UtcNow;
            existingModule.ModifiedBy = updateDto.ModifiedBy;

            // Track deactivation/reactivation
            if (wasActive && !willBeActive)
            {
                existingModule.DeactivatedOn = DateTime.UtcNow;
            }
            else if (!wasActive && willBeActive)
            {
                existingModule.ReactivatedOn = DateTime.UtcNow;
            }

            var updatedModule = await _moduleRepository.UpdateModuleAsync(existingModule);
            return MapToDto(updatedModule);
        }

        public async Task<ModuleDto> ActivateModuleAsync(int moduleId, string activatedBy)
        {
            var existingModule = await _moduleRepository.GetModuleByIdAsync(moduleId);
            if (existingModule == null)
            {
                throw new InvalidOperationException($"Module with ID {moduleId} not found.");
            }

            if (existingModule.IsActive)
            {
                throw new InvalidOperationException($"Module with ID {moduleId} is already active.");
            }

            existingModule.IsActive = true;
            existingModule.ReactivatedOn = DateTime.UtcNow;
            existingModule.ModifiedDate = DateTime.UtcNow;
            existingModule.ModifiedBy = activatedBy;

            var updatedModule = await _moduleRepository.UpdateModuleAsync(existingModule);
            return MapToDto(updatedModule);
        }

        public async Task<ModuleDto> DeactivateModuleAsync(int moduleId, string deactivatedBy)
        {
            var existingModule = await _moduleRepository.GetModuleByIdAsync(moduleId);
            if (existingModule == null)
            {
                throw new InvalidOperationException($"Module with ID {moduleId} not found.");
            }

            if (!existingModule.IsActive)
            {
                throw new InvalidOperationException($"Module with ID {moduleId} is already deactivated.");
            }

            existingModule.IsActive = false;
            existingModule.DeactivatedOn = DateTime.UtcNow;
            existingModule.ModifiedDate = DateTime.UtcNow;
            existingModule.ModifiedBy = deactivatedBy;

            var updatedModule = await _moduleRepository.UpdateModuleAsync(existingModule);
            return MapToDto(updatedModule);
        }

        public async Task<bool> DeleteModuleAsync(int moduleId)
        {
            return await _moduleRepository.DeleteModuleAsync(moduleId);
        }

        private static ModuleDto MapToDto(MasterModule module)
        {
            return new ModuleDto
            {
                ModuleId = module.ModuleId,
                ModuleName = module.ModuleName,
                ModuleCode = module.ModuleCode,
                Description = module.Description,
                Icon = module.Icon,
                RoutePath = module.RoutePath,
                DisplayOrder = module.DisplayOrder,
                IsActive = module.IsActive,
                CreatedDate = module.CreatedDate,
                CreatedBy = module.CreatedBy,
                ModifiedDate = module.ModifiedDate,
                ModifiedBy = module.ModifiedBy,
                DeactivatedOn = module.DeactivatedOn,
                ReactivatedOn = module.ReactivatedOn
            };
        }
    }
}
