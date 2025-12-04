using Microsoft.AspNetCore.Rewrite;
using OfficerService.DTOs;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IGovtDepotRepository
    {
        // Govt Depot Master Methods
        Task<IEnumerable<GovtDepotResponseDto>> GetAllGovtDepotsAsync();
        Task<GovtDepotResponseDto> GetGovtDepotByIdAsync(int id);
        Task<IEnumerable<GovtDepotResponseDto>> GetGovtDepotsByStateAsync(int stateId);
        Task<GovtDepotResponseDto> CreateGovtDepotAsync(CreateGovtDepotDto createDto);
        Task<GovtDepotResponseDto> UpdateGovtDepotAsync(UpdateGovtDepotDto updateDto);
        Task<bool> DeleteGovtDepotAsync(int id);
        Task<bool> GovtDepotExistsAsync(string depotName, int stateId, int circleId, int divisionId, int rangeId, int? excludeId = null);

        // Dropdown Data Methods
        Task<IEnumerable<State>> GetAllStatesAsync();
    }
}
