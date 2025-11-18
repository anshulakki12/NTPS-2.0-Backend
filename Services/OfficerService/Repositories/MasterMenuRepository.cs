using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class MasterMenuRepository : IMasterMenuRepository
    {
        private readonly AppDbContext _context;

        public MasterMenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterMenu> CreateMenuAsync(CreateMenuDto createDto)
        {
            // Check if menu code already exists
            var existingMenu = await _context.Set<MasterMenu>()
                .FirstOrDefaultAsync(m => m.MenuCode == createDto.MenuCode);

            if (existingMenu != null)
            {
                throw new InvalidOperationException($"Menu with code '{createDto.MenuCode}' already exists.");
            }

            // Validate parent menu if specified
            if (createDto.ParentMenuId.HasValue)
            {
                var parentMenu = await GetMenuByIdAsync(createDto.ParentMenuId.Value);
                if (parentMenu == null)
                {
                    throw new InvalidOperationException($"Parent menu with ID {createDto.ParentMenuId} not found.");
                }
                if (!parentMenu.IsParent)
                {
                    throw new InvalidOperationException("Parent menu must have IsParent set to true.");
                }
            }

            var menu = new MasterMenu
            {
                MenuName = createDto.MenuName,
                MenuCode = createDto.MenuCode,
                RoleId = createDto.RoleId,
                ModuleId = createDto.ModuleId,
                ParentMenuId = createDto.ParentMenuId,
                Label = createDto.Label,
                Icon = createDto.Icon,
                RouterLink = createDto.RouterLink,
                Url = createDto.Url,
                Target = createDto.Target,
                IsParent = createDto.IsParent,
                Description = createDto.Description,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createDto.CreatedBy
            };

            _context.Set<MasterMenu>().Add(menu);
            await _context.SaveChangesAsync();

            return menu;
        }

        public async Task<MasterMenu?> GetMenuByIdAsync(int menuId)
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Include(m => m.ParentMenu)
                .FirstOrDefaultAsync(m => m.MenuId == menuId);
        }

        public async Task<MasterMenu?> GetMenuByCodeAsync(string menuCode)
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Include(m => m.ParentMenu)
                .FirstOrDefaultAsync(m => m.MenuCode == menuCode);
        }

        public async Task<IEnumerable<MasterMenu>> GetAllMenusAsync()
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Include(m => m.ParentMenu)
                .OrderBy(m => m.ParentMenuId.HasValue ? 1 : 0)
                .ThenBy(m => m.Label)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterMenu>> GetActiveMenusAsync()
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Include(m => m.ParentMenu)
                .Where(m => m.IsActive)
                .OrderBy(m => m.ParentMenuId.HasValue ? 1 : 0)
                .ThenBy(m => m.Label)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterMenu>> GetParentMenusAsync()
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Where(m => m.IsParent && m.IsActive && !m.ParentMenuId.HasValue)
                .OrderBy(m => m.Label)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterMenu>> GetChildMenusAsync(int parentMenuId)
        {
            return await _context.Set<MasterMenu>()
                .Include(m => m.Role)
                .Include(m => m.Module)
                .Where(m => m.ParentMenuId == parentMenuId && m.IsActive)
                .OrderBy(m => m.Label)
                .ToListAsync();
        }

        public async Task<MasterMenu> UpdateMenuAsync(MasterMenu menu)
        {
            menu.UpdatedDate = DateTime.UtcNow;
            _context.Set<MasterMenu>().Update(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            var menu = await GetMenuByIdAsync(menuId);
            if (menu == null)
            {
                return false;
            }

            // Check if menu has child menus
            if (await HasChildMenusAsync(menuId))
            {
                throw new InvalidOperationException("Cannot delete menu that has child menus. Delete child menus first.");
            }

            // Check if menu is assigned to any roles
            var hasRoleAssignments = await _context.Set<RoleMenu>()
                .AnyAsync(rm => rm.MenuId == menuId);

            if (hasRoleAssignments)
            {
                throw new InvalidOperationException("Cannot delete menu that is assigned to roles. Remove role assignments first.");
            }

            _context.Set<MasterMenu>().Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasChildMenusAsync(int menuId)
        {
            return await _context.Set<MasterMenu>()
                .AnyAsync(m => m.ParentMenuId == menuId);
        }
    }
}
