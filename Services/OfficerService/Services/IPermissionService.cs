using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IPermissionService
    {
        Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createDto);
        Task<PermissionDto?> GetPermissionByIdAsync(int permissionId);
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module);
        Task<PermissionDto> UpdatePermissionAsync(int permissionId, UpdatePermissionDto updateDto);
        Task<bool> DeletePermissionAsync(int permissionId);
    }
}
