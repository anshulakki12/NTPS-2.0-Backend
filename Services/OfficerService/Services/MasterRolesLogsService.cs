using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterRolesLogsService : IMasterRolesLogsService
    {
        private readonly IMasterRolesLogsRepository _logsRepository;

        public MasterRolesLogsService(IMasterRolesLogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task LogUpdateAsync(MasterRoles oldRole, MasterRoles newRole, string performedBy, string? ipAddress = null, string? remarks = null)
        {
            var log = new MasterRolesLogs
            {
                RoleId = oldRole.RoleId,
                RoleName = newRole.RoleName,
                IsActive = newRole.IsActive,
                OperationType = "UPDATE",
                OperationDate = DateTime.UtcNow,
                OperationBy = performedBy,
                OldRoleName = oldRole.RoleName,
                NewRoleName = newRole.RoleName,
                OldIsActive = oldRole.IsActive,
                NewIsActive = newRole.IsActive,
                CreatedDate = oldRole.CreatedDate,
                CreatedBy = oldRole.CreatedBy,
                UpdatedDate = newRole.UpdatedDate,
                UpdatedBy = newRole.UpdatedBy,
                DeactivatedOn = newRole.DeactivatedOn,
                ReactivatedOn = newRole.ReactivatedOn,
                Remarks = remarks,
                IpAddress = ipAddress
            };

            await _logsRepository.CreateLogAsync(log);
        }

        public async Task LogDeleteAsync(MasterRoles role, string performedBy, string? ipAddress = null, string? remarks = null)
        {
            var log = new MasterRolesLogs
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive,
                OperationType = "DELETE",
                OperationDate = DateTime.UtcNow,
                OperationBy = performedBy,
                OldRoleName = role.RoleName,
                NewRoleName = null,
                OldIsActive = role.IsActive,
                NewIsActive = null,
                CreatedDate = role.CreatedDate,
                CreatedBy = role.CreatedBy,
                UpdatedDate = role.UpdatedDate,
                UpdatedBy = role.UpdatedBy,
                DeactivatedOn = role.DeactivatedOn,
                ReactivatedOn = role.ReactivatedOn,
                Remarks = remarks ?? "Role permanently deleted",
                IpAddress = ipAddress
            };

            await _logsRepository.CreateLogAsync(log);
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetLogsByRoleIdAsync(int roleId)
        {
            return await _logsRepository.GetLogsByRoleIdAsync(roleId);
        }

        public async Task<IEnumerable<MasterRolesLogs>> GetAllLogsAsync()
        {
            return await _logsRepository.GetAllLogsAsync();
        }
    }
}
