using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.DtoModels;
using OfficerService.Services;
using System.Security.Claims;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionsController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        /// <summary>
        /// Get all permissions assigned to a role
        /// </summary>
        [HttpGet("GetRolePermissions/{roleId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            try
            {
                var rolePermissions = await _rolePermissionService.GetRolePermissionsAsync(roleId);
                return Ok(new
                {
                    message = "Role permissions retrieved successfully",
                    roleId = roleId,
                    permissions = rolePermissions,
                    count = rolePermissions.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving role permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Assign permissions to a role
        /// </summary>
        [HttpPost("AssignPermissions")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AssignPermissions([FromBody] CreateRolePermissionDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                createDto.AssignedBy = loginId;

                var rolePermissions = await _rolePermissionService.AssignPermissionsToRoleAsync(createDto);

                return Ok(new
                {
                    message = "Permissions assigned successfully",
                    roleId = createDto.RoleId,
                    permissions = rolePermissions,
                    count = rolePermissions.Count()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update role permissions (replaces existing permissions)
        /// </summary>
        [HttpPut("UpdateRolePermissions")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                updateDto.UpdatedBy = loginId;

                var rolePermissions = await _rolePermissionService.UpdateRolePermissionsAsync(updateDto);

                return Ok(new
                {
                    message = "Role permissions updated successfully",
                    roleId = updateDto.RoleId,
                    permissions = rolePermissions,
                    count = rolePermissions.Count()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating role permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove a specific permission from a role
        /// </summary>
        [HttpDelete("RemovePermission/{roleId:int}/{permissionId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemovePermission(int roleId, int permissionId)
        {
            try
            {
                var isRemoved = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
                if (!isRemoved)
                {
                    return NotFound(new { message = $"Permission assignment not found for role {roleId} and permission {permissionId}." });
                }

                return Ok(new
                {
                    message = "Permission removed from role successfully",
                    roleId = roleId,
                    permissionId = permissionId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if a role has a specific permission
        /// </summary>
        [HttpGet("HasPermission/{roleId:int}/{permissionCode}")]
        public async Task<IActionResult> HasPermission(int roleId, string permissionCode)
        {
            try
            {
                var hasPermission = await _rolePermissionService.HasPermissionAsync(roleId, permissionCode);
                return Ok(new
                {
                    roleId = roleId,
                    permissionCode = permissionCode,
                    hasPermission = hasPermission
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking permission.", error = ex.Message });
            }
        }
    }
}
