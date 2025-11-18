using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.Services;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class MasterRolesLogsController : ControllerBase
    {
        private readonly IMasterRolesLogsService _logsService;

        public MasterRolesLogsController(IMasterRolesLogsService logsService)
        {
            _logsService = logsService;
        }

        /// <summary>
        /// Get all role logs
        /// </summary>
        /// <returns>List of all role operation logs</returns>
        [HttpGet("GetAllLogs")]
        [Authorize(Roles = "1")] // Admin only
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _logsService.GetAllLogsAsync();
                return Ok(new
                {
                    message = "Logs retrieved successfully",
                    count = logs.Count(),
                    logs = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving logs.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get logs for a specific role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of logs for the specified role</returns>
        [HttpGet("GetLogsByRoleId/{roleId:int}")]
        [Authorize(Roles = "1")] // Admin only
        public async Task<IActionResult> GetLogsByRoleId(int roleId)
        {
            try
            {
                var logs = await _logsService.GetLogsByRoleIdAsync(roleId);
                
                if (!logs.Any())
                {
                    return NotFound(new { message = $"No logs found for Role ID {roleId}." });
                }

                return Ok(new
                {
                    message = "Logs retrieved successfully",
                    roleId = roleId,
                    count = logs.Count(),
                    logs = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving logs.", error = ex.Message });
            }
        }
    }
}
