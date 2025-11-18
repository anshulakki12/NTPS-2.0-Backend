using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMasterRoleRepository _roleRepository;
        private readonly AppDbContext _context;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository,
            IPermissionRepository permissionRepository,
            IMasterRoleRepository roleRepository,
            AppDbContext context)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _context = context;
        }

        public async Task<IEnumerable<RolePermissionDto>> AssignPermissionsToRoleAsync(CreateRolePermissionDto createDto)
        {
            // Verify role exists
            var role = await _roleRepository.GetRoleByIdAsync(createDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {createDto.RoleId} not found.");
            }

            var result = new List<RolePermissionDto>();

            foreach (var permissionId in createDto.PermissionIds)
            {
                // Verify permission exists
                var permission = await _permissionRepository.GetPermissionByIdAsync(permissionId);
                if (permission == null)
                {
                    continue; // Skip invalid permission IDs
                }

                var rolePermission = await _rolePermissionRepository.AssignPermissionAsync(
                    createDto.RoleId, 
                    permissionId, 
                    createDto.AssignedBy);

                result.Add(new RolePermissionDto
                {
                    RolePermissionId = rolePermission.RolePermissionId,
                    RoleId = rolePermission.RoleId,
                    PermissionId = rolePermission.PermissionId,
                    RoleName = role.RoleName,
                    PermissionCode = permission.PermissionCode,
                    PermissionName = permission.PermissionName,
                    Module = permission.Module,
                    AssignedDate = rolePermission.AssignedDate,
                    AssignedBy = rolePermission.AssignedBy
                });
            }

            return result;
        }

        public async Task<IEnumerable<RolePermissionDto>> UpdateRolePermissionsAsync(UpdateRolePermissionDto updateDto)
        {
            // Verify role exists
            var role = await _roleRepository.GetRoleByIdAsync(updateDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {updateDto.RoleId} not found.");
            }

            // Remove all existing permissions
            await _rolePermissionRepository.RemoveAllRolePermissionsAsync(updateDto.RoleId);

            // Assign new permissions
            var createDto = new CreateRolePermissionDto
            {
                RoleId = updateDto.RoleId,
                PermissionIds = updateDto.PermissionIds,
                AssignedBy = updateDto.UpdatedBy
            };

            return await AssignPermissionsToRoleAsync(createDto);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetRolePermissionsAsync(roleId);
            
            return rolePermissions.Select(rp => new RolePermissionDto
            {
                RolePermissionId = rp.RolePermissionId,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                RoleName = rp.Role.RoleName,
                PermissionCode = rp.Permission.PermissionCode,
                PermissionName = rp.Permission.PermissionName,
                Module = rp.Permission.Module,
                AssignedDate = rp.AssignedDate,
                AssignedBy = rp.AssignedBy
            });
        }

        public async Task<UserPermissionDto?> GetUserPermissionsAsync(string loginId)
        {
            // Get user's registration info including role
            var user = await _context.OfficerRegistrations
                .Include(or => or.Role)
                .FirstOrDefaultAsync(or => or.LoginId == loginId);

            if (user == null || user.Role == null)
            {
                return null;
            }

            // Get permissions for the user's role
            var permissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(user.RoleId);

            return new UserPermissionDto
            {
                LoginId = user.LoginId,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                Permissions = permissions.Select(p => new PermissionDto
                {
                    PermissionId = p.PermissionId,
                    PermissionCode = p.PermissionCode,
                    PermissionName = p.PermissionName,
                    Description = p.Description,
                    Module = p.Module,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate,
                    CreatedBy = p.CreatedBy,
                    UpdatedDate = p.UpdatedDate,
                    UpdatedBy = p.UpdatedBy
                }).ToList()
            };
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            return await _rolePermissionRepository.RemovePermissionAsync(roleId, permissionId);
        }

        public async Task<bool> HasPermissionAsync(int roleId, string permissionCode)
        {
            return await _rolePermissionRepository.HasPermissionAsync(roleId, permissionCode);
        }
    }
}
