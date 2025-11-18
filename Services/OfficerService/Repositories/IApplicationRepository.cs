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
        Task<ApplicationDetailsDto?> GetApplicationByIdAsyncs(long applicationId);
        Task<ApplicationDetail> AddApplicationDetailAsync(ApplicationDetail applicationDetail);
        Task<ApplicationMaster?> UpdateApplicationAsync(UpdateApplicationRequestDto request);
        Task<List<SpeciesLogResponseDto>> GetSpeciesLogsByApplicationAsync(long applicationId);
        Task<bool> DeleteSpeciesLogAsync(long speciesLogId, string forestProduceType);
        Task<bool> UpdateSpeciesLogAsync(UpdateProduceDetailDto detail, string forestProduceType);
    }
}
