using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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
        private readonly IApplicationRepository _applicationRepository;

        public ApplyTpNocController(
            IApplyTpNocRepository repository,
            AppDbContext context,
            IApplicationService applicationService,
            ILogger<ApplyTpNocController> logger,
            IApplicationRepository applicationRepository)
        {
            _repository = repository;
            _context = context;
            _applicationService = applicationService;
            _logger = logger;
            _applicationRepository = applicationRepository;
        }

        [HttpGet("forestproduces/{stateCode}")]
        public async Task<ActionResult<List<ForestProduceDto>>> GetForestProducesByState(int stateCode)
        {
            try
            {
                var forestProduces = await _repository.GetForestProducesByStateAsync(stateCode);
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

        [HttpGet("districts/{stateCode}")]
        public async Task<ActionResult<List<District>>> GetDistrictsByState(int stateCode)
        {
            var districts = await _repository.GetDistrictsByState(stateCode);
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
                var result = await _applicationService.GetApplicationWithSpeciesLogsAsync(applicationId);
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

        // OfficerService/Controllers/ApplyTpNocController.cs
        [HttpGet("registered/{userId}")]
        public async Task<IActionResult> GetRegisteredApplications(string userId)
        {
            try
            {
                var result = await _applicationService.GetRegisteredApplicationsAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting registered applications for user: {UserId}", userId);
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

        [HttpPost("save-produce-source")]
        public async Task<IActionResult> SaveProduceSource([FromBody] SaveProduceSourceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving produce source details for ApplicationId: {ApplicationId}",
                    request.ApplicationId);

                var result = await _applicationService.SaveProduceSourceAsync(request);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Produce source details saved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving produce source details for ApplicationId: {ApplicationId}",
                    request.ApplicationId);

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("save-destination")]
        public async Task<IActionResult> SaveDestination([FromBody] SaveDestinationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving destination details for ApplicationId: {ApplicationId}",
                    request.ApplicationId);

                var result = await _applicationService.SaveDestinationAsync(request);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Destination details saved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving destination details for ApplicationId: {ApplicationId}",
                    request.ApplicationId);

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("source-destination-details/{applicationId}/{forestProduceId}")]
        public async Task<IActionResult> GetSourceDestinationDetails(long applicationId, int forestProduceId)
        {
            try
            {
                _logger.LogInformation("Getting source/destination details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    applicationId, forestProduceId);

                var result = await _applicationService.GetSourceDestinationDetailsAsync(applicationId, forestProduceId);

                if (result == null)
                    return NotFound(new { success = false, message = "Source/destination details not found" });

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting source/destination details for ApplicationId: {ApplicationId}",
                    applicationId);

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("check-source-destination/{applicationId}")]
        public async Task<IActionResult> CheckSourceDestinationExists(long applicationId)
        {
            try
            {
                // Get all forest produces for this application
                var applicationDetails = await _context.ApplicationDetails
                    .Where(ad => ad.ApplicationId == applicationId)
                    .ToListAsync();

                var results = new List<object>();

                foreach (var detail in applicationDetails)
                {
                    if (string.IsNullOrEmpty(detail.RegistrationNo))
                        continue;

                    var exists = await _applicationRepository.CheckSourceDestinationExistsAsync(
                        detail.RegistrationNo, Convert.ToInt32(detail.ApplicationCateogryId));

                    results.Add(new
                    {
                        CateogryId = detail.ApplicationCateogryId,
                        registrationNo = detail.RegistrationNo,
                        sourceDestinationExists = exists
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking source/destination existence for ApplicationId: {ApplicationId}",
                    applicationId);

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("application-details/{applicationId}")]
        public async Task<IActionResult> GetApplicationDetails(long applicationId)
        {
            try
            {
                // Get application master with projection to avoid circular reference
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .Where(a => a.ApplicationId == applicationId)
                    .Select(a => new
                    {
                        a.ApplicationId,
                        a.Status,
                        a.ApplicationStatus,
                        a.CreateByUserId,
                        a.CreateByUserName,
                        a.CreatedDate,
                        a.StateId,
                        a.DistrictId,
                        a.SubDistrictId,
                        a.ForestProduceId,
                        a.UpdatedByUserId,
                        a.Remarks,
                        a.UpdatedDate,
                        a.UpdatedByUserName,
                        State = a.State != null ? new
                        {
                            a.State.STCode,
                            a.State.StateName,
                            a.State.StateCode
                        } : null
                    })
                    .FirstOrDefaultAsync();

                if (application == null)
                    return NotFound(new { success = false, message = "Application not found" });

                // Get application details with projection
                var applicationDetails = await _context.ApplicationDetails
                    .Where(ad => ad.ApplicationId == applicationId)
                    .Include(ad => ad.ApplicationCategory)
                    .Select(ad => new
                    {
                        ad.Id,
                        ad.ApplicationId,
                        ad.RegistrationNo,
                        ad.ApplicationCateogryId,
                        ad.CreateByUserId,
                        ad.CreatedDate,
                        ApplicationCategory = ad.ApplicationCategory != null ? new
                        {
                            ad.ApplicationCategory.Id,
                            ad.ApplicationCategory.CategoryName
                            // Add other properties you need, but avoid navigation properties that cause cycles
                        } : null
                    })
                    .ToListAsync();

                // Get species logs to determine forest produces
                var speciesLogs = new List<object>();

                foreach (var detail in applicationDetails)
                {
                    if (!string.IsNullOrEmpty(detail.RegistrationNo))
                    {
                        // Get species logs for this registration number
                        var logs = await GetSpeciesLogsByRegistrationNoAsync(detail.RegistrationNo);

                        speciesLogs.AddRange(logs.Select(log => new
                        {
                            forestProduceId = detail.ApplicationCateogryId,
                            registrationNo = detail.RegistrationNo,
                            speciesLog = log
                        }));
                    }
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        application = application,
                        applicationDetails = applicationDetails,
                        speciesLogs = speciesLogs
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting application details for ApplicationId: {ApplicationId}", applicationId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private async Task<List<object>> GetSpeciesLogsByRegistrationNoAsync(string registrationNo)
        {
            var allLogs = new List<object>();

            // Round Timber
            var roundLogs = await _context.SpeciesLogsRoundTimbers
                .Include(sl => sl.Species)
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new
                {
                    speciesLogId = sl.Id,
                    speciesId = sl.SpeciesID,
                    forestProduceId = sl.ForestProduceId,
                    speciesName = sl.Species.Name,
                    noOfLogs = sl.LogsNo,
                    middleGirthCm = sl.Girth,
                    lengthCm = sl.Length,
                    quantity = sl.Quantity,
                    volume = sl.Volume,
                    forestProduceType = "RoundTimber"
                })
                .ToListAsync();

            allLogs.AddRange(roundLogs);

            // Bamboo
            var bambooLogs = await _context.SpeciesLogsBamboos
                .Include(sl => sl.Species)
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new
                {
                    speciesLogId = sl.Id,
                    speciesId = sl.SpeciesID,
                    forestProduceId = sl.ForestProduceId,
                    speciesName = sl.Species.Name,
                    middleGirthCm = sl.GirthClass,
                    lengthCm = sl.Length,
                    quantity = sl.Quantity,
                    volume = sl.Volume,
                    forestProduceType = "Bamboo"
                })
                .ToListAsync();

            allLogs.AddRange(roundLogs);

            // Fuelwood
            var fuelwoodLogs = await _context.SpeciesLogsFuelwoods
                .Include(sl => sl.Species)
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new
                {
                    speciesLogId = sl.Id,
                    speciesId = sl.SpeciesID,
                    forestProduceId = sl.ForestProduceId,
                    speciesName = sl.Species.Name,
                    unit = sl.Unit,
                    quantity = sl.Quantity,
                    forestProduceType = "Fuelwood"
                })
                .ToListAsync();

            allLogs.AddRange(fuelwoodLogs);

            // Minor Forest Produce
            var minorForestProduceLogs = await _context.SpeciesLogsMinorForestProduces
                .Include(sl => sl.Species)
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new
                {
                    speciesLogId = sl.Id,
                    speciesId = sl.SpeciesID,
                    forestProduceId = sl.ForestProduceId,
                    speciesName = sl.Species.Name,
                    plantPartID = sl.PlantPartID,
                    quantity = sl.Quantity,
                    forestProduceType = "Minor Forest Produce"
                })
                .ToListAsync();

            allLogs.AddRange(minorForestProduceLogs);

            // Sawn Timber
            var sawnTimberLogs = await _context.SpeciesLogsSawnTimbers
                .Include(sl => sl.Species)
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new
                {
                    speciesLogId = sl.Id,
                    speciesId = sl.SpeciesID,
                    forestProduceId = sl.ForestProduceId,
                    speciesName = sl.Species.Name,
                    noOfLogs = sl.LogsNo,
                    middleGirthCm = sl.Girth,
                    lengthCm = sl.Length,
                    thickness = sl.Thickness,
                    volume = sl.Volume,
                    forestProduceType = "Sawn Timber"
                })
                .ToListAsync();

            allLogs.AddRange(sawnTimberLogs);

            return allLogs;
        }
    }
}
