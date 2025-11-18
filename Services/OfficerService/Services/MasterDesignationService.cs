using OfficerService.DtoModels;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Services
{
    public class MasterDesignationService : IMasterDesignationService
    {
        private readonly IMasterDesignationRepository _designationRepository;
        private readonly IMasterDesignationLogsService _logsService;

        public MasterDesignationService(IMasterDesignationRepository designationRepository, IMasterDesignationLogsService logsService)
        {
            _designationRepository = designationRepository;
            _logsService = logsService;
        }

        public async Task<DesignationResponseDto> CreateDesignationAsync(CreateDesignationDto createDesignationDto)
        {
            try
            {
                var designation = await _designationRepository.CreateDesignationAsync(createDesignationDto);
                return MapToResponseDto(designation);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the designation.", ex);
            }
        }

        public async Task<DesignationResponseDto?> GetDesignationByIdAsync(int designationId)
        {
            var designation = await _designationRepository.GetDesignationByIdAsync(designationId);
            return designation != null ? MapToResponseDto(designation) : null;
        }

        public async Task<IEnumerable<DesignationResponseDto>> GetAllDesignationsAsync()
        {
            var designations = await _designationRepository.GetAllDesignationsAsync();
            return designations.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<DesignationResponseDto>> GetActiveDesignationsAsync()
        {
            var designations = await _designationRepository.GetActiveDesignationsAsync();
            return designations.Select(MapToResponseDto);
        }

        public async Task<DesignationResponseDto> UpdateDesignationAsync(int designationId, UpdateDesignationDto updateDesignationDto, string? ipAddress = null)
        {
            var existingDesignation = await _designationRepository.GetDesignationByIdAsync(designationId);
            if (existingDesignation == null)
            {
                throw new InvalidOperationException($"Designation with ID {designationId} not found.");
            }

            // Check if another designation with the same name exists (excluding current designation)
            var duplicateDesignation = await _designationRepository.GetDesignationByNameAsync(updateDesignationDto.DesignationName);
            if (duplicateDesignation != null && duplicateDesignation.DesignationId != designationId)
            {
                throw new InvalidOperationException($"Designation '{updateDesignationDto.DesignationName}' already exists.");
            }

            // Create a copy of the old designation for logging
            var oldDesignation = new MasterDesignation
            {
                DesignationId = existingDesignation.DesignationId,
                DesignationName = existingDesignation.DesignationName,
                Rank = existingDesignation.Rank,
                Abbreviation = existingDesignation.Abbreviation,
                IsActive = existingDesignation.IsActive,
                CreatedDate = existingDesignation.CreatedDate,
                CreatedBy = existingDesignation.CreatedBy,
                UpdatedDate = existingDesignation.UpdatedDate,
                UpdatedBy = existingDesignation.UpdatedBy,
                DeactivatedOn = existingDesignation.DeactivatedOn,
                ReactivatedOn = existingDesignation.ReactivatedOn
            };

            // Track activation/deactivation status changes
            var wasActive = existingDesignation.IsActive;
            var willBeActive = updateDesignationDto.IsActive;

            // Update properties
            existingDesignation.DesignationName = updateDesignationDto.DesignationName;
            existingDesignation.Rank = updateDesignationDto.Rank;
            existingDesignation.Abbreviation = updateDesignationDto.Abbreviation;
            existingDesignation.IsActive = updateDesignationDto.IsActive;
            existingDesignation.UpdatedDate = DateTime.UtcNow;
            existingDesignation.UpdatedBy = updateDesignationDto.UpdatedBy;

            // Track deactivation/reactivation - PRESERVE ALL HISTORY
            if (wasActive && !willBeActive)
            {
                existingDesignation.DeactivatedOn = DateTime.UtcNow;
            }
            else if (!wasActive && willBeActive)
            {
                existingDesignation.ReactivatedOn = DateTime.UtcNow;
            }

            var updatedDesignation = await _designationRepository.UpdateDesignationAsync(existingDesignation);
            
            // Log the update operation with IP address
            await _logsService.LogUpdateAsync(oldDesignation, updatedDesignation, updateDesignationDto.UpdatedBy ?? "System", ipAddress);
            
            return MapToResponseDto(updatedDesignation);
        }

        public async Task<DesignationResponseDto> DeactivateDesignationAsync(int designationId, string deactivatedBy, string? ipAddress = null)
        {
            var existingDesignation = await _designationRepository.GetDesignationByIdAsync(designationId);
            if (existingDesignation == null)
            {
                throw new InvalidOperationException($"Designation with ID {designationId} not found.");
            }

            if (!existingDesignation.IsActive)
            {
                throw new InvalidOperationException($"Designation with ID {designationId} is already deactivated.");
            }

            existingDesignation.IsActive = false;
            existingDesignation.DeactivatedOn = DateTime.UtcNow;
            existingDesignation.UpdatedDate = DateTime.UtcNow;
            existingDesignation.UpdatedBy = deactivatedBy;

            var updatedDesignation = await _designationRepository.UpdateDesignationAsync(existingDesignation);
            
            return MapToResponseDto(updatedDesignation);
        }

        public async Task<DesignationResponseDto> ReactivateDesignationAsync(int designationId, string reactivatedBy, string? ipAddress = null)
        {
            var existingDesignation = await _designationRepository.GetDesignationByIdAsync(designationId);
            if (existingDesignation == null)
            {
                throw new InvalidOperationException($"Designation with ID {designationId} not found.");
            }

            if (existingDesignation.IsActive)
            {
                throw new InvalidOperationException($"Designation with ID {designationId} is already active.");
            }

            existingDesignation.IsActive = true;
            existingDesignation.ReactivatedOn = DateTime.UtcNow;
            existingDesignation.UpdatedDate = DateTime.UtcNow;
            existingDesignation.UpdatedBy = reactivatedBy;

            var updatedDesignation = await _designationRepository.UpdateDesignationAsync(existingDesignation);
            
            return MapToResponseDto(updatedDesignation);
        }

        public async Task<bool> DeleteDesignationAsync(int designationId, string performedBy = "System", string? ipAddress = null)
        {
            try
            {
                var existingDesignation = await _designationRepository.GetDesignationByIdAsync(designationId);
                if (existingDesignation == null)
                {
                    return false;
                }

                var isDeleted = await _designationRepository.DeleteDesignationAsync(designationId);
                
                if (isDeleted)
                {
                    // Log the deletion with IP address
                    await _logsService.LogDeleteAsync(existingDesignation, performedBy, ipAddress);
                }

                return isDeleted;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the designation.", ex);
            }
        }

        private static DesignationResponseDto MapToResponseDto(MasterDesignation designation)
        {
            return new DesignationResponseDto
            {
                DesignationId = designation.DesignationId,
                DesignationName = designation.DesignationName,
                Rank = designation.Rank,
                Abbreviation = designation.Abbreviation,
                IsActive = designation.IsActive,
                CreatedDate = designation.CreatedDate,
                CreatedBy = designation.CreatedBy,
                UpdatedDate = designation.UpdatedDate,
                UpdatedBy = designation.UpdatedBy,
                DeactivatedOn = designation.DeactivatedOn,
                ReactivatedOn = designation.ReactivatedOn
            };
        }
    }
}
