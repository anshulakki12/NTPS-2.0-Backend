using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IMasterDesignationLogsRepository
    {
        Task<MasterDesignationLogs> CreateLogAsync(MasterDesignationLogs log);
        Task<IEnumerable<MasterDesignationLogs>> GetLogsByDesignationIdAsync(int designationId);
        Task<IEnumerable<MasterDesignationLogs>> GetAllLogsAsync();
        Task<IEnumerable<MasterDesignationLogs>> GetLogsByOperationTypeAsync(string operationType);
        Task<IEnumerable<MasterDesignationLogs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
