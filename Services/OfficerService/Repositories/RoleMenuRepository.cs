using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class RoleMenuRepository : IRoleMenuRepository
    {
        private readonly AppDbContext _context;

        public RoleMenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RoleMenu> AssignMenuToRoleAsync(int roleId, int menuId, int displayOrder, string? createdBy)
        {
            // Check if already assigned
            var existing = await _context.Set<RoleMenu>()
                .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);

            if (existing != null)
            {
                // Update existing assignment
                existing.DisplayOrder = displayOrder;
                existing.IsActive = true;
                existing.IsVisible = true;
                existing.UpdatedDate = DateTime.UtcNow;
                existing.UpdatedBy = createdBy;
                _context.Set<RoleMenu>().Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }

            var roleMenu = new RoleMenu
            {
                RoleId = roleId,
                MenuId = menuId,
                DisplayOrder = displayOrder,
                IsVisible = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            _context.Set<RoleMenu>().Add(roleMenu);
            await _context.SaveChangesAsync();

            return roleMenu;
        }

        public async Task<IEnumerable<RoleMenu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.Set<RoleMenu>()
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.Module)
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.ParentMenu)
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .OrderBy(rm => rm.DisplayOrder)
                .ToListAsync();
        }

        public async Task<RoleMenu?> GetRoleMenuAsync(int roleId, int menuId)
        {
            return await _context.Set<RoleMenu>()
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);
        }

        public async Task<RoleMenu> UpdateRoleMenuAsync(RoleMenu roleMenu)
        {
            roleMenu.UpdatedDate = DateTime.UtcNow;
            _context.Set<RoleMenu>().Update(roleMenu);
            await _context.SaveChangesAsync();
            return roleMenu;
        }

        public async Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId)
        {
            var roleMenu = await GetRoleMenuAsync(roleId, menuId);
            if (roleMenu == null)
            {
                return false;
            }

            _context.Set<RoleMenu>().Remove(roleMenu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAllRoleMenusAsync(int roleId)
        {
            var roleMenus = await _context.Set<RoleMenu>()
                .Where(rm => rm.RoleId == roleId)
                .ToListAsync();

            if (!roleMenus.Any())
            {
                return false;
            }

            _context.Set<RoleMenu>().RemoveRange(roleMenus);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoleMenu>> GetUserMenusAsync(int roleId)
        {
            return await _context.Set<RoleMenu>()
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.Module)
                .Include(rm => rm.Menu)
                    .ThenInclude(m => m.ParentMenu)
                .Where(rm => rm.RoleId == roleId && 
                            rm.IsActive && 
                            rm.IsVisible && 
                            rm.Menu.IsActive)
                .OrderBy(rm => rm.DisplayOrder)
                .ToListAsync();
        }
    }
}
