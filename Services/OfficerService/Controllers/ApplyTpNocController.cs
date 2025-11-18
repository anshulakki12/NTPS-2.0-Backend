using Microsoft.AspNetCore.Mvc;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;
using OfficerService.Repositories;
using OfficerService.Services;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplyTpNocController : ControllerBase
    {
        private readonly IApplyTpNocRepository _repository;
        private readonly AppDbContext _context;
        private readonly IApplicationService _applicationService;
        private readonly ILogger<ApplyTpNocController> _logger;

        public ApplyTpNocController(
            IApplyTpNocRepository repository,
            AppDbContext context,
            IApplicationService applicationService,
            ILogger<ApplyTpNocController> logger)
        {
            _repository = repository;
            _context = context;
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpGet("forestproduces/{stateId}")]
        public async Task<ActionResult<List<ForestProduceDto>>> GetForestProducesByState(int stateId)
        {
            try
            {
                var forestProduces = await _repository.GetForestProducesByStateAsync(stateId);
                return Ok(forestProduces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("species/{stateId}/{forestProduceId}")]
        public async Task<ActionResult<List<SpeciesDto>>> GetSpeciesByStateAndForestProduce(int stateId, int forestProduceId)
        {
            try
            {
                var species = await _repository.GetSpeciesByStateAndForestProduceAsync(stateId, forestProduceId);
                return Ok(species);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("mappings/{stateId}")]
        public async Task<ActionResult<List<SpeciesMappingDto>>> GetSpeciesMappingsByState(int stateId)
        {
            try
            {
                var mappings = await _repository.GetSpeciesMappingsByStateAsync(stateId);
                return Ok(mappings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("districts/{stateId}")]
        public async Task<ActionResult<List<District>>> GetDistrictsByState(int stateId)
        {
            var districts = await _repository.GetDistrictsByState(stateId);
            return Ok(districts);
        }

        [HttpGet("subdistricts/{districtId}")]
        public async Task<ActionResult<List<SubDistrict>>> GetSubDistrictsByDistrict(int districtId)
        {
            var subDistricts = await _repository.GetSubDistrictsByDistrict(districtId);
            return Ok(subDistricts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequestDto request)
        {
            try
            {
                var result = await _applicationService.CreateApplicationAsync(request);
                return Ok(new { success = true, data = result, message = "Application created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        //[HttpPost("add-produce-details")]
        //public async Task<IActionResult> AddProduceDetails([FromBody] AddProduceDetailRequestDto request)
        //{
        //    try
        //    {
        //        var result = await _applicationService.AddProduceDetailsAsync(request);
        //        return Ok(new { success = true, data = result, message = "Produce details added successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost("add-produce-details")]
        public async Task<IActionResult> AddProduceDetails([FromBody] AddProduceDetailRequestDto request)
        {
            try
            {
                _logger.LogInformation("Received request to add produce details: {@Request}", request);

                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request cannot be null" });
                }

                if (request.ApplicationId <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid ApplicationId" });
                }

                if (request.ProduceDetails == null || !request.ProduceDetails.Any())
                {
                    return BadRequest(new { success = false, message = "ProduceDetails cannot be empty" });
                }

                var result = await _applicationService.AddProduceDetailsAsyncs(request);

                _logger.LogInformation("Successfully processed produce details for ApplicationId: {ApplicationId}", request.ApplicationId);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Produce details added successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding produce details for ApplicationId: {ApplicationId}", request?.ApplicationId);

                // Return more detailed error information
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

            [HttpGet("details/{applicationId}")]
            public async Task<IActionResult> GetApplication(long applicationId)
           {
            try
            {
                var result = await _applicationService.GetApplicationAsync(applicationId);
                if (result == null)
                    return NotFound(new { success = false, message = "Application not found" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserApplications(string userId)
        {
            try
            {
                var result = await _applicationService.GetUserApplicationsAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/open")]
        public async Task<IActionResult> GetOpenUserApplications(string userId)
        {
            try
            {
                var result = await _applicationService.GetOpenUserApplicationsAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetApplicationById(long applicationId)
        {
            try
            {
                var result = await _applicationService.GetApplicationByIdAsyncs(applicationId);
                if (result == null)
                    return NotFound(new { success = false, message = "Application not found" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update-application")]
        public async Task<IActionResult> UpdateApplication([FromBody] UpdateApplicationRequestDto request)
        {
            try
            {
                var result = await _applicationService.UpdateApplicationAsync(request);
                if (result == null)
                    return NotFound(new { success = false, message = "Application not found" });

                return Ok(new { success = true, data = result, message = "Application updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{applicationId}/species-logs")]
        public async Task<IActionResult> GetSpeciesLogs(long applicationId)
        {
            try
            {
                var result = await _applicationService.GetSpeciesLogsByApplicationAsync(applicationId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update-produce-details")]
        public async Task<IActionResult> UpdateProduceDetails([FromBody] UpdateProduceDetailRequestDto request)
        {
            try
            {
                _logger.LogInformation("Received request to update produce details: {@Request}", request);

                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request cannot be null" });
                }

                var result = await _applicationService.UpdateProduceDetailsAsync(request);

                _logger.LogInformation("Successfully updated produce details for ApplicationId: {ApplicationId}", request.ApplicationId);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Produce details updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating produce details for ApplicationId: {ApplicationId}", request?.ApplicationId);

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpDelete("species-log/{speciesLogId}/{forestProduceType}")]
        public async Task<IActionResult> DeleteSpeciesLog(long speciesLogId, string forestProduceType)
        {
            try
            {
                var result = await _applicationService.DeleteSpeciesLogAsync(speciesLogId, forestProduceType);
                if (!result)
                    return NotFound(new { success = false, message = "Species log not found" });

                return Ok(new { success = true, message = "Species log deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
