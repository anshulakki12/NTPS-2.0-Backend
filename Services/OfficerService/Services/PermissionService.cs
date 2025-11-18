using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createDto)
        {
            var permission = await _permissionRepository.CreatePermissionAsync(createDto);
            return MapToDto(permission);
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(int permissionId)
        {
            var permission = await _permissionRepository.GetPermissionByIdAsync(permissionId);
            return permission != null ? MapToDto(permission) : null;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllPermissionsAsync();
            return permissions.Select(MapToDto);
        }

        public async Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync()
        {
            var permissions = await _permissionRepository.GetActivePermissionsAsync();
            return permissions.Select(MapToDto);
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module)
        {
            var permissions = await _permissionRepository.GetPermissionsByModuleAsync(module);
            return permissions.Select(MapToDto);
        }

        public async Task<PermissionDto> UpdatePermissionAsync(int permissionId, UpdatePermissionDto updateDto)
        {
            var permission = await _permissionRepository.GetPermissionByIdAsync(permissionId);
            if (permission == null)
            {
                throw new InvalidOperationException($"Permission with ID {permissionId} not found.");
            }

            permission.PermissionName = updateDto.PermissionName;
            permission.Description = updateDto.Description;
            permission.IsActive = updateDto.IsActive;
            permission.UpdatedDate = DateTime.UtcNow;
            permission.UpdatedBy = updateDto.UpdatedBy;

            var updatedPermission = await _permissionRepository.UpdatePermissionAsync(permission);
            return MapToDto(updatedPermission);
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            return await _permissionRepository.DeletePermissionAsync(permissionId);
        }

        private static PermissionDto MapToDto(Permission permission)
        {
            return new PermissionDto
            {
                PermissionId = permission.PermissionId,
                PermissionCode = permission.PermissionCode,
                PermissionName = permission.PermissionName,
                Description = permission.Description,
                Module = permission.Module,
                IsActive = permission.IsActive,
                CreatedDate = permission.CreatedDate,
                CreatedBy = permission.CreatedBy,
                UpdatedDate = permission.UpdatedDate,
                UpdatedBy = permission.UpdatedBy
            };
        }
    }
}
