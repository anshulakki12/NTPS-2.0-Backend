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
    }
}
