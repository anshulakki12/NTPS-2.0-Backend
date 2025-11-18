using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IRoleMenuService
    {
        Task<IEnumerable<RoleMenuDto>> AssignMenusToRoleAsync(CreateRoleMenuDto createDto);
        Task<IEnumerable<RoleMenuDto>> GetRoleMenusAsync(int roleId);
        Task<RoleMenuDto> UpdateRoleMenuAsync(UpdateRoleMenuDto updateDto);
        Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId);
        Task<bool> ReorderRoleMenusAsync(int roleId, ReorderMenusDto reorderDto);
        Task<IEnumerable<UserMenuItemDto>> GetUserMenusAsync(string loginId);
        Task<IEnumerable<UserMenuItemDto>> GetUserMenuTreeAsync(string loginId);
    }
}
