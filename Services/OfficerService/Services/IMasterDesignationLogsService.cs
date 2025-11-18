using OfficerService.Models;

namespace OfficerService.Services
{
    public interface IMasterDesignationLogsService
    {
        Task LogUpdateAsync(MasterDesignation oldDesignation, MasterDesignation newDesignation, string performedBy, string? ipAddress = null, string? remarks = null);
        Task LogDeleteAsync(MasterDesignation designation, string performedBy, string? ipAddress = null, string? remarks = null);
        Task<IEnumerable<MasterDesignationLogs>> GetLogsByDesignationIdAsync(int designationId);
        Task<IEnumerable<MasterDesignationLogs>> GetAllLogsAsync();
    }
}
