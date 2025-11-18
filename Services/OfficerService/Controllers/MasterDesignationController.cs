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
    public class MasterDesignationController : ControllerBase
    {
        private readonly IMasterDesignationService _designationService;

        public MasterDesignationController(IMasterDesignationService designationService)
        {
            _designationService = designationService;
        }

        /// <summary>
        /// Helper method to get the client IP address
        /// </summary>
        private string? GetClientIpAddress()
        {
            // Check for forwarded IP (when behind proxy/load balancer)
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }

            // Check X-Real-IP header
            var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fall back to RemoteIpAddress
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Create a new designation
        /// </summary>
        [HttpPost("CreateDesignation")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationDto createDesignationDto)
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
                createDesignationDto.CreatedBy = loginId;

                var createdDesignation = await _designationService.CreateDesignationAsync(createDesignationDto);

                return CreatedAtAction(
                    nameof(GetDesignationById),
                    new { id = createdDesignation.DesignationId },
                    new
                    {
                        message = "Designation created successfully",
                        designation = createdDesignation
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the designation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all designations
        /// </summary>
        [HttpGet("GetAllDesignations")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllDesignations()
        {
            try
            {
                var designations = await _designationService.GetAllDesignationsAsync();
                return Ok(designations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get only active designations
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDesignations()
        {
            try
            {
                var designations = await _designationService.GetActiveDesignationsAsync();
                return Ok(new
                {
                    message = "Active designations retrieved successfully",
                    designations = designations,
                    count = designations.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active designations.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get designation by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var designation = await _designationService.GetDesignationByIdAsync(id);
                if (designation == null)
                {
                    return NotFound(new { message = $"Designation with ID {id} not found." });
                }

                return Ok(new
                {
                    message = "Designation retrieved successfully",
                    designation = designation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the designation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing designation
        /// </summary>
        [HttpPut("UpdateDesignation/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody] UpdateDesignationDto updateDesignationDto)
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
                updateDesignationDto.UpdatedBy = loginId;

                var ipAddress = GetClientIpAddress();

                var updatedDesignation = await _designationService.UpdateDesignationAsync(id, updateDesignationDto, ipAddress);
                return Ok(new
                {
                    message = "Designation updated successfully",
                    designation = updatedDesignation
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the designation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a designation
        /// </summary>
        [HttpPatch("DeactivateDesignation/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeactivateDesignation(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var ipAddress = GetClientIpAddress();

                var deactivatedDesignation = await _designationService.DeactivateDesignationAsync(id, loginId, ipAddress);
                return Ok(new
                {
                    message = "Designation deactivated successfully",
                    designation = deactivatedDesignation
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the designation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Reactivate a designation
        /// </summary>
        [HttpPatch("ActivateDesignation/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ReactivateDesignation(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var ipAddress = GetClientIpAddress();

                var reactivatedDesignation = await _designationService.ReactivateDesignationAsync(id, loginId, ipAddress);
                return Ok(new
                {
                    message = "Designation reactivated successfully",
                    designation = reactivatedDesignation
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while reactivating the designation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a designation
        /// </summary>
        [HttpDelete("DeleteDesignation/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var designationToDelete = await _designationService.GetDesignationByIdAsync(id);
                
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var ipAddress = GetClientIpAddress();

                var isDeleted = await _designationService.DeleteDesignationAsync(id, loginId, ipAddress);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Designation with ID {id} not found." });
                }

                return Ok(new 
                { 
                    message = "Designation deleted successfully",
                    deletedDesignation = designationToDelete?.DesignationName,
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
                return StatusCode(500, new { message = "An error occurred while deleting the designation.", error = ex.Message });
            }
        }
    }
}
