using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class RoleMenuService : IRoleMenuService
    {
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly IMasterMenuRepository _menuRepository;
        private readonly IMasterRoleRepository _roleRepository;
        private readonly AppDbContext _context;

        public RoleMenuService(
            IRoleMenuRepository roleMenuRepository,
            IMasterMenuRepository menuRepository,
            IMasterRoleRepository roleRepository,
            AppDbContext context)
        {
            _roleMenuRepository = roleMenuRepository;
            _menuRepository = menuRepository;
            _roleRepository = roleRepository;
            _context = context;
        }

        public async Task<IEnumerable<RoleMenuDto>> AssignMenusToRoleAsync(CreateRoleMenuDto createDto)
        {
            // Verify role exists
            var role = await _roleRepository.GetRoleByIdAsync(createDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {createDto.RoleId} not found.");
            }

            var result = new List<RoleMenuDto>();
            var displayOrder = 1;

            foreach (var menuId in createDto.MenuIds)
            {
                // Verify menu exists
                var menu = await _menuRepository.GetMenuByIdAsync(menuId);
                if (menu == null)
                {
                    continue; // Skip invalid menu IDs
                }

                // Use custom display order if provided
                var order = createDto.DisplayOrders?.ContainsKey(menuId) == true 
                    ? createDto.DisplayOrders[menuId] 
                    : displayOrder++;

                var roleMenu = await _roleMenuRepository.AssignMenuToRoleAsync(
                    createDto.RoleId,
                    menuId,
                    order,
                    createDto.CreatedBy);

                result.Add(MapToDto(roleMenu, role.RoleName, menu));
            }

            return result;
        }

        public async Task<IEnumerable<RoleMenuDto>> GetRoleMenusAsync(int roleId)
        {
            var roleMenus = await _roleMenuRepository.GetRoleMenusAsync(roleId);
            return roleMenus.Select(rm => MapToDto(rm, rm.Role.RoleName, rm.Menu));
        }

        public async Task<RoleMenuDto> UpdateRoleMenuAsync(UpdateRoleMenuDto updateDto)
        {
            var roleMenu = await _context.Set<RoleMenu>()
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .FirstOrDefaultAsync(rm => rm.RoleMenuId == updateDto.RoleMenuId);

            if (roleMenu == null)
            {
                throw new InvalidOperationException($"Role-Menu assignment with ID {updateDto.RoleMenuId} not found.");
            }

            roleMenu.DisplayOrder = updateDto.DisplayOrder;
            roleMenu.IsVisible = updateDto.IsVisible;
            roleMenu.UpdatedDate = DateTime.UtcNow;
            roleMenu.UpdatedBy = updateDto.UpdatedBy;

            var updatedRoleMenu = await _roleMenuRepository.UpdateRoleMenuAsync(roleMenu);
            return MapToDto(updatedRoleMenu, updatedRoleMenu.Role.RoleName, updatedRoleMenu.Menu);
        }

        public async Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId)
        {
            return await _roleMenuRepository.RemoveMenuFromRoleAsync(roleId, menuId);
        }

        public async Task<bool> ReorderRoleMenusAsync(int roleId, ReorderMenusDto reorderDto)
        {
            foreach (var menuOrder in reorderDto.MenuOrders)
            {
                var roleMenu = await _roleMenuRepository.GetRoleMenuAsync(roleId, menuOrder.MenuId);
                if (roleMenu != null)
                {
                    roleMenu.DisplayOrder = menuOrder.DisplayOrder;
                    roleMenu.UpdatedDate = DateTime.UtcNow;
                    await _roleMenuRepository.UpdateRoleMenuAsync(roleMenu);
                }
            }

            return true;
        }

        public async Task<IEnumerable<UserMenuItemDto>> GetUserMenusAsync(string loginId)
        {
            // Get user's role
            var user = await _context.OfficerRegistrations
                .FirstOrDefaultAsync(or => or.LoginId == loginId);

            if (user == null)
            {
                return new List<UserMenuItemDto>();
            }

            // Get menus for user's role
            var roleMenus = await _roleMenuRepository.GetUserMenusAsync(user.RoleId);

            return roleMenus.Select(rm => new UserMenuItemDto
            {
                Label = rm.Menu.Label,
                Icon = rm.Menu.Icon,
                RouterLink = rm.Menu.RouterLink,
                Url = rm.Menu.Url,
                Target = rm.Menu.Target,
                DisplayOrder = rm.DisplayOrder,
                MenuId = rm.Menu.MenuId,
                ParentMenuId = rm.Menu.ParentMenuId,
                IsVisible = rm.IsVisible
            }).OrderBy(m => m.DisplayOrder);
        }

        public async Task<IEnumerable<UserMenuItemDto>> GetUserMenuTreeAsync(string loginId)
        {
            var flatMenus = await GetUserMenusAsync(loginId);
            return BuildMenuTree(flatMenus.ToList());
        }

        private List<UserMenuItemDto> BuildMenuTree(List<UserMenuItemDto> flatMenus)
        {
            // Get root menus (menus without parent)
            var rootMenus = flatMenus
                .Where(m => !m.ParentMenuId.HasValue && m.IsVisible)
                .OrderBy(m => m.DisplayOrder)
                .ToList();

            // Build tree recursively
            foreach (var rootMenu in rootMenus)
            {
                rootMenu.Items = GetChildMenus(rootMenu.MenuId, flatMenus);
            }

            return rootMenus;
        }

        private List<UserMenuItemDto> GetChildMenus(int parentMenuId, List<UserMenuItemDto> allMenus)
        {
            var childMenus = allMenus
                .Where(m => m.ParentMenuId == parentMenuId && m.IsVisible)
                .OrderBy(m => m.DisplayOrder)
                .ToList();

            foreach (var childMenu in childMenus)
            {
                childMenu.Items = GetChildMenus(childMenu.MenuId, allMenus);
            }

            return childMenus.Any() ? childMenus : null;
        }

        private static RoleMenuDto MapToDto(RoleMenu roleMenu, string roleName, MasterMenu menu)
        {
            return new RoleMenuDto
            {
                RoleMenuId = roleMenu.RoleMenuId,
                RoleId = roleMenu.RoleId,
                RoleName = roleName,
                MenuId = roleMenu.MenuId,
                MenuName = menu.MenuName,
                MenuLabel = menu.Label,
                MenuIcon = menu.Icon,
                RouterLink = menu.RouterLink,
                DisplayOrder = roleMenu.DisplayOrder,
                IsVisible = roleMenu.IsVisible,
                CreatedDate = roleMenu.CreatedDate,
                CreatedBy = roleMenu.CreatedBy,
                UpdatedDate = roleMenu.UpdatedDate,
                UpdatedBy = roleMenu.UpdatedBy,
                IsActive = roleMenu.IsActive
            };
        }
    }
}
