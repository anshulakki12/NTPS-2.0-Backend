using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterDesignationLogsService : IMasterDesignationLogsService
    {
        private readonly IMasterDesignationLogsRepository _logsRepository;

        public MasterDesignationLogsService(IMasterDesignationLogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task LogUpdateAsync(MasterDesignation oldDesignation, MasterDesignation newDesignation, string performedBy, string? ipAddress = null, string? remarks = null)
        {
            var log = new MasterDesignationLogs
            {
                DesignationId = oldDesignation.DesignationId,
                DesignationName = newDesignation.DesignationName,
                Rank = newDesignation.Rank,
                Abbreviation = newDesignation.Abbreviation,
                IsActive = newDesignation.IsActive,
                OperationType = "UPDATE",
                OperationDate = DateTime.UtcNow,
                OperationBy = performedBy,
                OldDesignationName = oldDesignation.DesignationName,
                NewDesignationName = newDesignation.DesignationName,
                OldRank = oldDesignation.Rank,
                NewRank = newDesignation.Rank,
                OldAbbreviation = oldDesignation.Abbreviation,
                NewAbbreviation = newDesignation.Abbreviation,
                OldIsActive = oldDesignation.IsActive,
                NewIsActive = newDesignation.IsActive,
                CreatedDate = oldDesignation.CreatedDate,
                CreatedBy = oldDesignation.CreatedBy,
                UpdatedDate = newDesignation.UpdatedDate,
                UpdatedBy = newDesignation.UpdatedBy,
                DeactivatedOn = newDesignation.DeactivatedOn,
                ReactivatedOn = newDesignation.ReactivatedOn,
                Remarks = remarks,
                IpAddress = ipAddress
            };

            await _logsRepository.CreateLogAsync(log);
        }

        public async Task LogDeleteAsync(MasterDesignation designation, string performedBy, string? ipAddress = null, string? remarks = null)
        {
            var log = new MasterDesignationLogs
            {
                DesignationId = designation.DesignationId,
                DesignationName = designation.DesignationName,
                Rank = designation.Rank,
                Abbreviation = designation.Abbreviation,
                IsActive = designation.IsActive,
                OperationType = "DELETE",
                OperationDate = DateTime.UtcNow,
                OperationBy = performedBy,
                OldDesignationName = designation.DesignationName,
                NewDesignationName = null,
                OldRank = designation.Rank,
                NewRank = null,
                OldAbbreviation = designation.Abbreviation,
                NewAbbreviation = null,
                OldIsActive = designation.IsActive,
                NewIsActive = null,
                CreatedDate = designation.CreatedDate,
                CreatedBy = designation.CreatedBy,
                UpdatedDate = designation.UpdatedDate,
                UpdatedBy = designation.UpdatedBy,
                DeactivatedOn = designation.DeactivatedOn,
                ReactivatedOn = designation.ReactivatedOn,
                Remarks = remarks ?? "Designation permanently deleted",
                IpAddress = ipAddress
            };

            await _logsRepository.CreateLogAsync(log);
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetLogsByDesignationIdAsync(int designationId)
        {
            return await _logsRepository.GetLogsByDesignationIdAsync(designationId);
        }

        public async Task<IEnumerable<MasterDesignationLogs>> GetAllLogsAsync()
        {
            return await _logsRepository.GetAllLogsAsync();
        }
    }
}
