using OfficerService.Models;
using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IMasterRoleService
    {
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<RoleResponseDto?> GetRoleByIdAsync(int roleId);
        Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync();
        Task<IEnumerable<RoleResponseDto>> GetActiveRolesAsync();
        Task<RoleResponseDto> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto, string? ipAddress = null);
        Task<bool> DeleteRoleAsync(int roleId, string performedBy = "System", string? ipAddress = null);
        Task<RoleResponseDto> DeactivateRoleAsync(int roleId, string deactivatedBy, string? ipAddress = null);
        Task<RoleResponseDto> ReactivateRoleAsync(int roleId, string reactivatedBy, string? ipAddress = null);
    }
}