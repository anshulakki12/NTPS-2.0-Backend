using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using OfficerService.DtoModels;

namespace OfficerService.Repositories
{
    public class MasterRoleRepository : IMasterRoleRepository
    {
        private readonly AppDbContext _context;

        public MasterRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterRoles> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Check if role already exists
            var existingRole = await GetRoleByNameAsync(createRoleDto.RoleName);
            if (existingRole != null)
            {
                throw new InvalidOperationException($"Role '{createRoleDto.RoleName}' already exists.");
            }

            var role = new MasterRoles
            {
                RoleName = createRoleDto.RoleName,
                IsActive = createRoleDto.IsActive,
                CreatedBy = createRoleDto.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.MasterRoles.Add(role);
            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<MasterRoles?> GetRoleByIdAsync(int roleId)
        {
            return await _context.MasterRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<MasterRoles?> GetRoleByNameAsync(string roleName)
        {
            return await _context.MasterRoles
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        }

        public async Task<IEnumerable<MasterRoles>> GetAllRolesAsync()
        {
            return await _context.MasterRoles
                .OrderBy(r => r.RoleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterRoles>> GetActiveRolesAsync()
        {
            return await _context.MasterRoles
                .Where(r => r.IsActive)
                .OrderBy(r => r.RoleId)
                .ToListAsync();
        }

        public async Task<MasterRoles> UpdateRoleAsync(MasterRoles role)
        {
            _context.MasterRoles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            if (role == null)
                return false;

            // Check if role is being used by any officers
            var isRoleInUse = await _context.OfficerRegistrations
                .AnyAsync(o => o.RoleId == roleId);

            if (isRoleInUse)
            {
                throw new InvalidOperationException("Cannot delete role as it is assigned to one or more officers.");
            }

            _context.MasterRoles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _context.MasterRoles
                .AnyAsync(r => r.RoleName.ToLower() == roleName.ToLower());
        }
    }
}