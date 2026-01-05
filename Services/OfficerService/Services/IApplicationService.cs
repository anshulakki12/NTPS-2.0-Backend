using OfficerService.Models;
using static OfficerService.DtoModels.ApplicationDto;

namespace OfficerService.Services
{
    public interface IApplicationService
    {
        Task<ApplicationResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request);
        Task<ApplicationResponseDto?> GetApplicationAsync(long applicationId);
        Task<List<ApplicationResponseDto>> GetUserApplicationsAsync(string userId);
        Task<List<OpenApplicationDto>> GetOpenUserApplicationsAsync(string userId);
        // OfficerService/Services/IApplicationService.cs
        Task<List<RegisteredTpResponseDto>> GetRegisteredApplicationsAsync(string userId);
        Task<ApplicationMaster?> GetApplicationByIdAsync(long applicationId);
        Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId);
        Task<ProduceDetailResponseDto> AddProduceDetailsAsyncs(AddProduceDetailRequestDto request);
        Task<string> GenerateRegistrationNoAsync(long applicationId, int forestProduceId);
        Task<ApplicationMaster?> UpdateApplicationAsync(UpdateApplicationRequestDto request);
        Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId);
        Task<ProduceDetailResponseDto> UpdateProduceDetailsAsync(UpdateProduceDetailRequestDto request);
        Task<bool> DeleteSpeciesLogAsync(long speciesLogId, string forestProduceType);
        Task<ApplicationDetailsDto?> GetApplicationWithSpeciesLogsAsync(long applicationId);
        Task<SourceDestinationResponseDto> SaveProduceSourceAsync(SaveProduceSourceRequestDto request);
        Task<SourceDestinationResponseDto> SaveDestinationAsync(SaveDestinationRequestDto request);
        Task<SourceDestinationDetailsDto?> GetSourceDestinationDetailsAsync(long applicationId, int forestProduceId);

    }
}
