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
    public class MasterModulesController : ControllerBase
    {
        private readonly IMasterModuleService _moduleService;

        public MasterModulesController(IMasterModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        /// <summary>
        /// Get all modules
        /// </summary>
        [HttpGet("GetAllModules")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAllModules()
        {
            try
            {
                var modules = await _moduleService.GetAllModulesAsync();
                return Ok(new
                {
                    message = "Modules retrieved successfully",
                    modules = modules,
                    count = modules.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving modules.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active modules only
        /// </summary>
        [HttpGet("GetActiveModules")]
        public async Task<IActionResult> GetActiveModules()
        {
            try
            {
                var modules = await _moduleService.GetActiveModulesAsync();
                return Ok(new
                {
                    message = "Active modules retrieved successfully",
                    modules = modules,
                    count = modules.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active modules.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get module by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var module = await _moduleService.GetModuleByIdAsync(id);
                if (module == null)
                {
                    return NotFound(new { message = $"Module with ID {id} not found." });
                }

                return Ok(new
                {
                    message = "Module retrieved successfully",
                    module = module
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the module.", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new module
        /// </summary>
        [HttpPost("CreateModule")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleDto createDto)
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

                var module = await _moduleService.CreateModuleAsync(createDto);

                return CreatedAtAction(
                    nameof(GetModuleById),
                    new { id = module.ModuleId },
                    new
                    {
                        message = "Module created successfully",
                        module = module
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the module.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing module
        /// </summary>
        [HttpPut("UpdateModule/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] UpdateModuleDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateDto.ModuleId)
            {
                return BadRequest(new { message = "Module ID mismatch." });
            }

            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";
                updateDto.ModifiedBy = loginId;

                var module = await _moduleService.UpdateModuleAsync(id, updateDto);
                return Ok(new
                {
                    message = "Module updated successfully",
                    module = module
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the module.", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a module
        /// </summary>
        [HttpPatch("ActivateModule/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ActivateModule(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var module = await _moduleService.ActivateModuleAsync(id, loginId);
                return Ok(new
                {
                    message = "Module activated successfully",
                    module = module
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the module.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a module
        /// </summary>
        [HttpPatch("DeactivateModule/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeactivateModule(int id)
        {
            try
            {
                var loginId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value 
                           ?? "System";

                var module = await _moduleService.DeactivateModuleAsync(id, loginId);
                return Ok(new
                {
                    message = "Module deactivated successfully",
                    module = module
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the module.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a module
        /// </summary>
        [HttpDelete("DeleteModule/{id:int}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            try
            {
                var moduleToDelete = await _moduleService.GetModuleByIdAsync(id);
                
                var isDeleted = await _moduleService.DeleteModuleAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Module with ID {id} not found." });
                }

                return Ok(new 
                { 
                    message = "Module deleted successfully",
                    deletedModule = moduleToDelete?.ModuleName
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the module.", error = ex.Message });
            }
        }
    }
}
