using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IRoleMenuRepository
    {
        Task<RoleMenu> AssignMenuToRoleAsync(int roleId, int menuId, int displayOrder, string? createdBy);
        Task<IEnumerable<RoleMenu>> GetRoleMenusAsync(int roleId);
        Task<RoleMenu?> GetRoleMenuAsync(int roleId, int menuId);
        Task<RoleMenu> UpdateRoleMenuAsync(RoleMenu roleMenu);
        Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId);
        Task<bool> RemoveAllRoleMenusAsync(int roleId);
        Task<IEnumerable<RoleMenu>> GetUserMenusAsync(int roleId);
    }
}
