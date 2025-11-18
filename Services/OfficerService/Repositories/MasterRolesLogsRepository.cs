using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class MasterRolesLogsRepository : IMasterRolesLogsRepository
    {
        private readonly AppDbContext _context;

        public MasterRolesLogsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MasterRolesLogs> CreateLogAsync(MasterRolesLogs log)
        {
            _context.MasterRolesLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetLogsByRoleIdAsync(int roleId)
        {
            return await _context.MasterRolesLogs
                .Where(log => log.RoleId == roleId)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetAllLogsAsync()
        {
            return await _context.MasterRolesLogs
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetLogsByOperationTypeAsync(string operationType)
        {
            return await _context.MasterRolesLogs
                .Where(log => log.OperationType == operationType)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MasterRolesLogs
                .Where(log => log.OperationDate >= startDate && log.OperationDate <= endDate)
                .OrderByDescending(log => log.OperationDate)
                .ToListAsync();
        }
    }
}
