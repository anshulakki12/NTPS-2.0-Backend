using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterMenuService : IMasterMenuService
    {
        private readonly IMasterMenuRepository _menuRepository;

        public MasterMenuService(IMasterMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDto> CreateMenuAsync(CreateMenuDto createDto)
        {
            var menu = await _menuRepository.CreateMenuAsync(createDto);
            return await MapToDtoAsync(menu);
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int menuId)
        {
            var menu = await _menuRepository.GetMenuByIdAsync(menuId);
            return menu != null ? await MapToDtoAsync(menu) : null;
        }

        public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
        {
            var menus = await _menuRepository.GetAllMenusAsync();
            var menuDtos = new List<MenuDto>();
            
            foreach (var menu in menus)
            {
                menuDtos.Add(await MapToDtoAsync(menu));
            }
            
            return menuDtos;
        }

        public async Task<IEnumerable<MenuDto>> GetActiveMenusAsync()
        {
            var menus = await _menuRepository.GetActiveMenusAsync();
            var menuDtos = new List<MenuDto>();
            
            foreach (var menu in menus)
            {
                menuDtos.Add(await MapToDtoAsync(menu));
            }
            
            return menuDtos;
        }

        public async Task<IEnumerable<MenuDto>> GetParentMenusAsync()
        {
            var menus = await _menuRepository.GetParentMenusAsync();
            var menuDtos = new List<MenuDto>();
            
            foreach (var menu in menus)
            {
                menuDtos.Add(await MapToDtoAsync(menu));
            }
            
            return menuDtos;
        }

        public async Task<IEnumerable<MenuDto>> GetChildMenusAsync(int parentMenuId)
        {
            var menus = await _menuRepository.GetChildMenusAsync(parentMenuId);
            var menuDtos = new List<MenuDto>();
            
            foreach (var menu in menus)
            {
                menuDtos.Add(await MapToDtoAsync(menu));
            }
            
            return menuDtos;
        }

        public async Task<MenuDto> UpdateMenuAsync(int menuId, UpdateMenuDto updateDto)
        {
            var existingMenu = await _menuRepository.GetMenuByIdAsync(menuId);
            if (existingMenu == null)
            {
                throw new InvalidOperationException($"Menu with ID {menuId} not found.");
            }

            // Check if another menu with the same code exists
            if (existingMenu.MenuCode != updateDto.MenuCode)
            {
                var duplicateMenu = await _menuRepository.GetMenuByCodeAsync(updateDto.MenuCode);
                if (duplicateMenu != null && duplicateMenu.MenuId != menuId)
                {
                    throw new InvalidOperationException($"Menu with code '{updateDto.MenuCode}' already exists.");
                }
            }

            // Update properties
            existingMenu.MenuName = updateDto.MenuName;
            existingMenu.MenuCode = updateDto.MenuCode;
            existingMenu.RoleId = updateDto.RoleId;
            existingMenu.ModuleId = updateDto.ModuleId;
            existingMenu.ParentMenuId = updateDto.ParentMenuId;
            existingMenu.Label = updateDto.Label;
            existingMenu.Icon = updateDto.Icon;
            existingMenu.RouterLink = updateDto.RouterLink;
            existingMenu.Url = updateDto.Url;
            existingMenu.Target = updateDto.Target;
            existingMenu.IsParent = updateDto.IsParent;
            existingMenu.Description = updateDto.Description;
            existingMenu.IsActive = updateDto.IsActive;
            existingMenu.UpdatedDate = DateTime.UtcNow;
            existingMenu.UpdatedBy = updateDto.UpdatedBy;

            var updatedMenu = await _menuRepository.UpdateMenuAsync(existingMenu);
            return await MapToDtoAsync(updatedMenu);
        }

        public async Task<MenuDto> ActivateMenuAsync(int menuId, string activatedBy)
        {
            var existingMenu = await _menuRepository.GetMenuByIdAsync(menuId);
            if (existingMenu == null)
            {
                throw new InvalidOperationException($"Menu with ID {menuId} not found.");
            }

            if (existingMenu.IsActive)
            {
                throw new InvalidOperationException($"Menu with ID {menuId} is already active.");
            }

            existingMenu.IsActive = true;
            existingMenu.UpdatedDate = DateTime.UtcNow;
            existingMenu.UpdatedBy = activatedBy;

            var updatedMenu = await _menuRepository.UpdateMenuAsync(existingMenu);
            return await MapToDtoAsync(updatedMenu);
        }

        public async Task<MenuDto> DeactivateMenuAsync(int menuId, string deactivatedBy)
        {
            var existingMenu = await _menuRepository.GetMenuByIdAsync(menuId);
            if (existingMenu == null)
            {
                throw new InvalidOperationException($"Menu with ID {menuId} not found.");
            }

            if (!existingMenu.IsActive)
            {
                throw new InvalidOperationException($"Menu with ID {menuId} is already deactivated.");
            }

            existingMenu.IsActive = false;
            existingMenu.UpdatedDate = DateTime.UtcNow;
            existingMenu.UpdatedBy = deactivatedBy;

            var updatedMenu = await _menuRepository.UpdateMenuAsync(existingMenu);
            return await MapToDtoAsync(updatedMenu);
        }

        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            return await _menuRepository.DeleteMenuAsync(menuId);
        }

        private async Task<MenuDto> MapToDtoAsync(MasterMenu menu)
        {
            var hasChildren = await _menuRepository.HasChildMenusAsync(menu.MenuId);
            
            return new MenuDto
            {
                MenuId = menu.MenuId,
                MenuName = menu.MenuName,
                MenuCode = menu.MenuCode,
                RoleId = menu.RoleId,
                RoleName = menu.Role?.RoleName,
                ModuleId = menu.ModuleId,
                ModuleName = menu.Module?.ModuleName,
                ModuleCode = menu.Module?.ModuleCode,
                ParentMenuId = menu.ParentMenuId,
                ParentMenuName = menu.ParentMenu?.MenuName,
                Label = menu.Label,
                Icon = menu.Icon,
                RouterLink = menu.RouterLink,
                Url = menu.Url,
                Target = menu.Target,
                IsParent = menu.IsParent,
                HasChildren = hasChildren,
                Description = menu.Description,
                CreatedDate = menu.CreatedDate,
                CreatedBy = menu.CreatedBy,
                UpdatedDate = menu.UpdatedDate,
                UpdatedBy = menu.UpdatedBy,
                IsActive = menu.IsActive
            };
        }
    }
}
