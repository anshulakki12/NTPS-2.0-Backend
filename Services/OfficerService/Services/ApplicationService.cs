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
                StateName = application.State?.StName,
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
                var stateCode = application.State.StCode.ToString();
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

        //public async Task<ProduceDetailResponseDto> AddProduceDetailsAsyncs(AddProduceDetailRequestDto request)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        _logger.LogInformation("Starting to add produce details for ApplicationId: {ApplicationId}, ForestProduceId: {ForestProduceId}",
        //            request.ApplicationId, request.ForestProduceId);

        //        // Get application with state information first
        //        var application = await _context.ApplicationMasters
        //            .Include(a => a.State)
        //            .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

        //        if (application?.State == null)
        //            throw new Exception("Application or State not found");

        //        // Parse all string values to decimals first
        //        foreach (var detail in request.ProduceDetails)
        //        {
        //            ParseDetailStringValues(detail);
        //        }

        //        // Determine Application Category based on SpeciesMapping table
        //        int applicationCategoryId = await DetermineApplicationCategory(
        //            request.ForestProduceId,
        //            application.StateId ?? throw new Exception("Application StateId is null"));

        //        _logger.LogInformation("ForestProduceId: {ForestProduceId} mapped to ApplicationCategoryId: {ApplicationCategoryId}",
        //            request.ForestProduceId, applicationCategoryId);

        //        // Check if ApplicationDetail already exists for this application and category
        //        var existingApplicationDetail = await _context.ApplicationDetails
        //            .FirstOrDefaultAsync(ad =>
        //                ad.ApplicationId == request.ApplicationId &&
        //                ad.ApplicationCateogryId == applicationCategoryId);

        //        ApplicationDetail applicationDetail;
        //        string registrationNo;

        //        if (existingApplicationDetail != null)
        //        {
        //            _logger.LogInformation("Existing ApplicationDetail found, reusing it");
        //            applicationDetail = existingApplicationDetail;
        //            registrationNo = applicationDetail.RegistrationNo;

        //            // Update registration number if it's null (shouldn't happen but just in case)
        //            if (string.IsNullOrEmpty(registrationNo))
        //            {
        //                registrationNo = await GenerateRegistrationNoAsync(request.ApplicationId, applicationCategoryId);
        //                applicationDetail.RegistrationNo = registrationNo;
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        else
        //        {
        //            _logger.LogInformation("Creating new ApplicationDetail");
        //            registrationNo = await GenerateRegistrationNoAsync(request.ApplicationId, applicationCategoryId);

        //            // Create new ApplicationDetail with CORRECT category ID from SpeciesMapping
        //            applicationDetail = new ApplicationDetail
        //            {
        //                ApplicationId = request.ApplicationId,
        //                ApplicationCateogryId = applicationCategoryId, // Use the category from SpeciesMapping
        //                RegistrationNo = registrationNo,
        //                CreatedDate = DateTime.UtcNow,
        //                CreateByUserId = "AP-Ans000"
        //            };

        //            _context.ApplicationDetails.Add(applicationDetail);
        //            await _context.SaveChangesAsync();
        //            _logger.LogInformation("ApplicationDetail created with ID: {Id}", applicationDetail.Id);
        //        }

        //        // Check and save species logs based on forest produce type
        //        _logger.LogInformation("Processing {Count} species logs for RegistrationNo: {RegistrationNo}",
        //            request.ProduceDetails.Count, registrationNo);

        //        foreach (var detail in request.ProduceDetails)
        //        {
        //            await CheckAndSaveSpeciesLogAsync(request.ForestProduceId, registrationNo, detail);
        //        }

        //        // Save all species logs to database
        //        var speciesLogsCount = await _context.SaveChangesAsync();
        //        _logger.LogInformation("Saved {Count} species logs to database", speciesLogsCount);

        //        await transaction.CommitAsync();

        //        _logger.LogInformation("Successfully saved all produce details and committed transaction");

        //        return new ProduceDetailResponseDto
        //        {
        //            Success = true,
        //            Message = "Produce details saved successfully",
        //            RegistrationNo = registrationNo,
        //            ApplicationDetailId = applicationDetail.Id
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error saving produce details for ApplicationId: {ApplicationId}", request.ApplicationId);
        //        throw new Exception($"Failed to save produce details: {ex.Message}", ex);
        //    }
        //}

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

                // Parse all string values to decimals first
                foreach (var detail in request.ProduceDetails)
                {
                    ParseDetailStringValues(detail);
                }

                // Determine Application Category based on SpeciesMapping table
                int applicationCategoryId = await DetermineApplicationCategory(
                    request.ForestProduceId,
                    application.StateId ?? throw new Exception("Application StateId is null"));

                _logger.LogInformation("ForestProduceId: {ForestProduceId} mapped to ApplicationCategoryId: {ApplicationCategoryId}",
                    request.ForestProduceId, applicationCategoryId);

                // Check if ApplicationDetail already exists for this application and category
                var existingApplicationDetail = await _context.ApplicationDetails
                    .FirstOrDefaultAsync(ad =>
                        ad.ApplicationId == request.ApplicationId &&
                        ad.ApplicationCateogryId == applicationCategoryId);

                ApplicationDetail applicationDetail;
                string registrationNo;

                if (existingApplicationDetail != null)
                {
                    _logger.LogInformation("Existing ApplicationDetail found, reusing RegistrationNo: {RegistrationNo}",
                        existingApplicationDetail.RegistrationNo);
                    applicationDetail = existingApplicationDetail;
                    registrationNo = applicationDetail.RegistrationNo;

                    // Update registration number if it's null (shouldn't happen but just in case)
                    if (string.IsNullOrEmpty(registrationNo))
                    {
                        registrationNo = await GenerateRegistrationNoAsync(request.ApplicationId, applicationCategoryId);
                        applicationDetail.RegistrationNo = registrationNo;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    _logger.LogInformation("Creating new ApplicationDetail");
                    registrationNo = await GenerateRegistrationNoAsync(request.ApplicationId, applicationCategoryId);

                    // Create new ApplicationDetail with CORRECT category ID from SpeciesMapping
                    applicationDetail = new ApplicationDetail
                    {
                        ApplicationId = request.ApplicationId,
                        ApplicationCateogryId = applicationCategoryId, // Use the category from SpeciesMapping
                        RegistrationNo = registrationNo,
                        CreatedDate = DateTime.UtcNow,
                        CreateByUserId = "AP-Ans000"
                    };

                    _context.ApplicationDetails.Add(applicationDetail);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("ApplicationDetail created with ID: {Id} and RegistrationNo: {RegistrationNo}",
                        applicationDetail.Id, registrationNo);
                }

                // ALWAYS save species logs - don't check for existence
                // This allows multiple entries of the same species under the same registration number
                _logger.LogInformation("Saving {Count} species logs for RegistrationNo: {RegistrationNo}",
                    request.ProduceDetails.Count, registrationNo);

                foreach (var detail in request.ProduceDetails)
                {
                    // Directly save without checking existence - always create new entries
                    await SaveSpeciesLogAsync(request.ForestProduceId, registrationNo, detail);
                }

                // Save all species logs to database
                var speciesLogsCount = await _context.SaveChangesAsync();
                _logger.LogInformation("Saved {Count} species logs to database for RegistrationNo: {RegistrationNo}",
                    speciesLogsCount, registrationNo);

                await transaction.CommitAsync();

                _logger.LogInformation("Successfully saved all produce details and committed transaction");

                return new ProduceDetailResponseDto
                {
                    Success = true,
                    Message = "Produce details saved successfully",
                    RegistrationNo = registrationNo,
                    ApplicationDetailId = applicationDetail.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving produce details for ApplicationId: {ApplicationId}", request.ApplicationId);
                throw new Exception($"Failed to save produce details: {ex.Message}", ex);
            }
        }

        //private async Task CheckAndSaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Checking and saving species log for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
        //            registrationNo, detail.SpeciesId);

        //        bool exists = await CheckIfSpeciesLogExistsAsync(forestProduceId, registrationNo, detail);

        //        if (!exists)
        //        {
        //            await SaveSpeciesLogAsync(forestProduceId, registrationNo, detail);
        //            _logger.LogInformation("Created new species log for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
        //                registrationNo, detail.SpeciesId);
        //        }
        //        else
        //        {
        //            _logger.LogInformation("Species log already exists for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId} - skipping",
        //                registrationNo, detail.SpeciesId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in CheckAndSaveSpeciesLogAsync for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
        //            registrationNo, detail.SpeciesId);
        //        throw;
        //    }
        //}

        private async Task CheckAndSaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail)
        {
            try
            {
                _logger.LogInformation("Saving species log for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
                    registrationNo, detail.SpeciesId);

                // ALWAYS save the species log - don't check for existence
                // This allows multiple entries of the same species under the same registration number
                await SaveSpeciesLogAsync(forestProduceId, registrationNo, detail);

                _logger.LogInformation("Created species log for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
                    registrationNo, detail.SpeciesId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckAndSaveSpeciesLogAsync for RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
                    registrationNo, detail.SpeciesId);
                throw;
            }
        }

        //private async Task SaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Saving species log for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}, SpeciesId: {SpeciesId}",
        //            forestProduceId, registrationNo, detail.SpeciesId);

        //        switch (forestProduceId)
        //        {
        //            case 1:  // Round Timber
        //                var roundTimberLog = new SpeciesLogsRoundTimber
        //                {
        //                    TPRegistration = registrationNo,
        //                    SpeciesID = detail.SpeciesId,
        //                    LogsNo = detail.NoOfLogs ?? 1,
        //                    Girth = detail.MiddleGirthCm ?? 0.01m,
        //                    Length = detail.LengthCm ?? 0.01m,
        //                    Quantity = detail.Quantity ?? 0.01m,
        //                    Volume = detail.Volume ?? 0.001m,
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _context.SpeciesLogsRoundTimbers.Add(roundTimberLog);
        //                break;

        //            case 2: // Bamboo 
        //                var bambooLog = new SpeciesLogsBamboo
        //                {
        //                    TPRegistration = registrationNo,
        //                    SpeciesID = detail.SpeciesId,
        //                    GirthClass = detail.GirthClass ?? 0.01m,
        //                    Length = detail.Length ?? 0.01m,
        //                    Quantity = detail.Quantity ?? 0.01m,
        //                    Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "n",
        //                    Volume = detail.Volume ?? 0.001m,
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _context.SpeciesLogsBamboos.Add(bambooLog);
        //                break;

        //            case 3: // Fuelwood
        //                var fuelwoodLog = new SpeciesLogsFuelwood
        //                {
        //                    TPRegistration = registrationNo,
        //                    SpeciesID = detail.SpeciesId,
        //                    Quantity = detail.Quantity ?? 0.01m,
        //                    Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "t",
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _context.SpeciesLogsFuelwoods.Add(fuelwoodLog);
        //                break;

        //            case 4: // Minor Forest Produce
        //                var minorLog = new SpeciesLogsMinorForestProduce
        //                {
        //                    TPRegistration = registrationNo,
        //                    SpeciesID = detail.SpeciesId,
        //                    PlantPartID = detail.PlantPartID ?? 1,
        //                    Quantity = detail.Quantity ?? 0.01m,
        //                    Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "k",
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _context.SpeciesLogsMinorForestProduces.Add(minorLog);
        //                break;

        //            case 5: // Sawn Timber
        //                var sawnTimberLog = new SpeciesLogsSawnTimber
        //                {
        //                    TPRegistration = registrationNo,
        //                    SpeciesID = detail.SpeciesId,
        //                    LogsNo = detail.NoOfPieces ?? 1,
        //                    Girth = 0.01m, // Required field, set default
        //                    Length = detail.LengthCm ?? 0.01m,
        //                    Width = detail.Width ?? 0.01m,
        //                    Thickness = detail.Thickness ?? 0.01m,
        //                    Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "c",
        //                    Volume = detail.Volume ?? 0.001m,
        //                    CreatedDate = DateTime.UtcNow
        //                };
        //                _context.SpeciesLogsSawnTimbers.Add(sawnTimberLog);
        //                break;

        //            default:
        //                throw new Exception($"Unknown forest produce ID: {forestProduceId}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving species log for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}",
        //            forestProduceId, registrationNo);
        //        throw;
        //    }
        //}

        private async Task SaveSpeciesLogAsync(int forestProduceId, string registrationNo, ProduceDetailDto detail)
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
                            TPRegistration = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            LogsNo = detail.NoOfLogs ?? 1,
                            Girth = detail.MiddleGirthCm ?? 0.01m,
                            Length = detail.LengthCm ?? 0.01m,
                            Quantity = detail.Quantity ?? 0.01m,
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsRoundTimbers.Add(roundTimberLog);
                        _logger.LogInformation("Added Round Timber log: {@RoundTimberLog}", roundTimberLog);
                        break;

                    case 2: // Bamboo 
                        var bambooLog = new SpeciesLogsBamboo
                        {
                            TPRegistration = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            GirthClass = detail.GirthClass ?? 0.01m,
                            Length = detail.Length ?? 0.01m,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "n",
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsBamboos.Add(bambooLog);
                        _logger.LogInformation("Added Bamboo log: {@BambooLog}", bambooLog);
                        break;

                    case 3: // Fuelwood
                        var fuelwoodLog = new SpeciesLogsFuelwood
                        {
                            TPRegistration = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "t",
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsFuelwoods.Add(fuelwoodLog);
                        _logger.LogInformation("Added Fuelwood log: {@FuelwoodLog}", fuelwoodLog);
                        break;

                    case 4: // Minor Forest Produce
                        var minorLog = new SpeciesLogsMinorForestProduce
                        {
                            TPRegistration = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            PlantPartID = detail.PlantPartID ?? 1,
                            Quantity = detail.Quantity ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "k",
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsMinorForestProduces.Add(minorLog);
                        _logger.LogInformation("Added Minor Forest Produce log: {@MinorLog}", minorLog);
                        break;

                    case 5: // Sawn Timber
                        var sawnTimberLog = new SpeciesLogsSawnTimber
                        {
                            TPRegistration = registrationNo,
                            SpeciesID = detail.SpeciesId,
                            LogsNo = detail.NoOfPieces ?? 1,
                            Girth = 0.01m, // Required field, set default
                            Length = detail.LengthCm ?? 0.01m,
                            Width = detail.Width ?? 0.01m,
                            Thickness = detail.Thickness ?? 0.01m,
                            Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "c",
                            Volume = detail.Volume ?? 0.001m,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.SpeciesLogsSawnTimbers.Add(sawnTimberLog);
                        _logger.LogInformation("Added Sawn Timber log: {@SawnTimberLog}", sawnTimberLog);
                        break;

                    default:
                        throw new Exception($"Unknown forest produce ID: {forestProduceId}");
                }

                // No need to call SaveChangesAsync here - it will be called once after all logs are added
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving species log for ForestProduceId: {ForestProduceId}, RegistrationNo: {RegistrationNo}",
                    forestProduceId, registrationNo);
                throw;
            }
        }

        // Helper method to map Forest Produce to Application Category using SpeciesMapping table
        private async Task<int> DetermineApplicationCategory(int forestProduceId, int stateId)
        {
            try
            {
                _logger.LogInformation("Determining application category for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                    forestProduceId, stateId);

                // Query the SpeciesMapping table to get the CategoryID
                var speciesMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.ForestProduceId == forestProduceId &&
                        sm.StateId == stateId &&
                        sm.IsActive);

                if (speciesMapping != null)
                {
                    _logger.LogInformation("Found SpeciesMapping - CategoryID: {CategoryID} for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                        speciesMapping.CategoryId, forestProduceId, stateId);
                    return speciesMapping.CategoryId;
                }
                else
                {
                    _logger.LogWarning("No active SpeciesMapping found for ForestProduceId: {ForestProduceId}, StateId: {StateId}, defaulting to Transit Pass (2)",
                        forestProduceId, stateId);

                    // Default to Transit Pass if no mapping found
                    return 2; // Transit Pass
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error determining application category for ForestProduceId: {ForestProduceId}, StateId: {StateId}",
                    forestProduceId, stateId);

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
                    request.ApplicationId, request.ForestProduceId);

                // Get application with state information
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

                if (application?.State == null)
                    throw new Exception("Application or State not found");

                // Determine Application Category
                int applicationCategoryId = await DetermineApplicationCategory(
                    request.ForestProduceId,
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
                        await _applicationRepository.DeleteSpeciesLogAsync(detail.SpeciesLogId.Value, GetForestProduceType(request.ForestProduceId));
                    }
                    else if (detail.SpeciesLogId.HasValue)
                    {
                        // Update existing record
                        await _applicationRepository.UpdateSpeciesLogAsync(detail, GetForestProduceType(request.ForestProduceId));
                    }
                    else
                    {
                        // Create new record
                        await SaveSpeciesLogAsync(request.ForestProduceId, registrationNo, detail);
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
    }
}