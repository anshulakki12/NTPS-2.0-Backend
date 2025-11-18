using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.DtoModels;
using OfficerService.Services;
using System.Security.Claims;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class MasterRolesController : ControllerBase
    {
        private readonly IMasterRoleService _roleService;

        public MasterRolesController(IMasterRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Helper method to get the client IP address
        /// </summary>
        private string? GetClientIpAddress()
        {
            // Try to get the forwarded IP first (from proxy/load balancer)
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs (client, proxy1, proxy2, ...)
                // The first one is usually the original client IP
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var clientIp = ips.FirstOrDefault()?.Trim();
                
                // Skip if it's a loopback address
                if (!string.IsNullOrEmpty(clientIp) && 
                    clientIp != "::1" && 
                    clientIp != "127.0.0.1" &&
                    !clientIp.StartsWith("::ffff:127.0.0.1"))
                {
                    return clientIp;
                }
            }

            // Check X-Real-IP header (common with Nginx)
            var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp) && 
                realIp != "::1" && 
                realIp != "127.0.0.1" &&
                !realIp.StartsWith("::ffff:127.0.0.1"))
            {
                return realIp;
            }

            // Try to get from HttpContext.Connection
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                // Convert IPv4-mapped IPv6 addresses to IPv4
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }

                var ipString = remoteIp.ToString();
                
                // For local development, try to get the actual machine IP
                if (ipString == "::1" || ipString == "127.0.0.1")
                {
                    // Try to get local network IP
                    try
                    {
                        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                        var localIp = host.AddressList
                            .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork 
                                              && !System.Net.IPAddress.IsLoopback(ip));
                        
                        if (localIp != null)
                        {
                            return localIp.ToString();
                        }
                    }
                    catch
                    {
                        // If we can't get the local IP, return the loopback
                    }
                }
                
                return ipString;
            }

            return null;
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="createRoleDto">Role creation data</param>
        /// <returns>Created role information</returns>
        [HttpPost("CreateRole") ] // Add explicit route for CreateRole 
        [Authorize(Roles = "1")] // Use role IDs: 1 = Applicant (adjust as needed)
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the LoginId from JWT claims (Subject claim contains LoginId)
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                createRoleDto.CreatedBy = loginId;

                var createdRole = await _roleService.CreateRoleAsync(createRoleDto);

                return CreatedAtAction(
                    nameof(GetRoleById),
                    new { id = createdRole.RoleId },
                    new
                    {
                        message = "Role created successfully",
                        role = createdRole
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the role.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        [HttpGet("GetAllRoles")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles); // Return only the list, not a wrapped object
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Get only active roles
        /// </summary>
        /// <returns>List of active roles</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveRoles()
        {
            try
            {
                var roles = await _roleService.GetActiveRolesAsync();
                return Ok(new
                {
                    message = "Active roles retrieved successfully",
                    roles = roles,
                    count = roles.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active roles.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role information</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }

                return Ok(new
                {
                    message = "Role retrieved successfully",
                    role = role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the role.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="updateRoleDto">Updated role data</param>
        /// <returns>Updated role information</returns>
        [HttpPut("UpdateRole/{id:int}")]    
        [Authorize(Roles = "1")] // Use role IDs: 2 = Officer, 3 = Admin
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the LoginId from JWT claims for audit trail
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                updateRoleDto.UpdatedBy = loginId;

                // Get client IP address
                var ipAddress = GetClientIpAddress();

                var updatedRole = await _roleService.UpdateRoleAsync(id, updateRoleDto, ipAddress);
                return Ok(new
                {
                    message = "Role updated successfully",
                    role = updatedRole
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the role.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Deactivated role information</returns>
        /// 
        
        [HttpPatch("DeactivateRole/{id:int}")]
        [Authorize(Roles = "1")] // Use role IDs: 2 = Officer, 3 = Admin
        public async Task<IActionResult> DeactivateRole(int id)
        {
            try
            {
                // Get the LoginId from JWT claims for audit trail
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                // Get client IP address
                var ipAddress = GetClientIpAddress();

                var deactivatedRole = await _roleService.DeactivateRoleAsync(id, loginId, ipAddress);
                return Ok(new
                {
                    message = "Role deactivated successfully",
                    role = deactivatedRole
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the role.", error = ex.Message });
            }
        }

        /// <summary>
        /// Reactivate a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Reactivated role information</returns>
        [HttpPatch("ActivateRole/{id:int}")]//for reactivation
        [Authorize(Roles = "1")] // Use role IDs: 2 = Officer, 3 = Admin
        public async Task<IActionResult> ReactivateRole(int id)
        {
            try
            {
                // Get the LoginId from JWT claims for audit trail
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                // Get client IP address
                var ipAddress = GetClientIpAddress();

                var reactivatedRole = await _roleService.ReactivateRoleAsync(id, loginId, ipAddress);
                return Ok(new
                {
                    message = "Role reactivated successfully",
                    role = reactivatedRole
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while reactivating the role.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Success or error message</returns>
        [HttpDelete("DeleteRole/{id:int}")]
        [Authorize(Roles = "1")] // Use role ID: 1 = Admin only
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                // Get role details before deletion for logging
                var roleToDelete = await _roleService.GetRoleByIdAsync(id);
                
                // Get the LoginId from JWT claims for audit trail
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                // Get client IP address
                var ipAddress = GetClientIpAddress();

                var isDeleted = await _roleService.DeleteRoleAsync(id, loginId, ipAddress);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }

                return Ok(new 
                { 
                    message = "Role deleted successfully",
                    deletedRole = roleToDelete?.RoleName,
                    deletedBy = loginId,
                    deletedAt = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the role.", error = ex.Message });
            }
        }
    }
}
