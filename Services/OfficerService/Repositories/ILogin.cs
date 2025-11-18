using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface ILogin
    {
        Task<OfficerRegistration> GetByLoginIdAsync(string loginId);
        Task<OfficerRegistration?> GetByLoginIdOrNullAsync(string loginId);
        Task<bool> ValidateOfficerCredentialsAsync(string loginId, string hashedPassword);
    }
}
