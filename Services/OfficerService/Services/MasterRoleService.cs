using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterRoleService : IMasterRoleService
    {
        private readonly IMasterRoleRepository _roleRepository;
        private readonly IMasterRolesLogsService _logsService;
        private readonly AppDbContext _context;

        public MasterRoleService(IMasterRoleRepository roleRepository, IMasterRolesLogsService logsService, AppDbContext context)
        {
            _roleRepository = roleRepository;
            _logsService = logsService;
            _context = context;
        }

        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            try
            {
                var role = await _roleRepository.CreateRoleAsync(createRoleDto);
                return MapToResponseDto(role);
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation exceptions
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the role.", ex);
            }
        }

        public async Task<RoleResponseDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            return role != null ? MapToResponseDto(role) : null;
        }

        public async Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<RoleResponseDto>> GetActiveRolesAsync()
        {
            var roles = await _roleRepository.GetActiveRolesAsync();
            return roles.Select(MapToResponseDto);
        }

        public async Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto, string? ipAddress = null)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            // Check if another role with the same name exists (excluding current role)
            var duplicateRole = await _roleRepository.GetRoleByNameAsync(updateRoleDto.RoleName);
            if (duplicateRole != null && duplicateRole.RoleId != roleId)
            {
                throw new InvalidOperationException($"Role '{updateRoleDto.RoleName}' already exists.");
            }

            // Create a copy of the old role for logging
            var oldRole = new MasterRoles
            {
                RoleId = existingRole.RoleId,
                RoleName = existingRole.RoleName,
                IsActive = existingRole.IsActive,
                CreatedDate = existingRole.CreatedDate,
                CreatedBy = existingRole.CreatedBy,
                UpdatedDate = existingRole.UpdatedDate,
                UpdatedBy = existingRole.UpdatedBy,
                DeactivatedOn = existingRole.DeactivatedOn,
                ReactivatedOn = existingRole.ReactivatedOn
            };

            // Track activation/deactivation status changes
            var wasActive = existingRole.IsActive;
            var willBeActive = updateRoleDto.IsActive;

            // Update properties
            existingRole.RoleName = updateRoleDto.RoleName;
            existingRole.IsActive = updateRoleDto.IsActive;
            existingRole.UpdatedDate = DateTime.UtcNow;
            existingRole.UpdatedBy = updateRoleDto.UpdatedBy;

            // Track deactivation/reactivation - PRESERVE ALL HISTORY
            if (wasActive && !willBeActive)
            {
                // Being deactivated - PRESERVE ReactivatedOn date
                existingRole.DeactivatedOn = DateTime.UtcNow;
                // DON'T clear ReactivatedOn - preserve reactivation history
            }
            else if (!wasActive && willBeActive)
            {
                // Being reactivated - PRESERVE DeactivatedOn date
                existingRole.ReactivatedOn = DateTime.UtcNow;
                // DON'T clear DeactivatedOn - preserve deactivation history
            }

            var updatedRole = await _roleRepository.UpdateRoleAsync(existingRole);
            
            // Log the update operation with IP address
            await _logsService.LogUpdateAsync(oldRole, updatedRole, updateRoleDto.UpdatedBy ?? "System", ipAddress);
            
            return MapToResponseDto(updatedRole);
        }

        public async Task<RoleResponseDto> DeactivateRoleAsync(int roleId, string deactivatedBy, string? ipAddress = null)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            if (!existingRole.IsActive)
            {
                throw new InvalidOperationException($"Role with ID {roleId} is already deactivated.");
            }

            existingRole.IsActive = false;
            existingRole.DeactivatedOn = DateTime.UtcNow;
            existingRole.UpdatedDate = DateTime.UtcNow;
            existingRole.UpdatedBy = deactivatedBy;
            // PRESERVE ReactivatedOn date - DON'T clear reactivation history
            // existingRole.ReactivatedOn = null; // ← REMOVED: Don't clear reactivation date

            var updatedRole = await _roleRepository.UpdateRoleAsync(existingRole);
            
            return MapToResponseDto(updatedRole);
        }

        public async Task<RoleResponseDto> ReactivateRoleAsync(int roleId, string reactivatedBy, string? ipAddress = null)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            if (existingRole.IsActive)
            {
                throw new InvalidOperationException($"Role with ID {roleId} is already active.");
            }

            existingRole.IsActive = true;
            existingRole.ReactivatedOn = DateTime.UtcNow;
            existingRole.UpdatedDate = DateTime.UtcNow;
            existingRole.UpdatedBy = reactivatedBy;
            // PRESERVE DeactivatedOn date - DON'T clear deactivation history
            // existingRole.DeactivatedOn = null; // ← REMOVED: Don't clear deactivation date

            var updatedRole = await _roleRepository.UpdateRoleAsync(existingRole);
            
            return MapToResponseDto(updatedRole);
        }

        public async Task<bool> DeleteRoleAsync(int roleId, string performedBy = "System", string? ipAddress = null)
        {
            try
            {
                // Get role details before deletion for logging
                var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
                if (existingRole == null)
                {
                    return false;
                }

                var isDeleted = await _roleRepository.DeleteRoleAsync(roleId);
                
                if (isDeleted)
                {
                    // Log the deletion with IP address
                    await _logsService.LogDeleteAsync(existingRole, performedBy, ipAddress);
                }

                return isDeleted;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation exceptions
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the role.", ex);
            }
        }

        private static RoleResponseDto MapToResponseDto(MasterRoles role)
        {
            return new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive,
                CreatedDate = role.CreatedDate,
                CreatedBy = role.CreatedBy,
                UpdatedDate = role.UpdatedDate,
                UpdatedBy = role.UpdatedBy,
                DeactivatedOn = role.DeactivatedOn,
                ReactivatedOn = role.ReactivatedOn
            };
        }

        public async Task<MasterRoles?> GetRoleByIdAsyncs(int roleId)
        {
            return await _context.MasterRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<List<MasterRoles>> GetAllActiveRolesAsync()
        {
            return await _context.MasterRoles
                .Where(r => r.IsActive)
                .OrderBy(r => r.RoleId)
                .ToListAsync();
        }

        public async Task<bool> IsRoleActiveAsync(int roleId)
        {
            return await _context.MasterRoles
                .AnyAsync(r => r.RoleId == roleId && r.IsActive);
        }

        public async Task<List<int>> GetValidRoleIdsAsync()
        {
            return await _context.MasterRoles
                .Where(r => r.IsActive)
                .Select(r => r.RoleId)
                .ToListAsync();
        }

        // Get role hierarchy (for officer chain of command)
        public async Task<Dictionary<int, List<int>>> GetRoleHierarchyAsync()
        {
            // Define role hierarchy based on your system
            var hierarchy = new Dictionary<int, List<int>>
            {
                // Admin can access all roles
                { 1, new List<int> { 1, 19, 20, 21, 22, 23, 24, 25, 28 } },
                // State Officer can access lower level officer roles
                { 19, new List<int> { 19, 20, 21, 22, 23, 24, 25 } },
                // Circle Officer can access division and below
                { 20, new List<int> { 20, 21, 22, 23, 24, 25 } },
                // Division Officer can access subdivision and below
                { 21, new List<int> { 21, 22, 23, 24, 25 } },
                // Subdivision Officer can access range and below
                { 22, new List<int> { 22, 23, 24, 25 } },
                // Range Officer can access MSI and Depot
                { 23, new List<int> { 23, 24, 25 } },
                // MSI Officer - standalone
                { 24, new List<int> { 24 } },
                // Depot Officer - standalone
                { 25, new List<int> { 25 } },
                // Applicant - standalone
                { 28, new List<int> { 28 } }
            };

            // Filter only active roles
            var activeRoleIds = await GetValidRoleIdsAsync();

            foreach (var key in hierarchy.Keys.ToList())
            {
                hierarchy[key] = hierarchy[key].Where(id => activeRoleIds.Contains(id)).ToList();
            }

            return hierarchy;
        }

        // Check if role has permission to access another role
        public async Task<bool> CanAccessRoleAsync(int userRoleId, int targetRoleId)
        {
            var hierarchy = await GetRoleHierarchyAsync();

            if (hierarchy.ContainsKey(userRoleId))
            {
                return hierarchy[userRoleId].Contains(targetRoleId);
            }

            return false;
        }
    }
}