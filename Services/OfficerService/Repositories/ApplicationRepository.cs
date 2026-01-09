using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ApplicationRepository> _logger;

        public ApplicationRepository(AppDbContext context, ILogger<ApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ApplicationRepository.cs
        public async Task<ApplicationMaster> CreateApplicationAsync(CreateApplicationRequestDto request)
        {
            var application = new ApplicationMaster
            {
                // Let database generate ApplicationId
                StateId = request.StateId,
                DistrictId = request.DistrictId,
                SubDistrictId = request.SubDistrictId,
                ForestProduceId = request.ForestProduceId,
                CreateByUserId = request.CreatedByUserId,
                CreateByUserName = request.CreatedByUserName,
                CreatedDate = DateTime.UtcNow,
                Status = true
            };

            _context.ApplicationMasters.Add(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<ApplicationDetail> AddProduceDetailAsync(AddProduceDetailRequestDto request)
        {
            var applicationDetail = new ApplicationDetail
            {
                ApplicationId = request.ApplicationId,
                ApplicationCateogryId = request.ForestProduceId,
                CreatedDate = DateTime.UtcNow,
                CreateByUserId = "AP-Ans000" // Get from auth context
            };

            _context.ApplicationDetails.Add(applicationDetail);
            await _context.SaveChangesAsync();

            return applicationDetail;
        }

        public async Task<ApplicationMaster?> GetApplicationByIdAsync(long applicationId)
        {
            return await _context.ApplicationMasters
                .Include(a => a.State)
                .Include(a => a.District)
                .Include(a => a.SubDistrict)
                .Include(a => a.ForestProduce)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }



        public async Task<long> GetNextApplicationIdAsync()
        {
            var lastApplication = await _context.ApplicationMasters
                .OrderByDescending(a => a.ApplicationId)
                .FirstOrDefaultAsync();

            return lastApplication?.ApplicationId + 1 ?? 1000;
        }

        public async Task<List<ApplicationResponseDto>> GetApplicationsByUserAsync(string userId)
        {
            return await _context.ApplicationMasters
                .Where(a => a.CreateByUserId == userId)
                .Include(a => a.State)
                .Include(a => a.District)
                .Include(a => a.SubDistrict)
                .Include(a => a.ForestProduce)
                .Select(a => new ApplicationResponseDto
                {
                    ApplicationId = a.ApplicationId,
                    StateId = a.StateId ?? 0,
                    StateName = a.State!.StateName,
                    DistrictId = a.DistrictId ?? 0,
                    DistrictName = a.District!.DistName,
                    SubDistrictId = a.SubDistrictId,
                    SubDistrictName = a.SubDistrict!.SubDistName,
                    ForestProduceId = a.ForestProduceId ?? 0,
                    ForestProduceName = a.ForestProduce!.Name,
                    CreatedDate = a.CreatedDate ?? DateTime.UtcNow,
                    Status = a.Status == true ? "Active" : "Inactive",
                    ApplicationStatus = a.ApplicationStatus ?? "Open"
                })
                .ToListAsync();
        }

        public async Task<List<OpenApplicationDto>> GetOpenApplicationsByUserAsync(string userId)
        {
            return await _context.ApplicationMasters
                .Where(a => a.CreateByUserId == userId &&
                           (a.ApplicationStatus == "Open" || a.ApplicationStatus == "InProgress"))
                .Include(a => a.State)
                .Include(a => a.District)
                .Include(a => a.ForestProduce)
                .Select(a => new OpenApplicationDto
                {
                    ApplicationId = a.ApplicationId,
                    StateName = a.State!.StateName,
                    DistrictName = a.District!.DistName,
                    ForestProduceName = a.ForestProduce!.Name,
                    CreatedDate = a.CreatedDate ?? DateTime.UtcNow,
                    ApplicationStatus = a.ApplicationStatus ?? "Open"
                })
                .ToListAsync();
        }

        // OfficerService/Repositories/ApplicationRepository.cs
        public async Task<List<RegisteredTpResponseDto>> GetRegisteredApplicationsByUserAsync(string userId)
        {
            try
            {
                var applications = await _context.ApplicationMasters
                    .Where(a => a.CreateByUserId == userId &&
                               (a.ApplicationStatus == "Open" || a.ApplicationStatus == "InProgress"))
                    .Include(a => a.State)
                    .Include(a => a.District)
                    .Include(a => a.ForestProduce)
                    .Include(a => a.ApplicationDetails)
                        .ThenInclude(ad => ad.ApplicationCategory)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToListAsync();

                var result = new List<RegisteredTpResponseDto>();

                foreach (var app in applications)
                {
                    var appDto = new RegisteredTpResponseDto
                    {
                        ApplicationId = app.ApplicationId,
                        CreatedDate = app.CreatedDate ?? DateTime.UtcNow,
                        ApplicationStatus = app.ApplicationStatus ?? "Open",
                        StateName = app.State?.StateName,
                        DistrictName = app.District?.DistName,
                        ForestProduceName = app.ForestProduce?.Name
                    };

                    if (app.ApplicationDetails != null)
                    {
                        foreach (var detail in app.ApplicationDetails)
                        {
                            appDto.ApplicationDetails.Add(new ApplicationDetailInfoDto
                            {
                                ApplicationDetailId = detail.Id,
                                RegistrationNo = detail.RegistrationNo ?? string.Empty,
                                ApplicationCategoryId = detail.ApplicationCateogryId ?? 0,
                                CategoryName = detail.ApplicationCategory?.CategoryName ?? "Unknown",
                                CreatedDate = detail.CreatedDate
                            });
                        }
                    }

                    result.Add(appDto);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting registered applications for user: {UserId}", userId);
                throw;
            }
        }
        public async Task<ApplicationDetail> AddApplicationDetailAsync(ApplicationDetail applicationDetail)
        {
            _context.ApplicationDetails.Add(applicationDetail);
            await _context.SaveChangesAsync();
            return applicationDetail;
        }

        public async Task<ApplicationMaster?> UpdateApplicationAsync(UpdateApplicationRequestDto request)
        {
            var application = await _context.ApplicationMasters
                .FirstOrDefaultAsync(a => a.ApplicationId == request.ApplicationId);

            if (application == null)
                return null;

            // Update fields if provided
            if (request.StateId.HasValue)
                application.StateId = request.StateId.Value;

            if (request.DistrictId.HasValue)
                application.DistrictId = request.DistrictId.Value;

            if (request.SubDistrictId.HasValue)
                application.SubDistrictId = request.SubDistrictId.Value;

            if (request.ForestProduceId.HasValue)
                application.ForestProduceId = request.ForestProduceId.Value;

            if (!string.IsNullOrEmpty(request.Remarks))
                application.Remarks = request.Remarks;

            application.UpdatedDate = DateTime.UtcNow;
            application.UpdatedByUserId = request.UpdatedByUserId;
            application.UpdatedByUserName = request.UpdatedByUserName;

            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteSpeciesLogAsync(long speciesLogId, string forestProduceType)
        {
            try
            {
                switch (forestProduceType.ToLower())
                {
                    case "roundtimber":
                        var roundLog = await _context.SpeciesLogsRoundTimbers.FindAsync(speciesLogId);
                        if (roundLog != null)
                        {
                            _context.SpeciesLogsRoundTimbers.Remove(roundLog);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                    case "bamboo":
                        var bambooLog = await _context.SpeciesLogsBamboos.FindAsync(speciesLogId);
                        if (bambooLog != null)
                        {
                            _context.SpeciesLogsBamboos.Remove(bambooLog);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                    case "fuelwood":
                        var fuelwoodLog = await _context.SpeciesLogsFuelwoods.FindAsync(speciesLogId);
                        if (fuelwoodLog != null)
                        {
                            _context.SpeciesLogsFuelwoods.Remove(fuelwoodLog);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                    case "minor":
                        var minorLog = await _context.SpeciesLogsMinorForestProduces.FindAsync(speciesLogId);
                        if (minorLog != null)
                        {
                            _context.SpeciesLogsMinorForestProduces.Remove(minorLog);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                    case "sawntimber":
                        var sawnLog = await _context.SpeciesLogsSawnTimbers.FindAsync(speciesLogId);
                        if (sawnLog != null)
                        {
                            _context.SpeciesLogsSawnTimbers.Remove(sawnLog);
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSpeciesLogAsync(UpdateProduceDetailDto detail, string forestProduceType)
        {
            try
            {
                switch (forestProduceType.ToLower())
                {
                    case "roundtimber":
                        var roundLog = await _context.SpeciesLogsRoundTimbers.FindAsync(detail.SpeciesLogId);
                        if (roundLog != null)
                        {
                            roundLog.SpeciesID = detail.SpeciesId;
                            roundLog.LogsNo = detail.NoOfLogs ?? 1;
                            roundLog.Girth = detail.MiddleGirthCm ?? 0.01m;
                            roundLog.Length = detail.LengthCm ?? 0.01m;
                            roundLog.Quantity = detail.Quantity ?? 0.01m;
                            roundLog.Volume = detail.Volume ?? 0.001m;
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                    case "bamboo":
                        var bambooLog = await _context.SpeciesLogsBamboos.FindAsync(detail.SpeciesLogId);
                        if (bambooLog != null)
                        {
                            bambooLog.SpeciesID = detail.SpeciesId;
                            bambooLog.GirthClass = detail.GirthClass ?? 0.01m;
                            bambooLog.Length = detail.Length ?? 0.01m;
                            bambooLog.Quantity = detail.Quantity ?? 0.01m;
                            bambooLog.Unit = !string.IsNullOrEmpty(detail.Unit) ? detail.Unit[0].ToString() : "n";
                            bambooLog.Volume = detail.Volume ?? 0.001m;
                            await _context.SaveChangesAsync();
                            return true;
                        }
                        break;

                        // Add cases for other forest produce types similarly...
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId)
        {
            var applicationDetails = await _context.ApplicationDetails
                .Where(ad => ad.ApplicationId == applicationId)
                .ToListAsync();

            var allLogs = new List<SpeciesLogResponseDto>();

            foreach (var appDetail in applicationDetails)
            {
                if (!string.IsNullOrEmpty(appDetail.RegistrationNo))
                {
                    var logs = await GetSpeciesLogsByRegistrationNoAsync(appDetail.RegistrationNo);
                    allLogs.AddRange(logs);
                }
            }

            return allLogs;
        }

        public async Task<ApplicationDetailsDto?> GetApplicationWithSpeciesLogsAsync(long applicationId)
        {
            try
            {
                _logger.LogInformation("Fetching application with species logs for ApplicationId: {ApplicationId}", applicationId);

                // Get application master with all relationships
                var application = await _context.ApplicationMasters
                    .Include(a => a.State)
                    .Include(a => a.District)
                    .Include(a => a.SubDistrict)
                    .Include(a => a.ForestProduce)
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (application == null)
                {
                    _logger.LogWarning("Application not found for ApplicationId: {ApplicationId}", applicationId);
                    return null;
                }

                // Get application details with registration numbers
                var applicationDetails = await _context.ApplicationDetails
                    .Where(ad => ad.ApplicationId == applicationId)
                    .Include(ad => ad.ApplicationCategory)
                    .ToListAsync();

                // Get species logs for all application details
                var speciesLogs = new List<SpeciesLogResponseDto>();

                foreach (var appDetail in applicationDetails)
                {
                    if (string.IsNullOrEmpty(appDetail.RegistrationNo)) continue;

                    var logs = await GetSpeciesLogsByRegistrationNoAsync(appDetail.RegistrationNo);
                    speciesLogs.AddRange(logs);
                }

                // Map to DTO
                var applicationDto = new ApplicationDetailsDto
                {
                    ApplicationId = application.ApplicationId,
                    Status = application.Status,
                    ApplicationStatus = application.ApplicationStatus,
                    CreateByUserId = application.CreateByUserId,
                    CreateByUserName = application.CreateByUserName,
                    CreatedDate = application.CreatedDate,
                    StateId = application.StateId,
                    StateName = application.State?.StateName,
                    DistrictId = application.DistrictId,
                    DistrictName = application.District?.DistName,
                    SubDistrictId = application.SubDistrictId,
                    SubDistrictName = application.SubDistrict?.SubDistName,
                    ForestProduceId = application.ForestProduceId,
                    ForestProduceName = application.ForestProduce?.Name,
                    Remarks = application.Remarks
                };

                // Add species logs to the DTO
                applicationDto.SpeciesLogs = speciesLogs;

                _logger.LogInformation("Successfully fetched application with {Count} species logs for ApplicationId: {ApplicationId}",
                    speciesLogs.Count, applicationId);

                return applicationDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching application with species logs for ApplicationId: {ApplicationId}", applicationId);
                throw;
            }
        }

        private async Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByRegistrationNoAsync(string registrationNo)
        {
            var speciesLogs = new List<SpeciesLogResponseDto>();

            // Round Timber
            var roundTimberLogs = await _context.SpeciesLogsRoundTimbers
                .Include(sl => sl.Species)
                .Include(sl => sl.ForestProduce) // Add this
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new SpeciesLogResponseDto
                {
                    SpeciesLogId = sl.Id,
                    SpeciesId = sl.SpeciesID,
                    ForestProduceId = sl.ForestProduceId,
                    SpeciesName = sl.Species.Name,
                    NoOfLogs = sl.LogsNo,
                    MiddleGirthCm = sl.Girth,
                    LengthCm = sl.Length,
                    Quantity = sl.Quantity,
                    Volume = sl.Volume,
                    ForestProduceType = "RoundTimber",
                    RegistrationNo = sl.RegistrationNo
                })
                .ToListAsync();
            speciesLogs.AddRange(roundTimberLogs);

            // Bamboo
            var bambooLogs = await _context.SpeciesLogsBamboos
                .Include(sl => sl.Species)
                .Include(sl => sl.ForestProduce) // Add this
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new SpeciesLogResponseDto
                {
                    SpeciesLogId = sl.Id,
                    SpeciesId = sl.SpeciesID,
                    ForestProduceId = sl.ForestProduceId, // Add this
                    SpeciesName = sl.Species.Name,
                    GirthClass = sl.GirthClass,
                    Length = sl.Length,
                    Quantity = sl.Quantity,
                    Volume = sl.Volume,
                    Unit = sl.Unit,
                    ForestProduceType = "Bamboo",
                    RegistrationNo = sl.RegistrationNo
                })
                .ToListAsync();
            speciesLogs.AddRange(bambooLogs);

            // Fuelwood
            var fuelwoodLogs = await _context.SpeciesLogsFuelwoods
                .Include(sl => sl.Species)
                .Include(sl => sl.ForestProduce) // Add this
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new SpeciesLogResponseDto
                {
                    SpeciesLogId = sl.Id,
                    SpeciesId = sl.SpeciesID,
                    ForestProduceId = sl.ForestProduceId, // Add this
                    SpeciesName = sl.Species.Name,
                    Quantity = sl.Quantity,
                    Unit = sl.Unit,
                    ForestProduceType = "Fuelwood",
                    RegistrationNo = sl.RegistrationNo
                })
                .ToListAsync();
            speciesLogs.AddRange(fuelwoodLogs);

            // Minor Forest Produce
            var minorLogs = await _context.SpeciesLogsMinorForestProduces
                .Include(sl => sl.Species)
                .Include(sl => sl.ForestProduce) // Add this
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new SpeciesLogResponseDto
                {
                    SpeciesLogId = sl.Id,
                    SpeciesId = sl.SpeciesID,
                    ForestProduceId = sl.ForestProduceId, // Add this
                    SpeciesName = sl.Species.Name,
                    PlantPartID = sl.PlantPartID,
                    Quantity = sl.Quantity,
                    Unit = sl.Unit,
                    ForestProduceType = "Minor",
                    RegistrationNo = sl.RegistrationNo
                })
                .ToListAsync();
            speciesLogs.AddRange(minorLogs);

            // Sawn Timber
            var sawnTimberLogs = await _context.SpeciesLogsSawnTimbers
                .Include(sl => sl.Species)
                .Include(sl => sl.ForestProduce) // Add this
                .Where(sl => sl.RegistrationNo == registrationNo)
                .Select(sl => new SpeciesLogResponseDto
                {
                    SpeciesLogId = sl.Id,
                    SpeciesId = sl.SpeciesID,
                    ForestProduceId = sl.ForestProduceId, // Add this
                    SpeciesName = sl.Species.Name,
                    NoOfPieces = sl.LogsNo,
                    LengthCm = sl.Length,
                    Width = sl.Width,
                    Thickness = sl.Thickness,
                    Volume = sl.Volume,
                    Unit = sl.Unit,
                    ForestProduceType = "SawnTimber",
                    RegistrationNo = sl.RegistrationNo
                })
                .ToListAsync();
            speciesLogs.AddRange(sawnTimberLogs);

            return speciesLogs;
        }

        public async Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId)
        {
            return await GetApplicationWithSpeciesLogsAsync(applicationId);
        }

        //public async Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request, int applicationCategoryId)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        _logger.LogInformation("Saving produce source for RegistrationNo: {RegistrationNo}, ApplicationCategory: {CategoryId}",
        //            request.RegistrationNo, applicationCategoryId);

        //        long sourceId = 0;
        //        int govDepotId = 0;
        //        int latLongId = 0;

        //        // 1. Save to appropriate source place table based on application category
        //        if (applicationCategoryId == 1) // NOC
        //        {
        //            var existingNocSource = await _context.NocSourcePlaces
        //                .FirstOrDefaultAsync(nsp => nsp.ApplicationId == request.RegistrationNo);

        //            if (existingNocSource != null)
        //            {
        //                // Update existing record
        //                existingNocSource.StateId = request.StateId;
        //                existingNocSource.CircleId = request.CircleId;
        //                existingNocSource.DivisionId = request.DivisionId;
        //                existingNocSource.RangeId = request.RangeId;
        //                existingNocSource.Address = request.Address;
        //                existingNocSource.PinCode = request.PinCode;
        //                existingNocSource.UpdatedDate = DateTime.UtcNow;

        //                _context.NocSourcePlaces.Update(existingNocSource);
        //                sourceId = existingNocSource.SourceId;
        //            }
        //            else
        //            {
        //                // Create new record
        //                var nocSourcePlace = new NocSourcePlace
        //                {
        //                    ApplicationId = request.RegistrationNo,
        //                    StateId = request.StateId,
        //                    CircleId = request.CircleId,
        //                    DivisionId = request.DivisionId,
        //                    RangeId = request.RangeId,
        //                    Address = request.Address,
        //                    PinCode = request.PinCode,
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.NocSourcePlaces.Add(nocSourcePlace);
        //                await _context.SaveChangesAsync();
        //                sourceId = nocSourcePlace.SourceId;
        //            }
        //        }
        //        else if (applicationCategoryId == 2) // Transit Pass
        //        {
        //            var existingTpSource = await _context.TpSourcePlaces
        //                .FirstOrDefaultAsync(tsp => tsp.ApplicationId == request.RegistrationNo);

        //            if (existingTpSource != null)
        //            {
        //                // Update existing record
        //                existingTpSource.StateId = request.StateId;
        //                existingTpSource.CircleId = request.CircleId;
        //                existingTpSource.DivisionId = request.DivisionId;
        //                existingTpSource.RangeId = request.RangeId;
        //                existingTpSource.Address = request.Address;
        //                existingTpSource.PinCode = request.PinCode;
        //                existingTpSource.UpdatedDate = DateTime.UtcNow;

        //                _context.TpSourcePlaces.Update(existingTpSource);
        //                sourceId = existingTpSource.SourceId;
        //            }
        //            else
        //            {
        //                // Create new record
        //                var tpSourcePlace = new TpSourcePlace
        //                {
        //                    ApplicationId = request.RegistrationNo,
        //                    StateId = request.StateId,
        //                    CircleId = request.CircleId,
        //                    DivisionId = request.DivisionId,
        //                    RangeId = request.RangeId,
        //                    Address = request.Address,
        //                    PinCode = request.PinCode,
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.TpSourcePlaces.Add(tpSourcePlace);
        //                await _context.SaveChangesAsync();
        //                sourceId = tpSourcePlace.SourceId;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Unknown application category: {applicationCategoryId}");
        //        }

        //        // 2. Save Government Depot if applicable
        //        if (request.PlaceObtained == "government_depot" &&
        //            !string.IsNullOrEmpty(request.GovernmentDepotName))
        //        {
        //            var existingGovDepot = await _context.GovernmentDepots
        //                .FirstOrDefaultAsync(gd => gd.RegistrationNo == request.RegistrationNo &&
        //                                          gd.Type == "source");

        //            if (existingGovDepot != null)
        //            {
        //                // Update existing
        //                existingGovDepot.DepotName = request.GovernmentDepotName;
        //                existingGovDepot.Type = request.GovernmentDepotType;
        //                existingGovDepot.SourceType = "web";
        //                existingGovDepot.PlaceType = "government";
        //                existingGovDepot.UpdatedDate = DateTime.UtcNow;

        //                _context.GovernmentDepots.Update(existingGovDepot);
        //                govDepotId = existingGovDepot.GdId;
        //            }
        //            else
        //            {
        //                // Create new
        //                var governmentDepot = new GovernmentDepot
        //                {
        //                    RegistrationNo = request.RegistrationNo,
        //                    DepotName = request.GovernmentDepotName,
        //                    Type = "source",
        //                    SourceType = "web",
        //                    PlaceType = "government",
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.GovernmentDepots.Add(governmentDepot);
        //                await _context.SaveChangesAsync();
        //                govDepotId = governmentDepot.GdId;
        //            }
        //        }

        //        // 3. Save Latitude/Longitude
        //        if (!string.IsNullOrEmpty(request.Latitude) && !string.IsNullOrEmpty(request.Longitude))
        //        {
        //            var existingLatLong = await _context.SourceLatLongs
        //                .FirstOrDefaultAsync(sll => sll.RegistrationNo == request.RegistrationNo);

        //            if (existingLatLong != null)
        //            {
        //                // Update existing
        //                existingLatLong.Latitude = request.Latitude;
        //                existingLatLong.Longitude = request.Longitude;
        //                existingLatLong.UpdatedDate = DateTime.UtcNow;

        //                _context.SourceLatLongs.Update(existingLatLong);
        //                latLongId = existingLatLong.LatLongId;
        //            }
        //            else
        //            {
        //                // Create new
        //                var sourceLatLong = new SourceLatLong
        //                {
        //                    RegistrationNo = request.RegistrationNo,
        //                    Latitude = request.Latitude,
        //                    Longitude = request.Longitude,
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.SourceLatLongs.Add(sourceLatLong);
        //                await _context.SaveChangesAsync();
        //                latLongId = sourceLatLong.LatLongId;
        //            }
        //        }

        //        await transaction.CommitAsync();

        //        return new SourceDestinationResponseDto
        //        {
        //            Success = true,
        //            Message = "Produce source details saved successfully",
        //            SourceId = sourceId,
        //            GovernmentDepotId = govDepotId,
        //            LatLongId = latLongId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error saving produce source for RegistrationNo: {RegistrationNo}",
        //            request.RegistrationNo);
        //        throw;
        //    }
        //}

        //public async Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request, int applicationCategoryId)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        _logger.LogInformation("Saving destination for RegistrationNo: {RegistrationNo}, ApplicationCategory: {CategoryId}",
        //            request.RegistrationNo, applicationCategoryId);

        //        long destinationId = 0;
        //        int govDepotId = 0;

        //        // 1. Save to appropriate destination place table based on application category
        //        if (applicationCategoryId == 1) // NOC
        //        {
        //            var existingNocDest = await _context.NocDestinationPlaces
        //                .FirstOrDefaultAsync(ndp => ndp.ApplicationId == request.RegistrationNo);

        //            if (existingNocDest != null)
        //            {
        //                // Update existing record
        //                existingNocDest.StateId = request.StateId;
        //                existingNocDest.CircleId = request.CircleId;
        //                existingNocDest.DivisionId = request.DivisionId;
        //                existingNocDest.RangeId = request.RangeId;
        //                existingNocDest.Address = request.Address;
        //                existingNocDest.PinCode = request.PinCode;
        //                existingNocDest.UpdatedDate = DateTime.UtcNow;

        //                _context.NocDestinationPlaces.Update(existingNocDest);
        //                destinationId = existingNocDest.DestinationId;
        //            }
        //            else
        //            {
        //                // Create new record
        //                var nocDestination = new NocDestinationPlace
        //                {
        //                    ApplicationId = request.RegistrationNo,
        //                    StateId = request.StateId,
        //                    CircleId = request.CircleId,
        //                    DivisionId = request.DivisionId,
        //                    RangeId = request.RangeId,
        //                    Address = request.Address,
        //                    PinCode = request.PinCode,
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.NocDestinationPlaces.Add(nocDestination);
        //                await _context.SaveChangesAsync();
        //                destinationId = nocDestination.DestinationId;
        //            }
        //        }
        //        else if (applicationCategoryId == 2) // Transit Pass
        //        {
        //            var existingTpDest = await _context.TpDestinationPlaces
        //                .FirstOrDefaultAsync(tdp => tdp.ApplicationId == request.RegistrationNo);

        //            if (existingTpDest != null)
        //            {
        //                // Update existing record
        //                existingTpDest.StateId = request.StateId;
        //                existingTpDest.CircleId = request.CircleId;
        //                existingTpDest.DivisionId = request.DivisionId;
        //                existingTpDest.RangeId = request.RangeId;
        //                existingTpDest.Address = request.Address;
        //                existingTpDest.PinCode = request.PinCode;
        //                existingTpDest.UpdatedDate = DateTime.UtcNow;

        //                _context.TpDestinationPlaces.Update(existingTpDest);
        //                destinationId = existingTpDest.DestinationId;
        //            }
        //            else
        //            {
        //                // Create new record
        //                var tpDestination = new TpDestinationPlace
        //                {
        //                    ApplicationId = request.RegistrationNo,
        //                    StateId = request.StateId,
        //                    CircleId = request.CircleId,
        //                    DivisionId = request.DivisionId,
        //                    RangeId = request.RangeId,
        //                    Address = request.Address,
        //                    PinCode = request.PinCode,
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.TpDestinationPlaces.Add(tpDestination);
        //                await _context.SaveChangesAsync();
        //                destinationId = tpDestination.DestinationId;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Unknown application category: {applicationCategoryId}");
        //        }

        //        // 2. Save Government Depot if applicable
        //        if (request.DestinationPlace == "government_depot" &&
        //            !string.IsNullOrEmpty(request.GovernmentDepotName))
        //        {
        //            var existingGovDepot = await _context.GovernmentDepots
        //                .FirstOrDefaultAsync(gd => gd.RegistrationNo == request.RegistrationNo &&
        //                                          gd.Type == "destination");

        //            if (existingGovDepot != null)
        //            {
        //                // Update existing
        //                existingGovDepot.DepotName = request.GovernmentDepotName;
        //                existingGovDepot.Type = request.GovernmentDepotType;
        //                existingGovDepot.SourceType = "web";
        //                existingGovDepot.PlaceType = "government";
        //                existingGovDepot.UpdatedDate = DateTime.UtcNow;

        //                _context.GovernmentDepots.Update(existingGovDepot);
        //                govDepotId = existingGovDepot.GdId;
        //            }
        //            else
        //            {
        //                // Create new
        //                var governmentDepot = new GovernmentDepot
        //                {
        //                    RegistrationNo = request.RegistrationNo,
        //                    DepotName = request.GovernmentDepotName,
        //                    Type = "destination",
        //                    SourceType = "web",
        //                    PlaceType = "government",
        //                    CreatedDate = DateTime.UtcNow
        //                };

        //                _context.GovernmentDepots.Add(governmentDepot);
        //                await _context.SaveChangesAsync();
        //                govDepotId = governmentDepot.GdId;
        //            }
        //        }

        //        await transaction.CommitAsync();

        //        return new SourceDestinationResponseDto
        //        {
        //            Success = true,
        //            Message = "Destination details saved successfully",
        //            DestinationId = destinationId,
        //            GovernmentDepotId = govDepotId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error saving destination for RegistrationNo: {RegistrationNo}",
        //            request.RegistrationNo);
        //        throw;
        //    }
        //}

        // Save Produce Source (UPDATED for multiple registrations)
        public async Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request, int applicationCategoryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Saving produce source for RegistrationNo: {RegistrationNo}, ApplicationCategory: {CategoryId}",
                    request.RegistrationNo, applicationCategoryId);

                long sourceId = 0;
                int govDepotId = 0;
                List<int> latLongIds = new List<int>();

                // 1. Save to appropriate source place table based on application category
                if (applicationCategoryId == 1) // NOC
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
                else if (applicationCategoryId == 2) // Transit Pass
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
                    throw new Exception($"Unknown application category: {applicationCategoryId}");
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

                // 3. Save Latitude/Longitude for ALL registration numbers of this application
                if (!string.IsNullOrEmpty(request.Latitude) && !string.IsNullOrEmpty(request.Longitude))
                {
                    // Get all registration numbers for this application
                    var allRegistrations = await _context.ApplicationDetails
                        .Where(ad => ad.ApplicationId == request.ApplicationId &&
                                    ad.RegistrationNo != null)
                        .Select(ad => new { ad.RegistrationNo, ad.ApplicationCateogryId })
                        .Distinct()
                        .ToListAsync();

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

        // Save Destination (UPDATED for multiple registrations)
        public async Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request, int applicationCategoryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Saving destination for RegistrationNo: {RegistrationNo}, ApplicationCategory: {CategoryId}",
                    request.RegistrationNo, applicationCategoryId);

                long destinationId = 0;
                int govDepotId = 0;

                // 1. Save to appropriate destination place table based on application category
                if (applicationCategoryId == 1) // NOC
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
                else if (applicationCategoryId == 2) // Transit Pass
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
                    throw new Exception($"Unknown application category: {applicationCategoryId}");
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

        //public async Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(long applicationId, string registrationNo, int applicationCategoryId)
        //{
        //    try
        //    {
        //        var details = new SourceDestinationDetailsDto();
        //        var produceSource = new ProduceSourceDto();
        //        var destination = new DestinationDto();

        //        // Get Produce Source Details
        //        if (applicationCategoryId == 1) // NOC
        //        {
        //            var nocSource = await _context.NocSourcePlaces
        //                .FirstOrDefaultAsync(nsp => nsp.ApplicationId == registrationNo);

        //            if (nocSource != null)
        //            {
        //                produceSource.StateId = nocSource.StateId;
        //                produceSource.CircleId = nocSource.CircleId;
        //                produceSource.DivisionId = nocSource.DivisionId;
        //                produceSource.RangeId = nocSource.RangeId;
        //                produceSource.Address = nocSource.Address;
        //                produceSource.PinCode = nocSource.PinCode;
        //            }
        //        }
        //        else if (applicationCategoryId == 2) // Transit Pass
        //        {
        //            var tpSource = await _context.TpSourcePlaces
        //                .FirstOrDefaultAsync(tsp => tsp.ApplicationId == registrationNo);

        //            if (tpSource != null)
        //            {
        //                produceSource.StateId = tpSource.StateId;
        //                produceSource.CircleId = tpSource.CircleId;
        //                produceSource.DivisionId = tpSource.DivisionId;
        //                produceSource.RangeId = tpSource.RangeId;
        //                produceSource.Address = tpSource.Address;
        //                produceSource.PinCode = tpSource.PinCode;
        //            }
        //        }

        //        // Get Destination Details
        //        if (applicationCategoryId == 1) // NOC
        //        {
        //            var nocDest = await _context.NocDestinationPlaces
        //                .FirstOrDefaultAsync(ndp => ndp.ApplicationId == registrationNo);

        //            if (nocDest != null)
        //            {
        //                destination.StateId = nocDest.StateId;
        //                destination.CircleId = nocDest.CircleId;
        //                destination.DivisionId = nocDest.DivisionId;
        //                destination.RangeId = nocDest.RangeId;
        //                destination.Address = nocDest.Address;
        //                destination.PinCode = nocDest.PinCode;
        //            }
        //        }
        //        else if (applicationCategoryId == 2) // Transit Pass
        //        {
        //            var tpDest = await _context.TpDestinationPlaces
        //                .FirstOrDefaultAsync(tdp => tdp.ApplicationId == registrationNo);

        //            if (tpDest != null)
        //            {
        //                destination.StateId = tpDest.StateId;
        //                destination.CircleId = tpDest.CircleId;
        //                destination.DivisionId = tpDest.DivisionId;
        //                destination.RangeId = tpDest.RangeId;
        //                destination.Address = tpDest.Address;
        //                destination.PinCode = tpDest.PinCode;
        //            }
        //        }

        //        // Get Government Depot Info for Source
        //        var sourceGovDepot = await _context.GovernmentDepots
        //            .FirstOrDefaultAsync(gd => gd.RegistrationNo == registrationNo &&
        //                                      gd.Type == "source");

        //        if (sourceGovDepot != null)
        //        {
        //            produceSource.PlaceObtained = "government_depot";
        //            produceSource.GovernmentDepotName = sourceGovDepot.DepotName;
        //            produceSource.GovernmentDepotType = sourceGovDepot.Type;
        //        }

        //        // Get Government Depot Info for Destination
        //        var destGovDepot = await _context.GovernmentDepots
        //            .FirstOrDefaultAsync(gd => gd.RegistrationNo == registrationNo &&
        //                                      gd.Type == "destination");

        //        if (destGovDepot != null)
        //        {
        //            destination.DestinationPlace = "government_depot";
        //            destination.GovernmentDepotName = destGovDepot.DepotName;
        //            destination.GovernmentDepotType = destGovDepot.Type;
        //        }

        //        // Get Latitude/Longitude
        //        var latLong = await _context.SourceLatLongs
        //            .FirstOrDefaultAsync(sll => sll.RegistrationNo == registrationNo);

        //        if (latLong != null)
        //        {
        //            produceSource.Latitude = latLong.Latitude;
        //            produceSource.Longitude = latLong.Longitude;
        //        }

        //        details.ProduceSource = produceSource;
        //        details.Destination = destination;

        //        return details;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting source/destination details for RegistrationNo: {RegistrationNo}",
        //            registrationNo);
        //        return null;
        //    }
        //}

        public async Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(
    long applicationId,
    string registrationNo,
    int applicationCategoryId)
        {
            try
            {
                // 1️⃣ Validate RegistrationNo against ApplicationDetails
                var application = await _context.ApplicationDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a =>
                        a.ApplicationId == applicationId &&
                        a.RegistrationNo == registrationNo);

                if (application == null)
                {
                    _logger.LogWarning(
                        "Invalid RegistrationNo {RegistrationNo} for ApplicationId {ApplicationId}",
                        registrationNo, applicationId);

                    return null;
                }

                // (Optional) Trust DB category instead of payload
                applicationCategoryId = application.ApplicationCateogryId ?? 0;


                var details = new SourceDestinationDetailsDto
                {
                    ProduceSource = new ProduceSourceDto(),
                    Destination = new DestinationDto()
                };

                // 2️⃣ Source Place
                if (applicationCategoryId == 1) // NOC
                {
                    var nocSource = await _context.NocSourcePlaces
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ApplicationId == application.RegistrationNo);

                    MapSource(nocSource, details.ProduceSource);
                }
                else if (applicationCategoryId == 2) // Transit Pass
                {
                    var tpSource = await _context.TpSourcePlaces
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ApplicationId == application.RegistrationNo);

                    MapSource(tpSource, details.ProduceSource);
                }

                // 3️⃣ Destination Place
                if (applicationCategoryId == 1)
                {
                    var nocDest = await _context.NocDestinationPlaces
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ApplicationId == application.RegistrationNo);

                    MapDestination(nocDest, details.Destination);
                }
                else if (applicationCategoryId == 2)
                {
                    var tpDest = await _context.TpDestinationPlaces
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ApplicationId == application.RegistrationNo);

                    MapDestination(tpDest, details.Destination);
                }

                // 4️⃣ Government Depot – Source
                var sourceDepot = await _context.GovernmentDepots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g =>
                        g.RegistrationNo == application.RegistrationNo &&
                        g.Type == "source");

                if (sourceDepot != null)
                {
                    details.ProduceSource.PlaceObtained = "government_depot";
                    details.ProduceSource.GovernmentDepotName = sourceDepot.DepotName;
                    details.ProduceSource.GovernmentDepotType = sourceDepot.Type;
                }

                // 5️⃣ Government Depot – Destination
                var destDepot = await _context.GovernmentDepots
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g =>
                        g.RegistrationNo == application.RegistrationNo &&
                        g.Type == "destination");

                if (destDepot != null)
                {
                    details.Destination.DestinationPlace = "government_depot";
                    details.Destination.GovernmentDepotName = destDepot.DepotName;
                    details.Destination.GovernmentDepotType = destDepot.Type;
                }

                // 6️⃣ Latitude / Longitude
                var latLong = await _context.SourceLatLongs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.RegistrationNo == application.RegistrationNo);

                if (latLong != null)
                {
                    details.ProduceSource.Latitude = latLong.Latitude;
                    details.ProduceSource.Longitude = latLong.Longitude;
                }

                // 7️⃣ Private Land Details
                var privateLand = await _context.Privatelands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pl => pl.RegistrationNo == application.RegistrationNo &&
                                               pl.Type == "source");

                if (privateLand != null)
                {
                    details.ProduceSource.PlaceObtained = "private_land";
                    details.ProduceSource.SurveyNumber = privateLand.SurveyNo;
                }

                return details;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting source/destination details for RegistrationNo: {RegistrationNo}",
                    registrationNo);

                return null;
            }
        }

        private static void MapSource(dynamic source, ProduceSourceDto dto)
        {
            if (source == null) return;

            dto.StateId = source.StateId;
            dto.CircleId = source.CircleId;
            dto.DivisionId = source.DivisionId;
            dto.RangeId = source.RangeId;
            dto.Address = source.Address;
            dto.PinCode = source.PinCode;
        }

        private static void MapDestination(dynamic dest, DestinationDto dto)
        {
            if (dest == null) return;

            dto.StateId = dest.StateId;
            dto.CircleId = dest.CircleId;
            dto.DivisionId = dest.DivisionId;
            dto.RangeId = dest.RangeId;
            dto.Address = dest.Address;
            dto.PinCode = dest.PinCode;
        }



        public async Task<bool> CheckSourceDestinationExistsAsync(string registrationNo, int applicationCategoryId)
        {
            try
            {
                if (applicationCategoryId == 1) // NOC
                {
                    var sourceExists = await _context.NocSourcePlaces
                        .AnyAsync(nsp => nsp.ApplicationId == registrationNo);
                    //var destExists = await _context.NocDestinationPlaces
                    //    .AnyAsync(ndp => ndp.ApplicationId == registrationNo);

                    return sourceExists; //&& destExists
                }
                else if (applicationCategoryId == 2) // Transit Pass
                {
                    var sourceExists = await _context.TpSourcePlaces
                        .AnyAsync(tsp => tsp.ApplicationId == registrationNo);
                    //var destExists = await _context.TpDestinationPlaces
                    //    .AnyAsync(tdp => tdp.ApplicationId == registrationNo);

                    return sourceExists; // && destExists
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking source/destination existence for RegistrationNo: {RegistrationNo}",
                    registrationNo);
                return false;
            }
        }


    }
}
