using OfficerService.Models;
using OfficerService.DtoModels;

namespace OfficerService.Repositories
{
    public interface IMasterRoleRepository
    {
        Task<MasterRoles> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<MasterRoles?> GetRoleByIdAsync(int roleId);
        Task<MasterRoles?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<MasterRoles>> GetAllRolesAsync();
        Task<IEnumerable<MasterRoles>> GetActiveRolesAsync();
        Task<MasterRoles> UpdateRoleAsync(MasterRoles role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> RoleExistsAsync(string roleName);
    }
}