using OfficerService.Models;

namespace OfficerService.Services
{
    public interface IMasterRolesLogsService
    {
        Task LogUpdateAsync(MasterRoles oldRole, MasterRoles newRole, string performedBy, string? ipAddress = null, string? remarks = null);
        Task LogDeleteAsync(MasterRoles role, string performedBy, string? ipAddress = null, string? remarks = null);
        Task<IEnumerable<MasterRolesLogs>> GetLogsByRoleIdAsync(int roleId);
        Task<IEnumerable<MasterRolesLogs>> GetAllLogsAsync();
    }
}
