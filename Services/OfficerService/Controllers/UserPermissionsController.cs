using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.Services;
using System.Security.Claims;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPermissionsController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public UserPermissionsController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        /// <summary>
        /// Get all permissions for a specific user by their LoginId
        /// </summary>
        [HttpGet("GetUserPermissions/{loginId}")]
        public async Task<IActionResult> GetUserPermissions(string loginId)
        {
            try
            {
                // Verify the user is requesting their own permissions or is an admin
                var currentLoginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                  ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("1");

                if (currentLoginId != loginId && !isAdmin)
                {
                    return Forbid();
                }

                var userPermissions = await _rolePermissionService.GetUserPermissionsAsync(loginId);
                if (userPermissions == null)
                {
                    return NotFound(new { message = $"User with LoginId '{loginId}' not found or has no role assigned." });
                }

                return Ok(new
                {
                    message = "User permissions retrieved successfully",
                    userPermissions = userPermissions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions for the currently authenticated user
        /// </summary>
        [HttpGet("GetMyPermissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(loginId))
                {
                    return Unauthorized(new { message = "Unable to identify user." });
                }

                var userPermissions = await _rolePermissionService.GetUserPermissionsAsync(loginId);
                if (userPermissions == null)
                {
                    return NotFound(new { message = "User not found or has no role assigned." });
                }

                return Ok(new
                {
                    message = "Your permissions retrieved successfully",
                    userPermissions = userPermissions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permissions.", error = ex.Message });
            }
        }
    }
}
