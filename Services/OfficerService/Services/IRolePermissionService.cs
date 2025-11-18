using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionDto>> AssignPermissionsToRoleAsync(CreateRolePermissionDto createDto);
        Task<IEnumerable<RolePermissionDto>> UpdateRolePermissionsAsync(UpdateRolePermissionDto updateDto);
        Task<IEnumerable<RolePermissionDto>> GetRolePermissionsAsync(int roleId);
        Task<UserPermissionDto?> GetUserPermissionsAsync(string loginId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<bool> HasPermissionAsync(int roleId, string permissionCode);
    }
}
