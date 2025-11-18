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
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        [HttpGet("GetAllPermissions")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();
                return Ok(new
                {
                    message = "Permissions retrieved successfully",
                    permissions = permissions,
                    count = permissions.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permissions.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active permissions only
        /// </summary>
        [HttpGet("GetActivePermissions")]
        public async Task<IActionResult> GetActivePermissions()
        {
            try
            {
                var permissions = await _permissionService.GetActivePermissionsAsync();
                return Ok(new
                {
                    message = "Active permissions retrieved successfully",
                    permissions = permissions,
                    count = permissions.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions by module
        /// </summary>
        [HttpGet("GetPermissionsByModule/{module}")]
        public async Task<IActionResult> GetPermissionsByModule(string module)
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsByModuleAsync(module);
                return Ok(new
                {
                    message = $"Permissions for module '{module}' retrieved successfully",
                    module = module,
                    permissions = permissions,
                    count = permissions.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permission by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var permission = await _permissionService.GetPermissionByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new { message = $"Permission with ID {id} not found." });
                }

                return Ok(new
                {
                    message = "Permission retrieved successfully",
                    permission = permission
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new permission
        /// </summary>
        [HttpPost("CreatePermission")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto createDto)
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
                createDto.CreatedBy = loginId;

                var permission = await _permissionService.CreatePermissionAsync(createDto);

                return CreatedAtAction(
                    nameof(GetPermissionById),
                    new { id = permission.PermissionId },
                    new
                    {
                        message = "Permission created successfully",
                        permission = permission
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing permission
        /// </summary>
        [HttpPut("UpdatePermission/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionDto updateDto)
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

                var permission = await _permissionService.UpdatePermissionAsync(id, updateDto);
                return Ok(new
                {
                    message = "Permission updated successfully",
                    permission = permission
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the permission.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        [HttpDelete("DeletePermission/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var isDeleted = await _permissionService.DeletePermissionAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Permission with ID {id} not found." });
                }

                return Ok(new { message = "Permission deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the permission.", error = ex.Message });
            }
        }
    }
}
