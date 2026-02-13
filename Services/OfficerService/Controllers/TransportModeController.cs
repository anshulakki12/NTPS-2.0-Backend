// Controllers/TransportModeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;
using System.ComponentModel.DataAnnotations;
using static OfficerService.DtoModels.OfficerMastersDTO;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransportModeController : ControllerBase
    {
        private readonly ITransportModeRepository _repository;
        private readonly ILogger<TransportModeController> _logger;

        public TransportModeController(
            ITransportModeRepository repository,
            ILogger<TransportModeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transportModes = await _repository.GetAllAsync();
                return Ok(new BaseResponse<IEnumerable<MasterTransportMode>>
                {
                    Success = true,
                    Message = "Transport modes retrieved successfully",
                    Data = transportModes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transport modes");
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching transport modes"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var transportMode = await _repository.GetByIdAsync(id);
                if (transportMode == null)
                {
                    return NotFound(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport mode not found"
                    });
                }

                return Ok(new BaseResponse<MasterTransportMode>
                {
                    Success = true,
                    Message = "Transport mode retrieved successfully",
                    Data = transportMode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transport mode with ID: {Id}", id);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while fetching transport mode"
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TransportModeCreateRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.TransportMode))
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport Mode is required"
                    });
                }

                // Check if transport mode already exists
                if (await _repository.ExistsAsync(request.TransportMode))
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport Mode already exists"
                    });
                }

                var transportMode = new MasterTransportMode
                {
                    TransportMode = request.TransportMode.Trim(),
                    IsActive = request.IsActive,
                    CreatedBy = request.OfficerId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = request.OfficerId,
                    ModifiedDate = DateTime.UtcNow
                };

                var created = await _repository.CreateAsync(transportMode);

                return Ok(new BaseResponse<MasterTransportMode>
                {
                    Success = true,
                    Message = "Transport Mode created successfully",
                    Data = created
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transport mode");
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while creating transport mode"
                });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransportModeUpdateRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.TransportMode))
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport Mode is required"
                    });
                }

                var transportMode = await _repository.GetByIdAsync(id);
                if (transportMode == null)
                {
                    return NotFound(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport mode not found"
                    });
                }

                // Check if transport mode already exists (excluding current one)
                if (await _repository.ExistsAsync(request.TransportMode, id))
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport Mode already exists"
                    });
                }

                transportMode.TransportMode = request.TransportMode.Trim();
                transportMode.IsActive = request.IsActive;
                transportMode.ModifiedBy = request.OfficerId;
                transportMode.ModifiedDate = DateTime.UtcNow;

                var updated = await _repository.UpdateAsync(transportMode);

                return Ok(new BaseResponse<MasterTransportMode>
                {
                    Success = true,
                    Message = "Transport Mode updated successfully",
                    Data = updated
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transport mode with ID: {Id}", id);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating transport mode"
                });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _repository.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport mode not found"
                    });
                }

                return Ok(new BaseResponse<string>
                {
                    Success = true,
                    Message = "Transport Mode deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transport mode with ID: {Id}", id);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting transport mode"
                });
            }
        }

        [HttpPatch("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id, [FromBody] StatusToggleRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.OfficerId))
                {
                    return BadRequest(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Officer ID is required"
                    });
                }

                var success = await _repository.ToggleStatusAsync(id, request.IsActive, request.OfficerId);
                if (!success)
                {
                    return NotFound(new BaseResponse<string>
                    {
                        Success = false,
                        Message = "Transport mode not found"
                    });
                }

                return Ok(new BaseResponse<string>
                {
                    Success = true,
                    Message = $"Transport Mode {(request.IsActive ? "activated" : "deactivated")} successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for transport mode with ID: {Id}", id);
                return StatusCode(500, new BaseResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating status"
                });
            }
        }
    }
}