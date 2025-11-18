using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IMasterDesignationService
    {
        Task<DesignationResponseDto> CreateDesignationAsync(CreateDesignationDto createDesignationDto);
        Task<DesignationResponseDto?> GetDesignationByIdAsync(int designationId);
        Task<IEnumerable<DesignationResponseDto>> GetAllDesignationsAsync();
        Task<IEnumerable<DesignationResponseDto>> GetActiveDesignationsAsync();
        Task<DesignationResponseDto> UpdateDesignationAsync(int designationId, UpdateDesignationDto updateDesignationDto, string? ipAddress = null);
        Task<bool> DeleteDesignationAsync(int designationId, string performedBy = "System", string? ipAddress = null);
        Task<DesignationResponseDto> DeactivateDesignationAsync(int designationId, string deactivatedBy, string? ipAddress = null);
        Task<DesignationResponseDto> ReactivateDesignationAsync(int designationId, string reactivatedBy, string? ipAddress = null);
    }
}
