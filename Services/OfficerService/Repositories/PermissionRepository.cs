using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> CreatePermissionAsync(CreatePermissionDto createDto)
        {
            // Check if permission code already exists
            var existingPermission = await _context.Set<Permission>()
                .FirstOrDefaultAsync(p => p.PermissionCode == createDto.PermissionCode);

            if (existingPermission != null)
            {
                throw new InvalidOperationException($"Permission with code '{createDto.PermissionCode}' already exists.");
            }

            var permission = new Permission
            {
                PermissionCode = createDto.PermissionCode,
                PermissionName = createDto.PermissionName,
                Description = createDto.Description,
                Module = createDto.Module,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createDto.CreatedBy
            };

            _context.Set<Permission>().Add(permission);
            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission?> GetPermissionByIdAsync(int permissionId)
        {
            return await _context.Set<Permission>()
                .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        }

        public async Task<Permission?> GetPermissionByCodeAsync(string permissionCode)
        {
            return await _context.Set<Permission>()
                .FirstOrDefaultAsync(p => p.PermissionCode == permissionCode);
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Set<Permission>()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
        {
            return await _context.Set<Permission>()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module)
        {
            return await _context.Set<Permission>()
                .Where(p => p.Module == module && p.IsActive)
                .OrderBy(p => p.PermissionName)
                .ToListAsync();
        }

        public async Task<Permission> UpdatePermissionAsync(Permission permission)
        {
            permission.UpdatedDate = DateTime.UtcNow;
            _context.Set<Permission>().Update(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            var permission = await GetPermissionByIdAsync(permissionId);
            if (permission == null)
            {
                return false;
            }

            // Check if permission is assigned to any roles
            var isAssigned = await _context.Set<RolePermission>()
                .AnyAsync(rp => rp.PermissionId == permissionId);

            if (isAssigned)
            {
                throw new InvalidOperationException("Cannot delete permission that is assigned to roles.");
            }

            _context.Set<Permission>().Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
