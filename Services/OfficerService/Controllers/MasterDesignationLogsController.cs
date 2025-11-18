using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.Services;

namespace OfficerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MasterDesignationLogsController : ControllerBase
    {
        private readonly IMasterDesignationLogsService _logsService;

        public MasterDesignationLogsController(IMasterDesignationLogsService logsService)
        {
            _logsService = logsService;
        }

        /// <summary>
        /// Get all designation logs
        /// </summary>
        [HttpGet("GetAllLogs")]
        [Authorize(Roles = "1")]
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
        /// Get logs for a specific designation
        /// </summary>
        [HttpGet("GetLogsByDesignationId/{designationId:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetLogsByDesignationId(int designationId)
        {
            try
            {
                var logs = await _logsService.GetLogsByDesignationIdAsync(designationId);
                
                if (!logs.Any())
                {
                    return NotFound(new { message = $"No logs found for Designation ID {designationId}." });
                }

                return Ok(new
                {
                    message = "Logs retrieved successfully",
                    designationId = designationId,
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
