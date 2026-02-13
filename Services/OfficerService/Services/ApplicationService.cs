using Azure.Core;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DtoModels.Enums;
using OfficerService.Models;
using OfficerService.Repositories;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IWebHostEnvironment _environment;

        public ApplicationService(IApplicationRepository applicationRepository, AppDbContext context, ILogger<ApplicationService> logger, IWebHostEnvironment environment)
        {
            _applicationRepository = applicationRepository;
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public async Task<ApplicationResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request)
        {
            var application = await _applicationRepository.CreateApplicationAsync(request);
            return await MapToApplicationResponseDto(application);
        }

        public async Task<ApplicationResponseDto?> GetApplicationAsync(long applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            return application != null ? await MapToApplicationResponseDto(application) : null;
        }

        public async Task<List<ApplicationResponseDto>> GetUserApplicationsAsync(string userId)
        {
            return await _applicationRepository.GetApplicationsByUserAsync(userId);
        }

        private async Task<ApplicationResponseDto> MapToApplicationResponseDto(ApplicationMaster application)
        {
            return new ApplicationResponseDto
            {
                ApplicationId = application.ApplicationId,
                StateId = application.StateId ?? 0,
                StateName = application.State?.StateName,
                DistrictId = application.DistrictId ?? 0,
                DistrictName = application.District?.DistName,
                SubDistrictId = application.SubDistrictId,
                SubDistrictName = application.SubDistrict?.SubDistName,
                ForestProduceId = application.ForestProduceId ?? 0,
                ForestProduceName = application.ForestProduce?.Name,
                CreatedDate = application.CreatedDate ?? DateTime.UtcNow,
                Status = application.Status == true ? "Active" : "Inactive"
            };
        }

        public async Task<List<OpenApplicationDto>> GetOpenUserApplicationsAsync(string userId)
        {
            return await _applicationRepository.GetOpenApplicationsByUserAsync(userId);
        }

        // OfficerService/Services/ApplicationService.cs
        public async Task<List<RegisteredTpResponseDto>> GetRegisteredApplicationsAsync(string userId)
        {
            return await _applicationRepository.GetRegisteredApplicationsByUserAsync(userId);
        }

        public async Task<ApplicationMaster?> GetApplicationByIdAsync(long applicationId)
        {
            return await _applicationRepository.GetApplicationByIdAsync(applicationId);
        }

        public async Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId)
        {
            return await _applicationRepository.GetApplicationByIdAsyncs(applicationId);
        }

        public async Task<string> GenerateRegistrationNoAsync(long applicationId, int applicationCategoryId)
        {
            try
            {
                // Get application with state information
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (application?.State == null)
                    throw new Exception("Application or State not found");

                // Safely get state code
                var stateCode = application.State.StateCode.ToString();
                var currentYear = DateTime.UtcNow.Year;

                // Get last registration for this state + year + application category
                var lastRegistration = await _context.ApplicationDetails
                    .Where(ad => ad.RegistrationNo != null &&
                                ad.RegistrationNo.StartsWith($"{stateCode}{currentYear}") &&
                                ad.ApplicationCateogryId == applicationCategoryId)
                    .OrderByDescending(ad => ad.RegistrationNo)
                    .FirstOrDefaultAsync();

                int sequenceNumber = 1000; // Start from 1000

                if (lastRegistration != null && !string.IsNullOrEmpty(lastRegistration.RegistrationNo))
                {
                    // Extract sequence number from registration number
                    var prefixLength = stateCode.Length + 4; // state code + year
                    if (lastRegistration.RegistrationNo.Length > prefixLength)
                    {
                        var lastNumberStr = lastRegistration.RegistrationNo.Substring(prefixLength);

                        if (int.TryParse(lastNumberStr, out int lastNumber))
                        {
                            sequenceNumber = lastNumber + 1;
                        }
                    }
                }

                return $"{stateCode}{currentYear}{sequenceNumber:D4}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating registration number for ApplicationId: {ApplicationId}", applicationId);
                throw new Exception($"Failed to generate registration number: {ex.Message}", ex);
            }
        }

        public async Task<ProduceDetailResponseDto> AddProduceDetailsAsyncs(AddProduceDetailRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Starting to add produce details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    request.ApplicationId, request.ForestProduceId);

                // Get application with state information first
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

                if (application?.State == null)
                    throw new Exception("Application or State not found");

                int applicationCategoryId = 0;

                foreach (var detail in request.ProduceDetails)
                {
                    ParseDetailStringValues(detail);

                    applicationCategoryId = await DetermineApplicationsCategory(
                        detail.SpeciesId,
                        application.StateId ?? throw new Exception("Application StateId is null"));

                    _logger.LogInformation("ForestProduceId: {ForestProduceId} mapped to ApplicationCategoryId: {ApplicationCategoryId}",
                        request.ForestProduceId, applicationCategoryId);
                }

                // Check if this category already has a registration number for this application
                var existingApplicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad =>
                        ad.ApplicationId == request.ApplicationId &&
                        ad.ApplicationCateogryId == applicationCategoryId);

                ApplicationDetail applicationDetail;
                string registrationNo;

                if (existingApplicationDetail != null)
                {
                    _logger.LogInformation("Existing ApplicationDetail found for category {CategoryId}, reusing RegistrationNo: {RegistrationNo}",
                        applicationCategoryId, existingApplicationDetail.RegistrationNo);
                    applicationDetail = existingApplicationDetail;
                    registrationNo = applicationDetail.RegistrationNo;
                }
                else
                {
                    _logger.LogInformation("Creating new ApplicationDetail for category {CategoryId}", applicationCategoryId);

                    // Generate unique registration number for this category
                    registrationNo = await GenerateUniqueRegistrationNoForCategoryAsync(
                        request.ApplicationId,
                        applicationCategoryId,
                        application.State.StateCode);

                    // 🔥 **CRITICAL FIX**: Check if the generated number already exists
                    var existsInDb = await _context.ApplicationDetails
                        .AnyAsync(ad => ad.RegistrationNo == registrationNo);

                    if (existsInDb)
                    {
                        // If it exists, generate a new one
                        registrationNo = await GenerateNextAvailableRegistrationNoAsync(
                            application.State.StateCode,
                            registrationNo);
                    }

                    applicationDetail = new ApplicationDetail
                    {
                        ApplicationId = request.ApplicationId,
                        ApplicationCateogryId = applicationCategoryId,
                        RegistrationNo = registrationNo,
                        CreatedDate = DateTime.UtcNow,
                        CreateByUserId = request.UserId
                    };

                    _context.ApplicationDetails.Add(applicationDetail);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("ApplicationDetail created with ID: {Id} and RegistrationNo: {RegistrationNo}",
                        applicationDetail.Id, registrationNo);
                }

                var response = new ProduceDetailResponseDto
                {
                    Success = true,
                    Message = "Produce details processed successfully",
                    RegistrationNo = registrationNo,
                    ApplicationDetailId = applicationDetail.Id
                };

                // Process each produce detail
                foreach (var detail in request.ProduceDetails)
                {
                    if (detail.IsDeleted && detail.SpeciesLogId.HasValue)
                    {
                        await DeleteSpeciesLogAsync(detail.SpeciesLogId.Value, request.ForestProduceId);
                        _logger.LogInformation("Deleted species log: {SpeciesLogId}", detail.SpeciesLogId.Value);
                    }
                    else if (detail.SpeciesLogId.HasValue)
                    {
                        var updatedLogId = await UpdateSpeciesLogAsync(request.ForestProduceId, detail.SpeciesLogId.Value, detail);
                        response.SavedLogs.Add(new SpeciesLogResponse
                        {
                            SpeciesLogId = updatedLogId,
                            SpeciesId = detail.SpeciesId,
                            TemporaryId = detail.TemporaryId
                        });
                    }
                    else
                    {
                        var newLogId = await SaveSpeciesLogAsync(request.ForestProduceId, registrationNo, detail, request);
                        response.SavedLogs.Add(new SpeciesLogResponse
                        {
                            SpeciesLogId = newLogId,
                            SpeciesId = detail.SpeciesId,
                            TemporaryId = detail.TemporaryId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving produce details for ApplicationId: {ApplicationId}", request.ApplicationId);
                throw new Exception($"Failed to save produce details: {ex.Message}", ex);
            }
        }

        private async Task<string> GenerateUniqueRegistrationNoForCategoryAsync(long applicationId,int applicationCategoryId,string stateCode)
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;

                // Check if this category already has a registration number in this application
                var existingForCategory = await _context.ApplicationDetails
                    .Where(ad => ad.ApplicationId == applicationId &&
                                ad.ApplicationCateogryId == applicationCategoryId)
                    .Select(ad => ad.RegistrationNo)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(existingForCategory))
                {
                    return existingForCategory;
                }

                // Get the next available sequence for this state and year
                return await GetNextRegistrationNumberAsync(stateCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating unique registration number for ApplicationId: {ApplicationId}, Category: {CategoryId}",
                    applicationId, applicationCategoryId);
                throw;
            }
        }

        // New method to get next available registration number
        private async Task<string> GetNextRegistrationNumberAsync(string stateCode)
        {
            var currentYear = DateTime.UtcNow.Year;
            var basePrefix = $"{stateCode}{currentYear}";

            // Get all registration numbers for this state and year
            var existingNumbers = await _context.ApplicationDetails
                .Where(ad => ad.RegistrationNo != null &&
                            ad.RegistrationNo.StartsWith(basePrefix))
                .Select(ad => ad.RegistrationNo)
                .ToListAsync();

            // Find the next available sequence
            int nextSequence = 1000;

            if (existingNumbers.Any())
            {
                // Extract sequence numbers
                var sequences = new List<int>();
                foreach (var number in existingNumbers)
                {
                    if (number.Length > basePrefix.Length)
                    {
                        var sequencePart = number.Substring(basePrefix.Length);
                        if (int.TryParse(sequencePart, out int seq))
                        {
                            sequences.Add(seq);
                        }
                    }
                }

                if (sequences.Any())
                {
                    nextSequence = sequences.Max() + 1;
                }
            }

            return $"{basePrefix}{nextSequence:D4}";
        }

        // New method to find next available number if generated one already exists
        private async Task<string> GenerateNextAvailableRegistrationNoAsync(string stateCode, string existingNumber)
        {
            var currentYear = DateTime.UtcNow.Year;
            var basePrefix = $"{stateCode}{currentYear}";

            // Extract sequence from existing number
            int currentSequence = 1000;
            if (existingNumber.Length > basePrefix.Length)
            {
                var sequencePart = existingNumber.Substring(basePrefix.Length);
                int.TryParse(sequencePart, out currentSequence);
            }

            // Get all existing registration numbers
            var existingNumbers = await _context.ApplicationDetails
                .Where(ad => ad.RegistrationNo != null &&
                            ad.RegistrationNo.StartsWith(basePrefix))
                .Select(ad => ad.RegistrationNo)
                .ToListAsync();

            // Find the next available sequence starting from currentSequence
            var sequences = new HashSet<int>();
            foreach (var number in existingNumbers)
            {
                if (number.Length > basePrefix.Length)
                {
                    var sequencePart = number.Substring(basePrefix.Length);
                    if (int.TryParse(sequencePart, out int seq))
                    {
                        sequences.Add(seq);
                    }
                }
            }

            // Find next available number
            int nextSequence = currentSequence;
            while (sequences.Contains(nextSequence))
            {
                nextSequence++;

                // Safety check to prevent infinite loop
                if (nextSequence > 9999)
                {
                    throw new Exception("No available registration numbers for this state and year");
                }
            }

            return $"{basePrefix}{nextSequence:D4}";
        }

        // Update the update method as well
        private async Task<long> SaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail, AddProduceDetailRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating species log entry for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}, SpeciesMappingId: {SpeciesMappingId}",
                    forestProduceId, registrationNo, detail.SpeciesId, detail.SpeciesMappingId);

                switch (forestProduceId)
                {
                    case 1:  // Round Timber
                        var roundTimberLog = new SpeciesLogsRoundTimber
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            SpeciesMappingId = detail.SpeciesMappingId, // Add this
                            ApplicationId = request?.ApplicationId,
                            ForestProduceId = detail.ForestProduceId,
                            LogsNo = detail.NoOfLogs ?? 1,
                            Girth = detail.MiddleGirthCm ?? 0.01m,
                            Length = detail.LengthCm ?? 0.01m,
                            Quantity = detail.Quantity ?? 0.01m,
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsRoundTimbers.Add(roundTimberLog);
                        await _context.SaveChangesAsync();
                        return roundTimberLog.Id;

                    case 2: // Bamboo 
                        var bambooLog = new SpeciesLogsBamboo
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            SpeciesMappingId = detail.SpeciesMappingId, // Add this
                            ApplicationId = request?.ApplicationId,
                            ForestProduceId = detail.ForestProduceId,
                            GirthClass = detail.GirthClass ?? 0.01m,
                            Length = detail.Length ?? 0.01m,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "n",
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsBamboos.Add(bambooLog);
                        await _context.SaveChangesAsync();
                        return bambooLog.Id;

                    case 3: // Fuelwood
                        var fuelwoodLog = new SpeciesLogsFuelwood
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            SpeciesMappingId = detail.SpeciesMappingId, // Add this
                            ForestProduceId = detail.ForestProduceId,
                            ApplicationId = request?.ApplicationId,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "t",
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsFuelwoods.Add(fuelwoodLog);
                        await _context.SaveChangesAsync();
                        return fuelwoodLog.Id;

                    case 4: // Minor Forest Produce
                        var minorLog = new SpeciesLogsMinorForestProduce
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            SpeciesMappingId = detail.SpeciesMappingId, // Add this
                            ForestProduceId = detail.ForestProduceId,
                            ApplicationId = request?.ApplicationId,
                            PlantPartID = detail.PlantPartID ?? 1,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "k",
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsMinorForestProduces.Add(minorLog);
                        await _context.SaveChangesAsync();
                        return minorLog.Id;

                    case 5: // Sawn Timber
                        var sawnTimberLog = new SpeciesLogsSawnTimber
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            SpeciesMappingId = detail.SpeciesMappingId, // Add this
                            ForestProduceId = detail.ForestProduceId,
                            ApplicationId = request?.ApplicationId,
                            LogsNo = detail.NoOfPieces ?? 1,
                            Girth = 0.01m,
                            Length = detail.LengthCm ?? 0.01m,
                            Width = detail.Width ?? 0.01m,
                            Thickness = detail.Thickness ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "c",
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsSawnTimbers.Add(sawnTimberLog);
                        await _context.SaveChangesAsync();
                        return sawnTimberLog.Id;

                    default:
                        throw new Exception($"Unknown forest produce ID: {forestProduceId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving species log for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}",
                    forestProduceId, registrationNo);
                throw;
            }
        }
        private async Task<long> UpdateSpeciesLogAsync(int forestProduceId, long speciesLogId, ProduceDetailDto detail)
        {
            switch (forestProduceId)
            {
                case 1: // Round Timber
                    var roundTimber = await _context.SpeciesLogsRoundTimbers.FindAsync(speciesLogId);
                    if (roundTimber != null)
                    {
                        roundTimber.SpeciesID = detail.SpeciesId;
                        roundTimber.ForestProduceId = detail.ForestProduceId; // Add this
                        roundTimber.SpeciesMappingId = detail.SpeciesMappingId; // Add this
                        roundTimber.LogsNo = detail.NoOfLogs ?? 1;
                        roundTimber.Girth = detail.MiddleGirthCm ?? 0.01m;
                        roundTimber.Length = detail.LengthCm ?? 0.01m;
                        roundTimber.Quantity = detail.Quantity ?? 0.01m;
                        roundTimber.Volume = detail.Volume ?? 0.001m;
                        _context.SpeciesLogsRoundTimbers.Update(roundTimber);
                    }
                    break;
                    // Add other cases with ForestProduceId updates...
            }

            await _context.SaveChangesAsync();
            return speciesLogId;
        }

        private async Task DeleteSpeciesLogAsync(long speciesLogId, int forestProduceId)
        {
            switch (forestProduceId)
            {
                case 1: // Round Timber
                    var roundTimber = await _context.SpeciesLogsRoundTimbers.FindAsync(speciesLogId);
                    if (roundTimber != null)
                    {
                        _context.SpeciesLogsRoundTimbers.Remove(roundTimber);
                    }
                    break;
                    // Implement other cases similarly
            }

            await _context.SaveChangesAsync();
        }

        // Helper method to map Forest Produce to Application Category using SpeciesMapping table
        private async Task<int> DetermineApplicationsCategory(int CategoryId, int stateId)
        {
            try
            {
                _logger.LogInformation("Determining application category for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                    CategoryId, stateId);

                // Query the SpeciesMapping table to get the CategoryID
                var speciesMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.SpeciesId == CategoryId &&
                        sm.StateId == stateId &&
                        sm.IsActive);

                if (speciesMapping != null)
                {
                    _logger.LogInformation("Found SpeciesMapping - CategoryID: {CategoryID} for speciesId: {speciesId}, StateId: {StateId}",
                        speciesMapping.CategoryId, CategoryId, stateId);
                    return speciesMapping.CategoryId;
                }
                else
                {
                    _logger.LogWarning("No active SpeciesMapping found for speciesId: {speciesId}, StateId: {StateId}, defaulting to Transit Pass (2)",
                        CategoryId, stateId);

                    // Default to Transit Pass if no mapping found
                    return 2; // Transit Pass
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error determining application category for speciesId: {speciesId}, StateId: {StateId}",
                    CategoryId, stateId);

                // Default to Transit Pass on error
                return 2; // Transit Pass
            }
        }

        // Helper method to map Forest Produce to Application Category using SpeciesMapping table
        private async Task<int> DetermineApplicationCategory(int CategoryId, int stateId)
        {
            try
            {
                _logger.LogInformation("Determining application category for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                    CategoryId, stateId);

                // Query the SpeciesMapping table to get the CategoryID
                var speciesMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.CategoryId == CategoryId &&
                        sm.StateId == stateId &&
                        sm.IsActive);

                if (speciesMapping != null)
                {
                    _logger.LogInformation("Found SpeciesMapping - CategoryID: {CategoryID} for speciesId: {speciesId}, StateId: {StateId}",
                        speciesMapping.CategoryId, CategoryId, stateId);
                    return speciesMapping.CategoryId;
                }
                else
                {
                    _logger.LogWarning("No active SpeciesMapping found for speciesId: {speciesId}, StateId: {StateId}, defaulting to Transit Pass (2)",
                        CategoryId, stateId);

                    // Default to Transit Pass if no mapping found
                    return 2; // Transit Pass
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error determining application category for speciesId: {speciesId}, StateId: {StateId}",
                    CategoryId, stateId);

                // Default to Transit Pass on error
                return 2; // Transit Pass
            }
        }

        private void ParseDetailStringValues(ProduceDetailDto detail)
        {
            try
            {
                // Parse quantity
                if (!string.IsNullOrEmpty(detail.QuantityStr) && decimal.TryParse(detail.QuantityStr, out decimal quantity))
                {
                    detail.Quantity = quantity;
                }
                else
                {
                    detail.Quantity = 0.01m; // Default value
                }

                // Parse volume
                if (!string.IsNullOrEmpty(detail.VolumeStr) && decimal.TryParse(detail.VolumeStr, out decimal volume))
                {
                    detail.Volume = volume;
                }
                else
                {
                    detail.Volume = 0.001m; // Default value
                }

                // Parse species mapping ID if provided as string (though it should be int)
                if (detail.SpeciesMappingId == 0 && !string.IsNullOrEmpty(detail.SpeciesMappingIdStr))
                {
                    if (int.TryParse(detail.SpeciesMappingIdStr, out int speciesMappingId))
                    {
                        detail.SpeciesMappingId = speciesMappingId;
                    }
                }

                // Parse bamboo specific fields
                if (!string.IsNullOrEmpty(detail.GirthClassStr) && decimal.TryParse(detail.GirthClassStr, out decimal girthClass))
                {
                    detail.GirthClass = girthClass;
                }

                if (!string.IsNullOrEmpty(detail.LengthStr) && decimal.TryParse(detail.LengthStr, out decimal length))
                {
                    detail.Length = length;
                }

                // Parse round timber specific fields
                if (!string.IsNullOrEmpty(detail.MiddleGirthCmStr) && decimal.TryParse(detail.MiddleGirthCmStr, out decimal middleGirth))
                {
                    detail.MiddleGirthCm = middleGirth;
                }

                if (!string.IsNullOrEmpty(detail.LengthCmStr) && decimal.TryParse(detail.LengthCmStr, out decimal lengthCm))
                {
                    detail.LengthCm = lengthCm;
                }

                // Parse sawn timber specific fields
                if (!string.IsNullOrEmpty(detail.WidthStr) && decimal.TryParse(detail.WidthStr, out decimal width))
                {
                    detail.Width = width;
                }

                if (!string.IsNullOrEmpty(detail.ThicknessStr) && decimal.TryParse(detail.ThicknessStr, out decimal thickness))
                {
                    detail.Thickness = thickness;
                }

                // Parse numeric string fields to integers
                if (!string.IsNullOrEmpty(detail.NoOfLogsStr) && int.TryParse(detail.NoOfLogsStr, out int noOfLogs))
                {
                    detail.NoOfLogs = noOfLogs;
                }

                if (!string.IsNullOrEmpty(detail.NoOfPiecesStr) && int.TryParse(detail.NoOfPiecesStr, out int noOfPieces))
                {
                    detail.NoOfPieces = noOfPieces;
                }

                _logger.LogInformation("Parsed detail - SpeciesId: {SpeciesId}, Quantity: {Quantity}, Volume: {Volume}",
                    detail.SpeciesId, detail.Quantity, detail.Volume);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing string values for SpeciesId: {SpeciesId}", detail.SpeciesId);
                throw;
            }
        }

        public async Task<ApplicationMaster?> UpdateApplicationAsync(UpdateApplicationRequestDto request)
        {
            return await _applicationRepository.UpdateApplicationAsync(request);
        }

        public async Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId)
        {
            return await _applicationRepository.GetSpeciesLogsByApplicationAsync(applicationId);
        }

        public async Task<ProduceDetailResponseDto> UpdateProduceDetailsAsync(UpdateProduceDetailRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating produce details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    request.ApplicationId, request.SpeciesId);

                // Get application with state information
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

                if (application?.State == null)
                    throw new Exception("Application or State not found");

                // Determine Application Category
                int applicationCategoryId = await DetermineApplicationsCategory(
                    request.SpeciesId,
                    application.StateId ?? throw new Exception("Application StateId is null"));

                // Get existing application detail
                var existingApplicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad =>
                        ad.ApplicationId == request.ApplicationId &&
                        ad.ApplicationCateogryId == applicationCategoryId);

                if (existingApplicationDetail == null)
                    throw new Exception("Application detail not found");

                string registrationNo = existingApplicationDetail.RegistrationNo;

                if (string.IsNullOrEmpty(registrationNo))
                    throw new Exception("Registration number not found");

                // Process updates and deletions
                foreach (var detail in request.ProduceDetails)
                {
                    if (detail.IsDeleted && detail.SpeciesLogId.HasValue)
                    {
                        // Delete existing record
                        await _applicationRepository.DeleteSpeciesLogAsync(detail.SpeciesLogId.Value, GetForestProduceType(request.SpeciesId));
                    }
                    else if (detail.SpeciesLogId.HasValue)
                    {
                        // Update existing record
                        await _applicationRepository.UpdateSpeciesLogAsync(detail, GetForestProduceType(request.SpeciesId));
                    }
                    else
                    {
                        // Create new record
                        await SaveSpeciesLogAsync(request.SpeciesId, registrationNo, detail, null);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated produce details for ApplicationId: {ApplicationId}", request.ApplicationId);

                return new ProduceDetailResponseDto
                {
                    Success = true,
                    Message = "Produce details updated successfully",
                    RegistrationNo = registrationNo,
                    ApplicationDetailId = existingApplicationDetail.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating produce details for ApplicationId: {ApplicationId}", request.ApplicationId);
                throw new Exception($"Failed to update produce details: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteSpeciesLogAsync(long speciesLogId, string forestProduceType)
        {
            return await _applicationRepository.DeleteSpeciesLogAsync(speciesLogId, forestProduceType);
        }

        private string GetForestProduceType(int forestProduceId)
        {
            return forestProduceId switch
            {
                1 => "RoundTimber",
                2 => "Bamboo",
                3 => "Fuelwood",
                4 => "Minor",
                5 => "SawnTimber",
                _ => "Unknown"
            };
        }

        public async Task<ApplicationDetailsDto?> GetApplicationWithSpeciesLogsAsync(long applicationId)
        {
            return await _applicationRepository.GetApplicationWithSpeciesLogsAsync(applicationId);
        }

        // Update the SaveProduceSourceAsync method
        public async Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving produce source for RegistrationNo: {RegistrationNo}, CategoryId: {CategoryId}",
                    request.RegistrationNo, request.CategoryId);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    long sourceId = 0;
                    int govDepotId = 0;
                    List<int> privateLandIds = new List<int>();
                    List<int> latLongIds = new List<int>();

                    // Get all registration numbers for this application
                    var allRegistrations = await _context.ApplicationDetails
                        .Where(ad => ad.ApplicationId == request.ApplicationId &&
                                   ad.RegistrationNo != null)
                        .Select(ad => new { ad.RegistrationNo, ad.ApplicationCateogryId })
                        .Distinct()
                        .ToListAsync();

                    // 1. Save to appropriate source place table based on category
                    if (request.CategoryId == 1) // NOC
                    {
                        var existingNocSource = await _context.NocSourcePlaces
                            .FirstOrDefaultAsync(nsp => nsp.ApplicationId == request.RegistrationNo);

                        if (existingNocSource != null)
                        {
                            // Update existing record
                            existingNocSource.StateId = request.StateId;
                            existingNocSource.CircleId = request.CircleId;
                            existingNocSource.DivisionId = request.DivisionId;
                            existingNocSource.RangeId = request.RangeId;
                            existingNocSource.Address = request.Address;
                            existingNocSource.PinCode = request.PinCode;
                            existingNocSource.UpdatedDate = DateTime.UtcNow;

                            _context.NocSourcePlaces.Update(existingNocSource);
                            sourceId = existingNocSource.SourceId;
                        }
                        else
                        {
                            // Create new record
                            var nocSourcePlace = new NocSourcePlace
                            {
                                ApplicationId = request.RegistrationNo,
                                StateId = request.StateId,
                                CircleId = request.CircleId,
                                DivisionId = request.DivisionId,
                                RangeId = request.RangeId,
                                Address = request.Address,
                                PinCode = request.PinCode,
                                CreatedDate = DateTime.UtcNow
                            };

                            _context.NocSourcePlaces.Add(nocSourcePlace);
                            await _context.SaveChangesAsync();
                            sourceId = nocSourcePlace.SourceId;
                        }
                    }
                    else if (request.CategoryId == 2) // Transit Pass
                    {
                        var existingTpSource = await _context.TpSourcePlaces
                            .FirstOrDefaultAsync(tsp => tsp.ApplicationId == request.RegistrationNo);

                        if (existingTpSource != null)
                        {
                            // Update existing record
                            existingTpSource.StateId = request.StateId;
                            existingTpSource.CircleId = request.CircleId;
                            existingTpSource.DivisionId = request.DivisionId;
                            existingTpSource.RangeId = request.RangeId;
                            existingTpSource.Address = request.Address;
                            existingTpSource.PinCode = request.PinCode;
                            existingTpSource.UpdatedDate = DateTime.UtcNow;

                            _context.TpSourcePlaces.Update(existingTpSource);
                            sourceId = existingTpSource.SourceId;
                        }
                        else
                        {
                            // Create new record
                            var tpSourcePlace = new TpSourcePlace
                            {
                                ApplicationId = request.RegistrationNo,
                                StateId = request.StateId,
                                CircleId = request.CircleId,
                                DivisionId = request.DivisionId,
                                RangeId = request.RangeId,
                                Address = request.Address,
                                PinCode = request.PinCode,
                                CreatedDate = DateTime.UtcNow
                            };

                            _context.TpSourcePlaces.Add(tpSourcePlace);
                            await _context.SaveChangesAsync();
                            sourceId = tpSourcePlace.SourceId;
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown category: {request.CategoryId}");
                    }

                    // 2. Save Government Depot if applicable
                    if (request.PlaceObtained == "government_depot" &&
                        !string.IsNullOrEmpty(request.GovernmentDepotName))
                    {
                        var existingGovDepot = await _context.GovernmentDepots
                            .FirstOrDefaultAsync(gd => gd.RegistrationNo == request.RegistrationNo &&
                                                      gd.Type == "source");

                        if (existingGovDepot != null)
                        {
                            // Update existing
                            existingGovDepot.DepotName = request.GovernmentDepotName;
                            existingGovDepot.Type = request.GovernmentDepotType;
                            existingGovDepot.SourceType = "web";
                            existingGovDepot.PlaceType = "government";
                            existingGovDepot.UpdatedDate = DateTime.UtcNow;

                            _context.GovernmentDepots.Update(existingGovDepot);
                            govDepotId = existingGovDepot.GdId;
                        }
                        else
                        {
                            // Create new
                            var governmentDepot = new GovernmentDepot
                            {
                                RegistrationNo = request.RegistrationNo,
                                DepotName = request.GovernmentDepotName,
                                Type = "source",
                                SourceType = "web",
                                PlaceType = "government",
                                CreatedDate = DateTime.UtcNow
                            };

                            _context.GovernmentDepots.Add(governmentDepot);
                            await _context.SaveChangesAsync();
                            govDepotId = governmentDepot.GdId;
                        }
                    }

                    // 3. Save Private Land if applicable
                    if (request.PlaceObtained == "private_land" &&
                        !string.IsNullOrEmpty(request.SurveyNumber))
                    {
                        // Save private land for each registration number
                        foreach (var reg in allRegistrations)
                        {
                            if (string.IsNullOrEmpty(reg.RegistrationNo)) continue;

                            var existingPrivateLand = await _context.Privatelands
                                .FirstOrDefaultAsync(pl => pl.RegistrationNo == reg.RegistrationNo &&
                                                           pl.Type == "source");

                            if (existingPrivateLand != null)
                            {
                                // Update existing
                                existingPrivateLand.SurveyNo = request.SurveyNumber;
                                existingPrivateLand.UpdatedDate = DateTime.UtcNow;

                                _context.Privatelands.Update(existingPrivateLand);
                                privateLandIds.Add(existingPrivateLand.Id);
                            }
                            else
                            {
                                // Create new
                                var privateLand = new Privateland
                                {
                                    RegistrationNo = reg.RegistrationNo,
                                    SurveyNo = request.SurveyNumber,
                                    Type = "source",
                                    CreatedDate = DateTime.UtcNow
                                };

                                _context.Privatelands.Add(privateLand);
                                await _context.SaveChangesAsync();
                                privateLandIds.Add(privateLand.Id);
                            }
                        }
                    }

                    // 4. Save Latitude/Longitude for ALL registration numbers of this application
                    if (!string.IsNullOrEmpty(request.Latitude) && !string.IsNullOrEmpty(request.Longitude))
                    {
                        foreach (var reg in allRegistrations)
                        {
                            if (string.IsNullOrEmpty(reg.RegistrationNo)) continue;

                            var existingLatLong = await _context.SourceLatLongs
                                .FirstOrDefaultAsync(sll => sll.RegistrationNo == reg.RegistrationNo);

                            if (existingLatLong != null)
                            {
                                // Update existing
                                existingLatLong.Latitude = request.Latitude;
                                existingLatLong.Longitude = request.Longitude;
                                existingLatLong.UpdatedDate = DateTime.UtcNow;

                                _context.SourceLatLongs.Update(existingLatLong);
                                latLongIds.Add(existingLatLong.LatLongId);
                            }
                            else
                            {
                                // Create new
                                var sourceLatLong = new SourceLatLong
                                {
                                    RegistrationNo = reg.RegistrationNo,
                                    Latitude = request.Latitude,
                                    Longitude = request.Longitude,
                                    CreatedDate = DateTime.UtcNow
                                };

                                _context.SourceLatLongs.Add(sourceLatLong);
                                await _context.SaveChangesAsync();
                                latLongIds.Add(sourceLatLong.LatLongId);
                            }
                        }
                    }

                    await transaction.CommitAsync();

                    return new SourceDestinationResponseDto
                    {
                        Success = true,
                        Message = "Produce source details saved successfully",
                        SourceId = sourceId,
                        GovernmentDepotId = govDepotId,
                        PrivateLandIds = privateLandIds,
                        LatLongIds = latLongIds
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error saving produce source for RegistrationNo: {RegistrationNo}",
                        request.RegistrationNo);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveProduceSourceAsync for RegistrationNo: {RegistrationNo}",
                    request.RegistrationNo);
                throw;
            }
        }

        public async Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving destination for RegistrationNo: {RegistrationNo}, CategoryId: {CategoryId}",
                    request.RegistrationNo, request.CategoryId);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    long destinationId = 0;
                    int govDepotId = 0;

                    // 1. Save to appropriate destination place table based on category
                    if (request.CategoryId == 1) // NOC
                    {
                        var existingNocDest = await _context.NocDestinationPlaces
                            .FirstOrDefaultAsync(ndp => ndp.ApplicationId == request.RegistrationNo);

                        if (existingNocDest != null)
                        {
                            // Update existing record
                            existingNocDest.StateId = request.StateId;
                            existingNocDest.CircleId = request.CircleId;
                            existingNocDest.DivisionId = request.DivisionId;
                            existingNocDest.RangeId = request.RangeId;
                            existingNocDest.Address = request.Address;
                            existingNocDest.PinCode = request.PinCode;
                            existingNocDest.UpdatedDate = DateTime.UtcNow;

                            _context.NocDestinationPlaces.Update(existingNocDest);
                            destinationId = existingNocDest.DestinationId;
                        }
                        else
                        {
                            // Create new record
                            var nocDestination = new NocDestinationPlace
                            {
                                ApplicationId = request.RegistrationNo,
                                StateId = request.StateId,
                                CircleId = request.CircleId,
                                DivisionId = request.DivisionId,
                                RangeId = request.RangeId,
                                Address = request.Address,
                                PinCode = request.PinCode,
                                CreatedDate = DateTime.UtcNow
                            };

                            _context.NocDestinationPlaces.Add(nocDestination);
                            await _context.SaveChangesAsync();
                            destinationId = nocDestination.DestinationId;
                        }
                    }
                    else if (request.CategoryId == 2) // Transit Pass
                    {
                        var existingTpDest = await _context.TpDestinationPlaces
                            .FirstOrDefaultAsync(tdp => tdp.ApplicationId == request.RegistrationNo);

                        if (existingTpDest != null)
                        {
                            // Update existing record
                            existingTpDest.StateId = request.StateId;
                            existingTpDest.CircleId = request.CircleId;
                            existingTpDest.DivisionId = request.DivisionId;
                            existingTpDest.RangeId = request.RangeId;
                            existingTpDest.Address = request.Address;
                            existingTpDest.PinCode = request.PinCode;
                            existingTpDest.UpdatedDate = DateTime.UtcNow;

                            _context.TpDestinationPlaces.Update(existingTpDest);
                            destinationId = existingTpDest.DestinationId;
                        }
                        else
                        {
                            // Create new record
                            var tpDestination = new TpDestinationPlace
                            {
                                ApplicationId = request.RegistrationNo,
                                StateId = request.StateId,
                                CircleId = request.CircleId,
                                DivisionId = request.DivisionId,
                                RangeId = request.RangeId,
                                Address = request.Address,
                                PinCode = request.PinCode,
                                CreatedDate = DateTime.UtcNow
                            };

                            _context.TpDestinationPlaces.Add(tpDestination);
                            await _context.SaveChangesAsync();
                            destinationId = tpDestination.DestinationId;
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown category: {request.CategoryId}");
                    }

                    //// 2. Save Government Depot if applicable
                    //if (request.DestinationPlace == "government_depot" &&
                    //    !string.IsNullOrEmpty(request.GovernmentDepotName))
                    //{
                    //    var existingGovDepot = await _context.GovernmentDepots
                    //        .FirstOrDefaultAsync(gd => gd.RegistrationNo == request.RegistrationNo &&
                    //                                  gd.Type == "destination");

                    //    if (existingGovDepot != null)
                    //    {
                    //        // Update existing
                    //        existingGovDepot.DepotName = request.GovernmentDepotName;
                    //        existingGovDepot.Type = request.GovernmentDepotType;
                    //        existingGovDepot.SourceType = "web";
                    //        existingGovDepot.PlaceType = "government";
                    //        existingGovDepot.UpdatedDate = DateTime.UtcNow;

                    //        _context.GovernmentDepots.Update(existingGovDepot);
                    //        govDepotId = existingGovDepot.GdId;
                    //    }
                    //    else
                    //    {
                    //        // Create new
                    //        var governmentDepot = new GovernmentDepot
                    //        {
                    //            RegistrationNo = request.RegistrationNo,
                    //            DepotName = request.GovernmentDepotName,
                    //            Type = "destination",
                    //            SourceType = "web",
                    //            PlaceType = "government",
                    //            CreatedDate = DateTime.UtcNow
                    //        };

                    //        _context.GovernmentDepots.Add(governmentDepot);
                    //        await _context.SaveChangesAsync();
                    //        govDepotId = governmentDepot.GdId;
                    //    }
                    //}

                    await transaction.CommitAsync();

                    return new SourceDestinationResponseDto
                    {
                        Success = true,
                        Message = "Destination details saved successfully",
                        DestinationId = destinationId,
                        GovernmentDepotId = govDepotId
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error saving destination for RegistrationNo: {RegistrationNo}",
                        request.RegistrationNo);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveDestinationAsync for RegistrationNo: {RegistrationNo}",
                    request.RegistrationNo);
                throw;
            }
        }
        public async Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(long applicationId, int categoryId)
        {
            try
            {
                _logger.LogInformation("Getting source/destination details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    applicationId, categoryId);

                // Get application to determine state
                var application = await _context.ApplicationMasters
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (application == null)
                    throw new Exception("Application not found");

                // Determine application category id
                int applicationCategoryId = await DetermineApplicationCategory(
                    categoryId,
                    application.StateId ?? throw new Exception("Application StateId is null"));

                // Get registration number for this forest produce
                var applicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad => ad.ApplicationId == applicationId &&
                                              ad.ApplicationCateogryId == applicationCategoryId);

                if (applicationDetail == null)
                    return null;

                string registrationNo = applicationDetail.RegistrationNo;

                if (string.IsNullOrEmpty(registrationNo))
                    return null;

                // Get source/destination details
                var details = await _applicationRepository.GetSourceDestinationDetailsAsync(
                    applicationId, registrationNo, applicationCategoryId);

                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting source/destination details for ApplicationId: {ApplicationId}",
                    applicationId);
                return null;
            }
        }

        // Add these methods to ApplicationService
        public async Task<VehicleDetailsResponseDto> SaveVehicleDetailsAsync(SaveVehicleDetailsRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Saving vehicle details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);

                // Validate file
                if (request.VehiclePhoto == null || request.VehiclePhoto.Length == 0)
                    throw new Exception("Vehicle photo is required.");

                if (request.VehiclePhoto.ContentType != "application/pdf")
                    throw new Exception("Only PDF files are allowed for vehicle photo.");

                if (request.VehiclePhoto.Length > 2 * 1024 * 1024) // 2MB
                    throw new Exception("Vehicle photo must be less than 2MB.");

                // Upload vehicle photo
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-photos");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{request.VehiclePhoto.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.VehiclePhoto.CopyToAsync(stream);
                }

                // Save to database
                var transportDetails = new TransportDetails
                {
                    RegistrationNo = request.RegistrationNo,
                    TransportId = request.TransportId,
                    DriverName = request.DriverName,
                    DriverLicenceNo = request.DriverLicenseNo,
                    VehicleNo = request.VehicleNo,
                    VehicleOwnerName = request.VehicleOwnerName,
                    VehiclePhotograph = fileName,
                    CreatedDate = DateTime.UtcNow,
                    SourceType = "web"
                };

                _context.TransportDetails.Add(transportDetails);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var dto = new VehicleDetailsDto
                {
                    TPId = transportDetails.TPId,
                    RegistrationNo = transportDetails.RegistrationNo,
                    TransportId = transportDetails.TransportId ?? 0,
                    DriverName = transportDetails.DriverName,
                    DriverLicenseNo = transportDetails.DriverLicenceNo,
                    VehicleNo = transportDetails.VehicleNo,
                    VehicleOwnerName = transportDetails.VehicleOwnerName,
                    VehiclePhotograph = transportDetails.VehiclePhotograph,
                    CreatedDate = transportDetails.CreatedDate ?? DateTime.UtcNow
                };

                return new VehicleDetailsResponseDto
                {
                    Success = true,
                    Message = "Vehicle details saved successfully",
                    Data = dto
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VehicleDetailsResponseDto> UpdateVehicleDetailsAsync(int tpId, SaveVehicleDetailsRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Updating vehicle details for TPId: {TPId}", tpId);

                var existingDetails = await _context.TransportDetails.FindAsync(tpId);
                if (existingDetails == null)
                    throw new Exception("Vehicle details not found.");

                // If new photo is provided
                if (request.VehiclePhoto != null && request.VehiclePhoto.Length > 0)
                {
                    // Validate file
                    if (request.VehiclePhoto.ContentType != "application/pdf")
                        throw new Exception("Only PDF files are allowed for vehicle photo.");

                    if (request.VehiclePhoto.Length > 2 * 1024 * 1024)
                        throw new Exception("Vehicle photo must be less than 2MB.");

                    // Delete old file
                    if (!string.IsNullOrEmpty(existingDetails.VehiclePhotograph))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-photos", existingDetails.VehiclePhotograph);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Upload new file
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-photos");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}_{request.VehiclePhoto.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.VehiclePhoto.CopyToAsync(stream);
                    }

                    existingDetails.VehiclePhotograph = fileName;
                }

                // Update other fields
                existingDetails.TransportId = request.TransportId;
                existingDetails.DriverName = request.DriverName;
                existingDetails.DriverLicenceNo = request.DriverLicenseNo;
                existingDetails.VehicleNo = request.VehicleNo;
                existingDetails.VehicleOwnerName = request.VehicleOwnerName;
                existingDetails.UpdatedDate = DateTime.UtcNow;

                _context.TransportDetails.Update(existingDetails);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var dto = new VehicleDetailsDto
                {
                    TPId = existingDetails.TPId,
                    RegistrationNo = existingDetails.RegistrationNo,
                    TransportId = existingDetails.TransportId ?? 0,
                    DriverName = existingDetails.DriverName,
                    DriverLicenseNo = existingDetails.DriverLicenceNo,
                    VehicleNo = existingDetails.VehicleNo,
                    VehicleOwnerName = existingDetails.VehicleOwnerName,
                    VehiclePhotograph = existingDetails.VehiclePhotograph,
                    CreatedDate = existingDetails.CreatedDate ?? DateTime.UtcNow
                };

                return new VehicleDetailsResponseDto
                {
                    Success = true,
                    Message = "Vehicle details updated successfully",
                    Data = dto
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VehicleDetailsDto> GetVehicleDetailsAsync(string registrationNo)
        {
            var vehicleDetails = await _context.TransportDetails
                .FirstOrDefaultAsync(td => td.RegistrationNo == registrationNo);

            if (vehicleDetails == null)
                return null;

            return new VehicleDetailsDto
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
        }

        public async Task<bool> DeleteVehicleDetailsAsync(int tpId)
        {
            var vehicleDetails = await _context.TransportDetails.FindAsync(tpId);
            if (vehicleDetails == null)
                return false;

            // Delete the file
            if (!string.IsNullOrEmpty(vehicleDetails.VehiclePhotograph))
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "vehicle-photos", vehicleDetails.VehiclePhotograph);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.TransportDetails.Remove(vehicleDetails);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckVehicleDetailsExistsAsync(string registrationNo)
        {
            return await _context.TransportDetails
                .AnyAsync(td => td.RegistrationNo == registrationNo);
        }

        // Add these methods to ApplicationService class
        public async Task<RouteDetailsResponseDto> SaveRouteDetailsAsync(SaveRouteDetailsRequestDto request)
        {
            try
            {
                var routeDetails = await _applicationRepository.SaveRouteDetailsAsync(request);

                var dto = new RouteDetailsDto
                {
                    Id = routeDetails.Id,
                    RegistrationNo = routeDetails.RegistrationNo,
                    StateId = routeDetails.StateId ?? 0,
                    DistrictId = routeDetails.DistrictId ?? 0,
                    CreatedDate = routeDetails.CreatedDate,
                    UpdatedDate = routeDetails.UpdatedDate
                };

                return new RouteDetailsResponseDto
                {
                    Success = true,
                    Message = "Route details saved successfully",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving route details for RegistrationNo: {RegistrationNo}", request.RegistrationNo);
                return new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Error saving route details: {ex.Message}"
                };
            }
        }

        public async Task<RouteDetailsDto> GetRouteDetailsAsync(string registrationNo)
        {
            var routeDetails = await _applicationRepository.GetRouteDetailsAsync(registrationNo);

            if (routeDetails == null)
                return null;

            return new RouteDetailsDto
            {
                Id = routeDetails.Id,
                RegistrationNo = routeDetails.RegistrationNo,
                StateId = routeDetails.StateId ?? 0,
                DistrictId = routeDetails.DistrictId ?? 0,
                CreatedDate = routeDetails.CreatedDate,
                UpdatedDate = routeDetails.UpdatedDate
            };
        }

        public async Task<RouteDetailsResponseDto> UpdateRouteDetailsAsync(int routeId, SaveRouteDetailsRequestDto request)
        {
            try
            {
                var routeDetails = await _applicationRepository.UpdateRouteDetailsAsync(routeId, request);

                var dto = new RouteDetailsDto
                {
                    Id = routeDetails.Id,
                    RegistrationNo = routeDetails.RegistrationNo,
                    StateId = routeDetails.StateId ?? 0,
                    DistrictId = routeDetails.DistrictId ?? 0,
                    CreatedDate = routeDetails.CreatedDate,
                    UpdatedDate = routeDetails.UpdatedDate
                };

                return new RouteDetailsResponseDto
                {
                    Success = true,
                    Message = "Route details updated successfully",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route details for RouteId: {RouteId}", routeId);
                return new RouteDetailsResponseDto
                {
                    Success = false,
                    Message = $"Error updating route details: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteRouteDetailsAsync(int routeId)
        {
            return await _applicationRepository.DeleteRouteDetailsAsync(routeId);
        }

        public async Task<bool> CheckRouteDetailsExistsAsync(string registrationNo)
        {
            return await _applicationRepository.CheckRouteDetailsExistsAsync(registrationNo);
        }

        public async Task<UpdateApplicationStatusResponseDto> UpdateApplicationStatusAsync(UpdateApplicationStatusRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get ApplicationMaster by RegistrationNo
                var appMaster = await _applicationRepository.GetApplicationMasterByRegistrationNoAsync(request.RegistrationNo);
                if (appMaster == null)
                    throw new Exception($"Application not found for registration: {request.RegistrationNo}");

                // 2. Get current step order
                int? currentStep = await _applicationRepository.GetCurrentStepOrderForRegistrationAsync(request.RegistrationNo);
                if (currentStep == null) currentStep = 0;

                // 3. Get next officer based on current step and new status
                ApplicationOfficerAssignmentDto? nextOfficer = null;
                if (request.NewStatus != ApplicationStatusEnum.Approved &&
                    request.NewStatus != ApplicationStatusEnum.TPIssued &&
                    request.NewStatus != ApplicationStatusEnum.NOCDownloadedAndProcessCompleted &&
                    request.NewStatus != ApplicationStatusEnum.Expired &&
                    request.NewStatus != ApplicationStatusEnum.NotRecommended)
                {
                    // For statuses that need to go to next officer, find the next workflow step
                    nextOfficer = await _applicationRepository.GetNextOfficerForRegistrationAsync(request.RegistrationNo, currentStep.Value);
                }

                // 4. Create TpStatusMultiple record
                var tpStatus = new TpStatusMultiple
                {
                    RegistrationNo = request.RegistrationNo,
                    LoginIdFrom = request.OfficerLoginId,   // current officer
                    LoginIdTo = nextOfficer?.OfficerLoginId, // next officer (null if final status)
                    Status = (int)request.NewStatus,
                    CreatedDate = DateTime.UtcNow,
                    Remarks = request.Remarks ?? $"Status updated to {request.NewStatus.ToDisplayString()}"
                };
                await _applicationRepository.InsertTpStatusMultipleAsync(tpStatus);

                // 5. Update ApplicationMaster status
                await _applicationRepository.UpdateApplicationMasterStatusAsync(appMaster.ApplicationId, request.NewStatus, request.OfficerLoginId);

                await transaction.CommitAsync();

                return new UpdateApplicationStatusResponseDto
                {
                    Success = true,
                    Message = $"Status updated to {request.NewStatus.ToDisplayString()} successfully.",
                    Data = new
                    {
                        tpStatus.MultipleStatusId,
                        tpStatus.RegistrationNo,
                        tpStatus.LoginIdFrom,
                        tpStatus.LoginIdTo,
                        tpStatus.Status
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating status for RegistrationNo: {RegistrationNo}", request.RegistrationNo);
                return new UpdateApplicationStatusResponseDto
                {
                    Success = false,
                    Message = $"Failed to update status: {ex.Message}"
                };
            }

        }

        public async Task<SubmitApplicationResponseDto> SubmitApplicationAsync(SubmitApplicationRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate registration exists
                var applicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad => ad.RegistrationNo == request.RegistrationNo);
                if (applicationDetail == null)
                    throw new Exception($"Registration number {request.RegistrationNo} not found.");

                // 2. Get officers assigned to this registration
                var officers = await _applicationRepository.GetOfficersForRegistrationAsync(request.RegistrationNo);
                var firstOfficer = officers.FirstOrDefault();
                if (firstOfficer == null)
                    throw new Exception("No officer found for this registration. Cannot submit.");

                // 3. Build the TpStatusMultiple record
                var tpStatus = new TpStatusMultiple
                {
                    RegistrationNo = request.RegistrationNo,
                    LoginIdFrom = request.SubmittedByUserId,           // Applicant's LoginId
                    LoginIdTo = firstOfficer.OfficerLoginId,          // First officer's LoginId
                    Status = 1,                                       // Status ID 1 = Submitted
                    CreatedDate = DateTime.UtcNow,
                    Remarks = "Application submitted for approval."
                };
                await _applicationRepository.SaveTpStatusMultipleAsync(tpStatus);

                // 4. Update application master status to "Submitted"
                await _applicationRepository.UpdateApplicationStatusAsync(
                    request.ApplicationId,
                    "Submitted",
                    request.SubmittedByUserId
                );

                await transaction.CommitAsync();

                return new SubmitApplicationResponseDto
                {
                    Success = true,
                    Message = "Application submitted successfully.",
                    Data = new
                    {
                        tpStatus.MultipleStatusId,
                        tpStatus.RegistrationNo,
                        tpStatus.LoginIdFrom,
                        tpStatus.LoginIdTo,
                        tpStatus.Status,
                        tpStatus.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error submitting application for RegistrationNo: {RegistrationNo}", request.RegistrationNo);
                return new SubmitApplicationResponseDto
                {
                    Success = false,
                    Message = $"Failed to submit application: {ex.Message}"
                };
            }
        }

    }
}