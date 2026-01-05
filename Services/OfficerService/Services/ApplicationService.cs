using Azure.Core;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
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

        public ApplicationService(IApplicationRepository applicationRepository, AppDbContext context, ILogger<ApplicationService> logger)
        {
            _applicationRepository = applicationRepository;
            _context = context;
            _logger = logger;
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

                    applicationCategoryId = await DetermineApplicationCategory(
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
                        CreateByUserId = "AP-Ans000"
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

        private async Task<string> GenerateUniqueRegistrationNoForCategoryAsync(
    long applicationId,
    int applicationCategoryId,
    string stateCode)
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

        private async Task<long> SaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail, AddProduceDetailRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating species log entry for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
                    forestProduceId, registrationNo, detail.SpeciesId);

                switch (forestProduceId)
                {
                    case 1:  // Round Timber
                        var roundTimberLog = new SpeciesLogsRoundTimber
                        {
                            RegistrationNo = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            ApplicationId = request?.ApplicationId, // Store ApplicationId
                            ForestProduceId = detail.ForestProduceId, // Add this
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
                            ApplicationId = request?.ApplicationId, // Store ApplicationId
                            ForestProduceId = detail.ForestProduceId, // Add this
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
                            ForestProduceId = detail.ForestProduceId, // Add this
                            ApplicationId = request?.ApplicationId, // Store ApplicationId
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
                            ForestProduceId = detail.ForestProduceId, // Add this
                            ApplicationId = request?.ApplicationId, // Store ApplicationId
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
                            ForestProduceId = detail.ForestProduceId, // Add this
                            ApplicationId = request?.ApplicationId, // Store ApplicationId
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

        //// Add update and delete methods
        //private async Task<long> UpdateSpeciesLogAsync(int forestProduceId, long speciesLogId, ProduceDetailDto detail)
        //{
        //    // Implementation for updating existing species logs
        //    // Similar to SaveSpeciesLogAsync but with existing ID
        //    // This would find the existing entity and update its properties
        //    switch (forestProduceId)
        //    {
        //        case 1: // Round Timber
        //            var roundTimber = await _context.SpeciesLogsRoundTimbers.FindAsync(speciesLogId);
        //            if (roundTimber != null)
        //            {
        //                roundTimber.SpeciesID = detail.SpeciesId;
        //                roundTimber.LogsNo = detail.NoOfLogs ?? 1;
        //                roundTimber.Girth = detail.MiddleGirthCm ?? 0.01m;
        //                roundTimber.Length = detail.LengthCm ?? 0.01m;
        //                roundTimber.Quantity = detail.Quantity ?? 0.01m;
        //                roundTimber.Volume = detail.Volume ?? 0.001m;
        //                _context.SpeciesLogsRoundTimbers.Update(roundTimber);
        //            }
        //            break;
        //            // Implement other cases similarly
        //    }

        //    await _context.SaveChangesAsync();
        //    return speciesLogId;
        //}


        // Update the update method as well
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
        private async Task<int> DetermineApplicationCategory(int speciesId, int stateId)
        {
            try
            {
                _logger.LogInformation("Determining application category for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                    speciesId, stateId);

                // Query the SpeciesMapping table to get the CategoryID
                var speciesMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.SpeciesId == speciesId &&
                        sm.StateId == stateId &&
                        sm.IsActive);

                if (speciesMapping != null)
                {
                    _logger.LogInformation("Found SpeciesMapping - CategoryID: {CategoryID} for speciesId: {speciesId}, StateId: {StateId}",
                        speciesMapping.CategoryId, speciesId, stateId);
                    return speciesMapping.CategoryId;
                }
                else
                {
                    _logger.LogWarning("No active SpeciesMapping found for speciesId: {speciesId}, StateId: {StateId}, defaulting to Transit Pass (2)",
                        speciesId, stateId);

                    // Default to Transit Pass if no mapping found
                    return 2; // Transit Pass
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error determining application category for speciesId: {speciesId}, StateId: {StateId}",
                    speciesId, stateId);

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
                int applicationCategoryId = await DetermineApplicationCategory(
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

        public async Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving produce source for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    request.ApplicationId, request.SpeciesId);

                // Get application to determine state
                var application = await _context.ApplicationMasters
                    .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

                if (application == null)
                    throw new Exception("Application not found");

                // Determine application category id
                int applicationCategoryId = await DetermineApplicationCategory(
                    request.SpeciesId,
                    application.StateId ?? throw new Exception("Application StateId is null"));

                // Get registration number for this forest produce
                var applicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad => ad.ApplicationId == request.ApplicationId &&
                                              ad.ApplicationCateogryId == applicationCategoryId);

                if (applicationDetail == null)
                    throw new Exception("Application detail not found. Please save species details first.");

                string registrationNo = applicationDetail.RegistrationNo;

                if (string.IsNullOrEmpty(registrationNo))
                    throw new Exception("Registration number not found");

                // Save produce source
                var result = await _applicationRepository.SaveProduceSourceAsync(request, applicationCategoryId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving produce source for ApplicationId: {ApplicationId}",
                    request.ApplicationId);
                throw;
            }
        }

        public async Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Saving destination for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    request.ApplicationId, request.SpeciesId);

                // Get application to determine state
                var application = await _context.ApplicationMasters
                    .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

                if (application == null)
                    throw new Exception("Application not found");

                // Determine application category id
                int applicationCategoryId = await DetermineApplicationCategory(
                    request.SpeciesId,
                    application.StateId ?? throw new Exception("Application StateId is null"));

                // Get registration number for this forest produce
                var applicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad => ad.ApplicationId == request.ApplicationId &&
                                              ad.ApplicationCateogryId == applicationCategoryId);

                if (applicationDetail == null)
                    throw new Exception("Application detail not found. Please save species details first.");

                string registrationNo = applicationDetail.RegistrationNo;

                if (string.IsNullOrEmpty(registrationNo))
                    throw new Exception("Registration number not found");

                // Save destination
                var result = await _applicationRepository.SaveDestinationAsync(request, applicationCategoryId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving destination for ApplicationId: {ApplicationId}",
                    request.ApplicationId);
                throw;
            }
        }

        public async Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(long applicationId, int forestProduceId)
        {
            try
            {
                _logger.LogInformation("Getting source/destination details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
                    applicationId, forestProduceId);

                // Get application to determine state
                var application = await _context.ApplicationMasters
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (application == null)
                    throw new Exception("Application not found");

                // Determine application category id
                int applicationCategoryId = await DetermineApplicationCategory(
                    forestProduceId,
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

    }
}