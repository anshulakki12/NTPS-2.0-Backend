using OfficerService.DtoModels.Enums;
using OfficerService.Models;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Repositories
{
    public interface IApplicationRepository
    {
        Task<ApplicationMaster> CreateApplicationAsync(CreateApplicationRequestDto request);
        Task<ApplicationMaster?> GetApplicationByIdAsync(long applicationId);
        Task<ApplicationDetail> AddProduceDetailAsync(AddProduceDetailRequestDto request);
        Task<List<ApplicationResponseDto>> GetApplicationsByUserAsync(string userId);
        Task<long> GetNextApplicationIdAsync();
        Task<List<OpenApplicationDto>> GetOpenApplicationsByUserAsync(string userId);
        // OfficerService/Repositories/IApplicationRepository.cs
        Task<List<RegisteredTpResponseDto>> GetRegisteredApplicationsByUserAsync(string userId);
        Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId);
        Task<ApplicationDetail> AddApplicationDetailAsync(ApplicationDetail applicationDetail);
        Task<ApplicationMaster?> UpdateApplicationAsync(UpdateApplicationRequestDto request);
        Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId);
        Task<bool> DeleteSpeciesLogAsync(long speciesLogId, string forestProduceType);
        Task<bool> UpdateSpeciesLogAsync(UpdateProduceDetailDto detail, string forestProduceType);

        // Add this method for getting complete application data with species logs
        Task<ApplicationDetailsDto?> GetApplicationWithSpeciesLogsAsync(long applicationId);

        Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request, int applicationCategoryId);
        Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request, int applicationCategoryId);
        Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(long applicationId, string registrationNo, int applicationCategoryId);
        Task<bool> CheckSourceDestinationExistsAsync(string registrationNo, int applicationCategoryId);
        // Add these methods to IApplicationRepository
        Task<TransportDetails> SaveVehicleDetailsAsync(SaveVehicleDetailsRequestDto request);
        Task<TransportDetails> UpdateVehicleDetailsAsync(int tpId, SaveVehicleDetailsRequestDto request);
        Task<TransportDetails> GetVehicleDetailsAsync(string registrationNo);
        Task<bool> DeleteVehicleDetailsAsync(int tpId);
        Task<bool> CheckVehicleDetailsExistsAsync(string registrationNo);
        Task<RouteDetails> SaveRouteDetailsAsync(SaveRouteDetailsRequestDto request);
        Task<RouteDetails?> GetRouteDetailsAsync(string registrationNo);
        Task<RouteDetails?> UpdateRouteDetailsAsync(int routeId, SaveRouteDetailsRequestDto request);
        Task<bool> DeleteRouteDetailsAsync(int routeId);
        Task<bool> CheckRouteDetailsExistsAsync(string registrationNo);
        Task<List<ApplicationOfficerAssignmentDto>> GetOfficersForRegistrationAsync(string registrationNo);
        Task<bool> SaveTpStatusMultipleAsync(TpStatusMultiple status);
        Task<bool> UpdateApplicationStatusAsync(long applicationId, string status, string updatedBy);
        Task<ApplicationOfficerAssignmentDto?> GetNextOfficerForRegistrationAsync(string registrationNo, int currentStepOrder);
        Task<bool> UpdateApplicationMasterStatusAsync(long applicationId, ApplicationStatusEnum newStatus, string updatedBy);
        Task<bool> InsertTpStatusMultipleAsync(TpStatusMultiple statusRecord);
        Task<ApplicationMaster?> GetApplicationMasterByRegistrationNoAsync(string registrationNo);
        Task<int?> GetCurrentStepOrderForRegistrationAsync(string registrationNo);
    }
}
