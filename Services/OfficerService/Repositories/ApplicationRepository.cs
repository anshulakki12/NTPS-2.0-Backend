using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
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

        // Remove the GetNextApplicationIdAsync method as it's no longer needed

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
                    StateName = a.State!.StName,
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
                    StateName = a.State!.StName,
                    DistrictName = a.District!.DistName,
                    ForestProduceName = a.ForestProduce!.Name,
                    CreatedDate = a.CreatedDate ?? DateTime.UtcNow,
                    ApplicationStatus = a.ApplicationStatus ?? "Open"
                })
                .ToListAsync();
        }

        // ApplicationRepository.cs
        public async Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId)
        {
            return await _context.ApplicationMasters
                .Where(a => a.ApplicationId == applicationId)
                .Include(a => a.State)
                .Include(a => a.District)
                .Include(a => a.SubDistrict)
                .Include(a => a.ForestProduce)
                .Select(a => new ApplicationDetailsDto
                {
                    ApplicationId = a.ApplicationId,
                    Status = a.Status,
                    ApplicationStatus = a.ApplicationStatus,
                    CreateByUserId = a.CreateByUserId,
                    CreateByUserName = a.CreateByUserName,
                    CreatedDate = a.CreatedDate,
                    StateId = a.StateId,
                    StateName = a.State != null ? a.State.StName : null,
                    DistrictId = a.DistrictId,
                    DistrictName = a.District != null ? a.District.DistName : null,
                    SubDistrictId = a.SubDistrictId,
                    SubDistrictName = a.SubDistrict != null ? a.SubDistrict.SubDistName : null,
                    ForestProduceId = a.ForestProduceId,
                    ForestProduceName = a.ForestProduce != null ? a.ForestProduce.Name : null,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
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

        public async Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId)
        {
            var speciesLogs = new List<SpeciesLogResponseDto>();

            // Get application details to find registration numbers
            var applicationDetails = await _context.ApplicationDetails
                .Where(ad => ad.ApplicationId == applicationId)
                .ToListAsync();

            foreach (var appDetail in applicationDetails)
            {
                if (string.IsNullOrEmpty(appDetail.RegistrationNo))
                    continue;

                // Round Timber
                var roundTimberLogs = await _context.SpeciesLogsRoundTimbers
                    .Where(log => log.TPRegistration == appDetail.RegistrationNo)
                    .Include(log => log.Species)
                    .Select(log => new SpeciesLogResponseDto
                    {
                        SpeciesLogId = log.Id,
                        SpeciesId = log.SpeciesID,
                        SpeciesName = log.Species.Name,
                        NoOfLogs = log.LogsNo,
                        MiddleGirthCm = log.Girth,
                        LengthCm = log.Length,
                        Quantity = log.Quantity,
                        Volume = log.Volume,
                        ForestProduceType = "RoundTimber"
                    })
                    .ToListAsync();
                speciesLogs.AddRange(roundTimberLogs);

                // Bamboo
                var bambooLogs = await _context.SpeciesLogsBamboos
                    .Where(log => log.TPRegistration == appDetail.RegistrationNo)
                    .Include(log => log.Species)
                    .Select(log => new SpeciesLogResponseDto
                    {
                        SpeciesLogId = log.Id,
                        SpeciesId = log.SpeciesID,
                        SpeciesName = log.Species.Name,
                        GirthClass = log.GirthClass,
                        Length = log.Length,
                        Quantity = log.Quantity,
                        Unit = log.Unit,
                        Volume = log.Volume,
                        ForestProduceType = "Bamboo"
                    })
                    .ToListAsync();
                speciesLogs.AddRange(bambooLogs);

                // Fuelwood
                var fuelwoodLogs = await _context.SpeciesLogsFuelwoods
                    .Where(log => log.TPRegistration == appDetail.RegistrationNo)
                    .Include(log => log.Species)
                    .Select(log => new SpeciesLogResponseDto
                    {
                        SpeciesLogId = log.Id,
                        SpeciesId = log.SpeciesID,
                        SpeciesName = log.Species.Name,
                        Quantity = log.Quantity,
                        Unit = log.Unit,
                        ForestProduceType = "Fuelwood"
                    })
                    .ToListAsync();
                speciesLogs.AddRange(fuelwoodLogs);

                // Minor Forest Produce
                var minorLogs = await _context.SpeciesLogsMinorForestProduces
                    .Where(log => log.TPRegistration == appDetail.RegistrationNo)
                    .Include(log => log.Species)
                    .Select(log => new SpeciesLogResponseDto
                    {
                        SpeciesLogId = log.Id,
                        SpeciesId = log.SpeciesID,
                        SpeciesName = log.Species.Name,
                        PlantPartID = log.PlantPartID,
                        Quantity = log.Quantity,
                        Unit = log.Unit,
                        ForestProduceType = "Minor"
                    })
                    .ToListAsync();
                speciesLogs.AddRange(minorLogs);

                // Sawn Timber
                var sawnTimberLogs = await _context.SpeciesLogsSawnTimbers
                    .Where(log => log.TPRegistration == appDetail.RegistrationNo)
                    .Include(log => log.Species)
                    .Select(log => new SpeciesLogResponseDto
                    {
                        SpeciesLogId = log.Id,
                        SpeciesId = log.SpeciesID,
                        SpeciesName = log.Species.Name,
                        NoOfPieces = log.LogsNo,
                        LengthCm = log.Length,
                        Width = log.Width,
                        Thickness = log.Thickness,
                        Unit = log.Unit,
                        Volume = log.Volume,
                        ForestProduceType = "SawnTimber"
                    })
                    .ToListAsync();
                speciesLogs.AddRange(sawnTimberLogs);
            }

            return speciesLogs;
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


    }
}
