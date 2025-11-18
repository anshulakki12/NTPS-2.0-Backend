using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class MasterDesignationLogsRepository : IMasterDesignationLogsRepository
    {
        private readonly AppDbContext _context;

        public MasterDesignationLogsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterDesignationLogs> CreateLogAsync(MasterDesignationLogs log)
        {
            _context.MasterDesignationLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetLogsByDesignationIdAsync(int designationId)
        {
            return await _context.MasterDesignationLogs
                .Where(log => log.DesignationId == designationId)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetAllLogsAsync()
        {
            return await _context.MasterDesignationLogs
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetLogsByOperationTypeAsync(string operationType)
        {
            return await _context.MasterDesignationLogs
                .Where(log => log.OperationType == operationType)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MasterDesignationLogs
                .Where(log => log.OperationDate >= startDate && log.OperationDate <= endDate)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }
    }
}
