using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;
using OfficerService.Repositories;
using OfficerService.Services;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static OfficerService.DtoModels.ApplicationDto;
using static OfficerService.DtoModels.DocumentUploadDto;

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
        private readonly IWebHostEnvironment _environment;

        public ApplyTpNocController(
            IApplyTpNocRepository repository,
            AppDbContext context,
            IApplicationService applicationService,
            ILogger<ApplyTpNocController> logger,
            IApplicationRepository applicationRepository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _context = context;
            _applicationService = applicationService;
            _logger = logger;
            _applicationRepository = applicationRepository;
            _environment = environment;
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

        [HttpGet("source-destination-details/{applicationId}/{categoryId}")]
        public async Task<IActionResult> GetSourceDestinationDetails(long applicationId, int categoryId)
        {
            try
            {
                _logger.LogInformation("Getting source/destination details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    applicationId, categoryId);

                var result = await _applicationService.GetSourceDestinationDetailsAsync(applicationId, categoryId);

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

        [HttpGet("check-registration-source/{applicationDetailId}")]
        public async Task<IActionResult> CheckRegistrationSourceExists(string applicationDetailId, [FromQuery] string category)
        {
            try
            {
                bool hasSourceData = false;

                if (category.ToLower() == "transit pass")
                {
                    // Check TPSource table
                    hasSourceData = await _context.TpSourcePlaces
                        .AnyAsync(tsp => tsp.ApplicationId == applicationDetailId);
                }
                else if (category.ToLower() == "noc")
                {
                    // Check NOCSource table
                    hasSourceData = await _context.NocSourcePlaces
                        .AnyAsync(nsp => nsp.ApplicationId == applicationDetailId);
                }

                return Ok(new
                {
                    success = true,
                    data = new { hasSourceData }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking registration source for ApplicationDetailId: {ApplicationDetailId}",
                    applicationDetailId);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet("check-registration-sources/{applicationDetailId}")]
        public async Task<IActionResult> CheckRegistrationSourcesExists(string applicationDetailId, [FromQuery] int category)
        {
            try
            {
                bool hasSourceData = false;

                if (category == 2)
                {
                    // Check TPSource table
                    hasSourceData = await _context.TpSourcePlaces
                        .AnyAsync(tsp => tsp.ApplicationId == applicationDetailId);
                }
                else if (category == 1)
                {
                    // Check NOCSource table
                    hasSourceData = await _context.NocSourcePlaces
                        .AnyAsync(nsp => nsp.ApplicationId == applicationDetailId);
                }

                return Ok(new
                {
                    success = true,
                    data = new { hasSourceData }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking registration source for ApplicationDetailId: {ApplicationDetailId}",
                    applicationDetailId);
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

        [HttpGet("applications-details/{applicationId}")]
        public async Task<IActionResult> GetApplicationsDetails(long applicationId)
        {
            try
            {
                // Get application master with projection to avoid circular reference
                var application = await _context.ApplicationMasters
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
                      a.UpdatedByUserName
                  })
                  .FirstOrDefaultAsync();

                if (application == null)
                    return NotFound(new { success = false, message = "Application not found" });

                // Get application details with projection
                var applicationDetails = await _context.ApplicationDetails
                  .Where(ad => ad.ApplicationId == applicationId && ad.RegistrationNo != null)
                  .Include(ad => ad.ApplicationCategory)
                  .Select(ad => new
                  {
                      ad.Id,
                      ad.ApplicationId,
                      ad.RegistrationNo,
                      ad.ApplicationCateogryId,
                      ad.CreateByUserId,
                      ad.CreatedDate,
                      CategoryName = ad.ApplicationCategory != null ? ad.ApplicationCategory.CategoryName : "Unknown"
                  })
                  .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        application = application,
                        applicationDetails = applicationDetails
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

        [HttpGet("documents")]
        public IActionResult GetDocuments()
        {
            var documents = new[]
            {
    new
    {
        documentTypeId = "DOC001",
        documentName = "Photo of the Forest Produced",
        isActive = true,
        isMandatory = true
    },
    new
    {
        documentTypeId = "DOC002",
        documentName = "Proof of ownership(Land Revenue Records)",
        isActive = false,
        isMandatory = false
    },
    new
    {
        documentTypeId = "DOC003",
        documentName = "Document of felling order",
        isActive = false,
        isMandatory = false
    },
    new
    {
        documentTypeId = "DOC004",
        documentName = "Any Other document",
        isActive = true,
        isMandatory = false
    }
};
            return Ok(documents);
        }


        [HttpGet("CheckDocumentExists")]
        public async Task<IActionResult> CheckDocumentExists([FromQuery] string registrationNo, [FromQuery] string documentTypeId)
        {
            try
            {
                var exists = await _context.PhotoForestProduces
                    .AnyAsync(p => p.RegistrationNo == registrationNo && p.DocumentType == documentTypeId);

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Document check completed",
                    Data = new { exists }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error checking document: {ex.Message}"
                });
            }
        }

        [HttpGet("GetExistingDocuments/{applicationId}")]
        public async Task<IActionResult> GetExistingDocuments(int applicationId, [FromQuery] string registrationNo)
        {
            try
            {
                var documents = await _context.PhotoForestProduces
                    .Where(p => p.ApplicationId == applicationId && p.RegistrationNo == registrationNo)
                    .Select(p => new
                    {
                        p.DocumentType,
                        p.PhotoUpload,
                        p.OtherDocument
                    })
                    .ToListAsync();

                var result = new Dictionary<string, object>();
                foreach (var doc in documents)
                {
                    if (doc.DocumentType == "DOC001")
                    {
                        result["DOC001"] = doc.PhotoUpload;
                    }
                    else if (doc.DocumentType == "DOC004")
                    {
                        result["DOC004"] = doc.OtherDocument;
                    }
                }

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Documents retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error retrieving documents: {ex.Message}"
                });
            }
        }

        //[HttpPost("AddDocuments")]
        //public async Task<IActionResult> AddDocuments([FromForm] DocumentDto request)
        //{
        //    try
        //    {
        //        // Check if document already exists
        //        var existingDoc = await _context.PhotoForestProduces
        //            .FirstOrDefaultAsync(p => p.RegistrationNo == request.RegistrationNo
        //                && p.DocumentType == request.DocumentTypeId);

        //        if (existingDoc != null)
        //        {
        //            return BadRequest(new DocumentResponse
        //            {
        //                Success = false,
        //                Message = "Document already exists. Use update instead."
        //            });
        //        }

        //        var fileNames = new List<string>();
        //        var uploadPath = "";

        //        if (request.DocumentTypeId == "DOC001")
        //        {
        //            uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "photos");

        //            // Handle multiple files for DOC001
        //            foreach (var file in request.Files)
        //            {
        //                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        //                var filePath = Path.Combine(uploadPath, fileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }

        //                fileNames.Add(fileName);
        //            }

        //            var photoDocument = new PhotoForestProduce
        //            {
        //                RegistrationNo = request.RegistrationNo,
        //                DocumentType = request.DocumentTypeId,
        //                PhotoUpload = string.Join(",", fileNames),
        //                ApplicationId = request.ApplicationId,
        //                CreatedDate = DateTime.Now,
        //                CategoryId = request.CategoryId
        //            };

        //            _context.PhotoForestProduces.Add(photoDocument);
        //        }
        //        else if (request.DocumentTypeId == "DOC004")
        //        {
        //            uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");
        //            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        //            var filePath = Path.Combine(uploadPath, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await request.File.CopyToAsync(stream);
        //            }

        //            var otherDocument = new PhotoForestProduce
        //            {
        //                RegistrationNo = request.RegistrationNo,
        //                DocumentType = request.DocumentTypeId,
        //                OtherDocument = fileName,
        //                ApplicationId = request.ApplicationId,
        //                CreatedDate = DateTime.Now,
        //                CategoryId = request.CategoryId
        //            };

        //            _context.PhotoForestProduces.Add(otherDocument);
        //            fileNames.Add(fileName);
        //        }

        //        await _context.SaveChangesAsync();

        //        return Ok(new DocumentResponse
        //        {
        //            Success = true,
        //            Message = "Documents added successfully",
        //            Data = new { fileNames = fileNames.ToArray() }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new DocumentResponse
        //        {
        //            Success = false,
        //            Message = $"Error adding documents: {ex.Message}"
        //        });
        //    }
        //}

        [HttpPost("AddDocuments")]
        public async Task<IActionResult> AddDocuments([FromForm] DocumentDto request)
        {
            try
            {
                // Check if document already exists
                var existingDoc = await _context.PhotoForestProduces
                    .FirstOrDefaultAsync(p => p.RegistrationNo == request.RegistrationNo
                        && p.DocumentType == request.DocumentTypeId);

                if (existingDoc != null)
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = "Document already exists. Use update instead."
                    });
                }

                var fileNames = new List<string>();
                var uploadPath = "";

                // Get the base path - use WebRootPath if available, otherwise use ContentRootPath
                string basePath;
                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    // Fallback to ContentRootPath and create wwwroot directory
                    basePath = Path.Combine(_environment.ContentRootPath, "wwwroot");

                    // Ensure wwwroot directory exists
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                }
                else
                {
                    basePath = _environment.WebRootPath;
                }

                if (request.DocumentTypeId == "DOC001")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "photos");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Handle multiple files for DOC001
                    if (request.Files != null && request.Files.Count > 0)
                    {
                        foreach (var file in request.Files)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            fileNames.Add(fileName);
                        }
                    }
                    else
                    {
                        return BadRequest(new DocumentResponse
                        {
                            Success = false,
                            Message = "No files provided for DOC001"
                        });
                    }

                    var photoDocument = new PhotoForestProduce
                    {
                        RegistrationNo = request.RegistrationNo,
                        DocumentType = request.DocumentTypeId,
                        PhotoUpload = string.Join(",", fileNames),
                        ApplicationId = request.ApplicationId,
                        CreatedDate = DateTime.Now,
                        CategoryId = request.CategoryId,
                        SourceType = "web"
                    };

                    _context.PhotoForestProduces.Add(photoDocument);
                }
                else if (request.DocumentTypeId == "DOC004")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "documents");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    if (request.File == null || request.File.Length == 0)
                    {
                        return BadRequest(new DocumentResponse
                        {
                            Success = false,
                            Message = "No file provided for DOC004"
                        });
                    }

                    var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }

                    var otherDocument = new PhotoForestProduce
                    {
                        RegistrationNo = request.RegistrationNo,
                        DocumentType = request.DocumentTypeId,
                        OtherDocument = fileName,
                        ApplicationId = request.ApplicationId,
                        CreatedDate = DateTime.Now,
                        CategoryId = request.CategoryId,
                        SourceType = "web"
                    };

                    _context.PhotoForestProduces.Add(otherDocument);
                    fileNames.Add(fileName);
                }
                else
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = $"Invalid document type: {request.DocumentTypeId}"
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Documents added successfully",
                    Data = new { fileNames = fileNames.ToArray() }
                });
            }
            catch (Exception ex)
            {
                // Log the complete error for debugging
                Console.WriteLine($"Error adding documents: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error adding documents: {ex.Message}"
                });
            }
        }

        [HttpPost("UpdateDocuments")]
        public async Task<IActionResult> UpdateDocuments([FromForm] DocumentDto request)
        {
            try
            {
                var existingDoc = await _context.PhotoForestProduces
                    .FirstOrDefaultAsync(p => p.RegistrationNo == request.RegistrationNo
                        && p.DocumentType == request.DocumentTypeId);

                if (existingDoc == null)
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = "Document not found. Use add instead."
                    });
                }

                var fileNames = new List<string>();
                var uploadPath = "";

                if (request.DocumentTypeId == "DOC001")
                {
                    uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "photos");

                    // Delete old files
                    if (!string.IsNullOrEmpty(existingDoc.PhotoUpload))
                    {
                        var oldFiles = existingDoc.PhotoUpload.Split(',');
                        foreach (var oldFile in oldFiles)
                        {
                            var oldFilePath = Path.Combine(uploadPath, oldFile.Trim());
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }

                    // Upload new files
                    foreach (var file in request.Files)
                    {
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        fileNames.Add(fileName);
                    }

                    existingDoc.PhotoUpload = string.Join(",", fileNames);
                    existingDoc.UpdatedDate = DateTime.Now;
                }
                else if (request.DocumentTypeId == "DOC004")
                {
                    uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");

                    // Delete old file
                    if (!string.IsNullOrEmpty(existingDoc.OtherDocument))
                    {
                        var oldFilePath = Path.Combine(uploadPath, existingDoc.OtherDocument);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Upload new file
                    var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }

                    existingDoc.OtherDocument = fileName;
                    existingDoc.UpdatedDate = DateTime.Now;
                    fileNames.Add(fileName);
                }

                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Documents updated successfully",
                    Data = new { fileNames = fileNames.ToArray() }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error updating documents: {ex.Message}"
                });
            }
        }

        [HttpDelete("DeleteDocument")]
        public async Task<IActionResult> DeleteDocument([FromQuery] string registrationNo, [FromQuery] string documentTypeId)
        {
            try
            {
                var document = await _context.PhotoForestProduces
                    .FirstOrDefaultAsync(p => p.RegistrationNo == registrationNo
                        && p.DocumentType == documentTypeId);

                if (document == null)
                {
                    return NotFound(new DocumentResponse
                    {
                        Success = false,
                        Message = "Document not found"
                    });
                }

                // Delete physical files
                if (documentTypeId == "DOC001" && !string.IsNullOrEmpty(document.PhotoUpload))
                {
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "photos");
                    var files = document.PhotoUpload.Split(',');
                    foreach (var file in files)
                    {
                        var filePath = Path.Combine(uploadPath, file.Trim());
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
                else if (documentTypeId == "DOC004" && !string.IsNullOrEmpty(document.OtherDocument))
                {
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");
                    var filePath = Path.Combine(uploadPath, document.OtherDocument);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.PhotoForestProduces.Remove(document);
                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Document deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error deleting document: {ex.Message}"
                });
            }
        }

        [HttpPost("AddDocument")]
        public async Task<IActionResult> AddDocument([FromForm] DocumentDto request)
        {
            try
            {
                // Check if document already exists
                var existingDoc = await _context.PhotoForestProduces
                    .FirstOrDefaultAsync(p => p.RegistrationNo == request.RegistrationNo
                        && p.DocumentType == request.DocumentTypeId);

                if (existingDoc != null)
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = "Document already exists. Use update instead."
                    });
                }

                var fileNames = new List<string>();
                var uploadPath = "";

                // Get the base path
                string basePath;
                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    basePath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                }
                else
                {
                    basePath = _environment.WebRootPath;
                }

                if (request.DocumentTypeId == "DOC001")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "photos");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Handle multiple files for DOC001
                    if (request.Files != null && request.Files.Count > 0)
                    {
                        foreach (var file in request.Files)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            fileNames.Add(fileName);
                        }
                    }
                    else
                    {
                        return BadRequest(new DocumentResponse
                        {
                            Success = false,
                            Message = "No files provided for DOC001"
                        });
                    }

                    var photoDocument = new PhotoForestProduce
                    {
                        RegistrationNo = request.RegistrationNo,
                        DocumentType = request.DocumentTypeId,
                        PhotoUpload = string.Join(",", fileNames),
                        ApplicationId = request.ApplicationId,
                        CreatedDate = DateTime.Now,
                        CategoryId = request.CategoryId,
                        SourceType = "web"
                    };

                    _context.PhotoForestProduces.Add(photoDocument);
                }
                else if (request.DocumentTypeId == "DOC004")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "documents");

                    // Ensure directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    if (request.File == null || request.File.Length == 0)
                    {
                        return BadRequest(new DocumentResponse
                        {
                            Success = false,
                            Message = "No file provided for DOC004"
                        });
                    }

                    var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }

                    var otherDocument = new PhotoForestProduce
                    {
                        RegistrationNo = request.RegistrationNo,
                        DocumentType = request.DocumentTypeId,
                        OtherDocument = fileName,
                        ApplicationId = request.ApplicationId,
                        CreatedDate = DateTime.Now,
                        CategoryId = request.CategoryId,
                        SourceType = "web"
                    };

                    _context.PhotoForestProduces.Add(otherDocument);
                    fileNames.Add(fileName);
                }
                else
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = $"Invalid document type: {request.DocumentTypeId}"
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Document added successfully",
                    Data = new { fileNames = fileNames.ToArray() }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding document: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error adding document: {ex.Message}"
                });
            }
        }

        [HttpPost("UpdateDocument")]
        public async Task<IActionResult> UpdateDocument([FromForm] DocumentDto request)
        {
            try
            {
                var existingDoc = await _context.PhotoForestProduces
                    .FirstOrDefaultAsync(p => p.RegistrationNo == request.RegistrationNo
                        && p.DocumentType == request.DocumentTypeId);

                if (existingDoc == null)
                {
                    return BadRequest(new DocumentResponse
                    {
                        Success = false,
                        Message = "Document not found. Use add instead."
                    });
                }

                var fileNames = new List<string>();
                var uploadPath = "";

                // Get the base path
                string basePath;
                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    basePath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                }
                else
                {
                    basePath = _environment.WebRootPath;
                }

                if (request.DocumentTypeId == "DOC001")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "photos");

                    // Delete old files
                    if (!string.IsNullOrEmpty(existingDoc.PhotoUpload))
                    {
                        var oldFiles = existingDoc.PhotoUpload.Split(',');
                        foreach (var oldFile in oldFiles)
                        {
                            var oldFilePath = Path.Combine(uploadPath, oldFile.Trim());
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }

                    // Upload new files
                    if (request.Files != null && request.Files.Count > 0)
                    {
                        foreach (var file in request.Files)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            fileNames.Add(fileName);
                        }
                    }

                    existingDoc.PhotoUpload = string.Join(",", fileNames);
                    existingDoc.UpdatedDate = DateTime.Now;
                }
                else if (request.DocumentTypeId == "DOC004")
                {
                    uploadPath = Path.Combine(basePath, "uploads", "documents");

                    // Delete old file
                    if (!string.IsNullOrEmpty(existingDoc.OtherDocument))
                    {
                        var oldFilePath = Path.Combine(uploadPath, existingDoc.OtherDocument);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Upload new file
                    if (request.File != null && request.File.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.File.CopyToAsync(stream);
                        }

                        existingDoc.OtherDocument = fileName;
                        existingDoc.UpdatedDate = DateTime.Now;
                        fileNames.Add(fileName);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new DocumentResponse
                {
                    Success = true,
                    Message = "Document updated successfully",
                    Data = new { fileNames = fileNames.ToArray() }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating document: {ex.Message}");
                return StatusCode(500, new DocumentResponse
                {
                    Success = false,
                    Message = $"Error updating document: {ex.Message}"
                });
            }
        }

        [HttpPost("add-vehicle-details")]
        public async Task<IActionResult> AddVehicleDetails([FromForm] SaveVehicleDetailsRequestDto request)
        {
            try
            {
                _logger.LogInformation("Adding vehicle details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);

                // Validate driver license number
                //if (!IsValidDriverLicense(request.DriverLicenseNo))
                //{
                //    return BadRequest(new VehicleDetailsResponseDto
                //    {
                //        Success = false,
                //        Message = "Invalid driver license number format."
                //    });
                //}

                // Validate vehicle number
                if (!IsValidVehicleNumber(request.VehicleNo))
                {
                    return BadRequest(new VehicleDetailsResponseDto
                    {
                        Success = false,
                        Message = "Invalid vehicle number format. Format should be: DL1A1234 or DL12ABC1234"
                    });
                }

                // Check if vehicle details already exist for this registration
                var existingDetails = await _context.TransportDetails
                    .FirstOrDefaultAsync(td => td.RegistrationNo == request.RegistrationNo);

                if (existingDetails != null)
                {
                    return BadRequest(new VehicleDetailsResponseDto
                    {
                        Success = false,
                        Message = "Vehicle details already exist for this registration. Use update instead."
                    });
                }

                var result = await _applicationService.SaveVehicleDetailsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vehicle details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);
                return BadRequest(new VehicleDetailsResponseDto
                {
                    Success = false,
                    Message = $"Error adding vehicle details: {ex.Message}"
                });
            }
        }

        [HttpPost("update-vehicle-details/{tpId}")]
        public async Task<IActionResult> UpdateVehicleDetails(int tpId, [FromForm] SaveVehicleDetailsRequestDto request)
        {
            try
            {
                _logger.LogInformation("Updating vehicle details for TPId: {TPId}", tpId);

                // Validate driver license number
                if (!IsValidDriverLicense(request.DriverLicenseNo))
                {
                    return BadRequest(new VehicleDetailsResponseDto
                    {
                        Success = false,
                        Message = "Invalid driver license number format."
                    });
                }

                // Validate vehicle number
                if (!IsValidVehicleNumber(request.VehicleNo))
                {
                    return BadRequest(new VehicleDetailsResponseDto
                    {
                        Success = false,
                        Message = "Invalid vehicle number format. Format should be: DL1A1234 or DL12ABC1234"
                    });
                }

                var result = await _applicationService.UpdateVehicleDetailsAsync(tpId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle details for TPId: {TPId}", tpId);
                return BadRequest(new VehicleDetailsResponseDto
                {
                    Success = false,
                    Message = $"Error updating vehicle details: {ex.Message}"
                });
            }
        }

        [HttpGet("vehicle-details/{registrationNo}")]
        public async Task<IActionResult> GetVehicleDetails(string registrationNo)
        {
            try
            {
                _logger.LogInformation("Getting vehicle details for RegistrationNo: {RegistrationNo}", registrationNo);

                var vehicleDetails = await _context.TransportDetails
                    .FirstOrDefaultAsync(td => td.RegistrationNo == registrationNo);

                if (vehicleDetails == null)
                {
                    return NotFound(new VehicleDetailsResponseDto
                    {
                        Success = false,
                        Message = "Vehicle details not found"
                    });
                }

                var dto = new VehicleDetailsDto
                {
                    TPId = vehicleDetails.TPId,
                    RegistrationNo = vehicleDetails.RegistrationNo,
                    TransportId = vehicleDetails.TransportId ?? 0,
                    DriverName = vehicleDetails.DriverName,
                    DriverLicenseNo = vehicleDetails.DriverLicenceNo,
                    VehicleNo = vehicleDetails.VehicleNo,
                    VehicleOwnerName = vehicleDetails.VehicleOwnerName,
                    VehiclePhotograph = vehicleDetails.VehiclePhotograph,
                    CreatedDate = vehicleDetails.CreatedDate ?? DateTime.UtcNow
                };

                return Ok(new VehicleDetailsResponseDto
                {
                    Success = true,
                    Message = "Vehicle details retrieved successfully",
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle details for RegistrationNo: {RegistrationNo}", registrationNo);
                return BadRequest(new VehicleDetailsResponseDto
                {
                    Success = false,
                    Message = $"Error getting vehicle details: {ex.Message}"
                });
            }
        }

        [HttpGet("check-vehicle-details/{registrationNo}")]
        public async Task<IActionResult> CheckVehicleDetailsExists(string registrationNo)
        {
            try
            {
                var exists = await _context.TransportDetails
                    .AnyAsync(td => td.RegistrationNo == registrationNo);

                return Ok(new
                {
                    Success = true,
                    Data = new { exists }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking vehicle details for RegistrationNo: {RegistrationNo}", registrationNo);
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Error checking vehicle details: {ex.Message}"
                });
            }
        }

        [HttpDelete("delete-vehicle-details/{tpId}")]
        public async Task<IActionResult> DeleteVehicleDetails(int tpId)
        {
            try
            {
                var vehicleDetails = await _context.TransportDetails.FindAsync(tpId);

                if (vehicleDetails == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Vehicle details not found"
                    });
                }

                // Delete the vehicle photo file if it exists
                if (!string.IsNullOrEmpty(vehicleDetails.VehiclePhotograph))
                {
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-photos");
                    var filePath = Path.Combine(uploadPath, vehicleDetails.VehiclePhotograph);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.TransportDetails.Remove(vehicleDetails);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Vehicle details deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle details for TPId: {TPId}", tpId);
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Error deleting vehicle details: {ex.Message}"
                });
            }
        }

        // Helper methods for validation
        private bool IsValidDriverLicense(string licenseNo)
        {
            if (string.IsNullOrEmpty(licenseNo))
                return false;

            var patterns = new[]
            {
        new Regex(@"^(([A-Za-z]{2}[0-9]{2})( )|([A-Za-z]{2}-[0-9]{2}))((19|20)[0-9][0-9])[0-9]{7}$", RegexOptions.IgnoreCase),
        new Regex(@"^[A-Za-z]{2}\d{14}$", RegexOptions.IgnoreCase),
        new Regex(@"^[A-Za-z]{2} \d{14}$", RegexOptions.IgnoreCase),
        new Regex(@"^DL/[A-Z]/[A-Z]{2}/\d{4}/\d{2}-\d{2}$", RegexOptions.IgnoreCase),
        new Regex(@"^[A-Z]{2}\d{2}[A-Z]\d{11}$", RegexOptions.IgnoreCase),
        new Regex(@"^[A-Z]/[A-Z]{2}/\d{2}-[A-Z]/\d{6}/\d{4}$", RegexOptions.IgnoreCase),
        new Regex(@"^\d{2}/\d{4}/\d{4}$", RegexOptions.IgnoreCase)
    };

            return patterns.Any(pattern => pattern.IsMatch(licenseNo));
        }

        private bool IsValidVehicleNumber(string vehicleNo)
        {
            if (string.IsNullOrEmpty(vehicleNo))
                return false;

            var pattern = new Regex(@"^[A-Z]{2}[0-9]{1,2}[A-Z]{1,2}[0-9]{4}$", RegexOptions.IgnoreCase);
            return pattern.IsMatch(vehicleNo);
        }

        // Add these methods to ApplyTpNocController class

        [HttpPost("save-route-details")]
        public async Task<IActionResult> SaveRouteDetails([FromBody] SaveRouteDetailsRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving route details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);

                // Validate required fields
                if (string.IsNullOrEmpty(request.RegistrationNo))
                {
                    return BadRequest(new RouteDetailsResponseDto
                    {
                        Success = false,
                        Message = "Registration number is required"
                    });
                }

                var result = await _applicationService.SaveRouteDetailsAsync(request);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving route details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);
                return StatusCode(500, new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPut("update-route-details/{routeId}")]
        public async Task<IActionResult> UpdateRouteDetails(int routeId, [FromBody] SaveRouteDetailsRequestDto request)
        {
            try
            {
                _logger.LogInformation("Updating route details for RouteId: {RouteId}", routeId);

                // Validate required fields
                if (string.IsNullOrEmpty(request.RegistrationNo))
                {
                    return BadRequest(new RouteDetailsResponseDto
                    {
                        Success = false,
                        Message = "Registration number is required"
                    });
                }

                var result = await _applicationService.UpdateRouteDetailsAsync(routeId, request);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route details for RouteId: {RouteId}", routeId);
                return StatusCode(500, new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("route-details/{registrationNo}")]
        public async Task<IActionResult> GetRouteDetails(string registrationNo)
        {
            try
            {
                _logger.LogInformation("Getting route details for RegistrationNo: {RegistrationNo}", registrationNo);

                var routeDetails = await _applicationService.GetRouteDetailsAsync(registrationNo);

                if (routeDetails == null)
                {
                    return NotFound(new RouteDetailsResponseDto
                    {
                        Success = false,
                        Message = "Route details not found"
                    });
                }

                return Ok(new RouteDetailsResponseDto
                {
                    Success = true,
                    Message = "Route details retrieved successfully",
                    Data = routeDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting route details for RegistrationNo: {RegistrationNo}", registrationNo);
                return StatusCode(500, new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("check-route-details/{registrationNo}")]
        public async Task<IActionResult> CheckRouteDetailsExists(string registrationNo)
        {
            try
            {
                var exists = await _applicationService.CheckRouteDetailsExistsAsync(registrationNo);

                if (exists)
                {
                    var routeDetails = await _applicationService.GetRouteDetailsAsync(registrationNo);
                    return Ok(new
                    {
                        Success = true,
                        Data = new CheckRouteDetailsExistsResponse
                        {
                            Exists = true,
                            Details = routeDetails
                        }
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = new CheckRouteDetailsExistsResponse
                    {
                        Exists = false,
                        Details = null
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking route details for RegistrationNo: {RegistrationNo}", registrationNo);
                return StatusCode(500, new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpDelete("delete-route-details/{routeId}")]
        public async Task<IActionResult> DeleteRouteDetails(int routeId)
        {
            try
            {
                var deleted = await _applicationService.DeleteRouteDetailsAsync(routeId);

                if (deleted)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Route details deleted successfully"
                    });
                }

                return NotFound(new
                {
                    Success = false,
                    Message = "Route details not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting route details for RouteId: {RouteId}", routeId);
                return StatusCode(500, new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("submit-application")]
        public async Task<IActionResult> SubmitApplication([FromBody] SubmitApplicationRequestDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new SubmitApplicationResponseDto { Success = false, Message = "Request cannot be null." });

                if (!request.ConsentConfirmed)
                    return BadRequest(new SubmitApplicationResponseDto { Success = false, Message = "Consent not confirmed." });

                var result = await _applicationService.SubmitApplicationAsync(request);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SubmitApplication");
                return StatusCode(500, new SubmitApplicationResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("update-application-status")]
        public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusRequestDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new UpdateApplicationStatusResponseDto { Success = false, Message = "Request cannot be null." });

                var result = await _applicationService.UpdateApplicationStatusAsync(request);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateApplicationStatus");
                return StatusCode(500, new UpdateApplicationStatusResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

    }
}
