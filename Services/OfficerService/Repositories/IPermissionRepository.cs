using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> CreatePermissionAsync(CreatePermissionDto createDto);
        Task<Permission?> GetPermissionByIdAsync(int permissionId);
        Task<Permission?> GetPermissionByCodeAsync(string permissionCode);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<IEnumerable<Permission>> GetActivePermissionsAsync();
        Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module);
        Task<Permission> UpdatePermissionAsync(Permission permission);
        Task<bool> DeletePermissionAsync(int permissionId);
    }
}
