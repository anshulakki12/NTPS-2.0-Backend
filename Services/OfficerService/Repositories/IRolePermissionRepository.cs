using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<RolePermission> AssignPermissionAsync(int roleId, int permissionId, string? assignedBy);
        Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(int roleId);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
        Task<bool> RemovePermissionAsync(int roleId, int permissionId);
        Task<bool> RemoveAllRolePermissionsAsync(int roleId);
        Task<bool> HasPermissionAsync(int roleId, string permissionCode);
        Task<IEnumerable<string>> GetRolePermissionCodesAsync(int roleId);
    }
}
