using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _context;

        public RolePermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RolePermission> AssignPermissionAsync(int roleId, int permissionId, string? assignedBy)
        {
            // Check if already assigned
            var existing = await _context.Set<RolePermission>()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (existing != null)
            {
                return existing;
            }

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                AssignedDate = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            _context.Set<RolePermission>().Add(rolePermission);
            await _context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Permission)
                .Include(rp => rp.Role)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            return await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => rp.Permission)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
        {
            var rolePermission = await _context.Set<RolePermission>()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null)
            {
                return false;
            }

            _context.Set<RolePermission>().Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAllRolePermissionsAsync(int roleId)
        {
            var rolePermissions = await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            if (!rolePermissions.Any())
            {
                return false;
            }

            _context.Set<RolePermission>().RemoveRange(rolePermissions);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasPermissionAsync(int roleId, string permissionCode)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Permission)
                .AnyAsync(rp => rp.RoleId == roleId && 
                               rp.Permission.PermissionCode == permissionCode && 
                               rp.Permission.IsActive);
        }

        public async Task<IEnumerable<string>> GetRolePermissionCodesAsync(int roleId)
        {
            return await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => rp.Permission.PermissionCode)
                .ToListAsync();
        }
    }
}
