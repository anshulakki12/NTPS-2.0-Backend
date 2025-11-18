using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IMasterRolesLogsRepository
    {
        Task<MasterRolesLogs> CreateLogAsync(MasterRolesLogs log);
        Task<IEnumerable<MasterRolesLogs>> GetLogsByRoleIdAsync(int roleId);
        Task<IEnumerable<MasterRolesLogs>> GetAllLogsAsync();
        Task<IEnumerable<MasterRolesLogs>> GetLogsByOperationTypeAsync(string operationType);
        Task<IEnumerable<MasterRolesLogs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
