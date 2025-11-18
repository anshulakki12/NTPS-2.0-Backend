using AuthenticationService.Models;

namespace AuthenticationService.Repositories
{
    public interface IApplicantRegistrationRepository
    {
        Task<IEnumerable<ApplicantRegistration>> GetAllAsync();
        Task<ApplicantRegistration> AddAsync(ApplicantRegistration registration);
        Task<ApplicantRegistration?> GetByIdAsync(int id); // ✅ Add this method
        Task<ApplicantRegistration> UpdateAsync(ApplicantRegistration registration);
        Task<bool> DeleteAsync(int id);
        Task<ApplicantRegistration?> GetByLoginIdAsync(string loginId);
        Task<ApplicantPersonalDetails> AddAsyncApplicant(ApplicantPersonalDetails applicantpersonaldetails);
        Task<ApplicantPersonalDetails?> GetApplicantPersonalDetailsByLoginIdAsync(string loginId);
        Task<ApplicantRegistration?> GetApplicantLoggedPersonalDetailsByLoginIdAsync(string loginId);

        Task<ApplicantPersonalDetails> UpdateApplicantAsync(ApplicantPersonalDetails applicantpersonaldetails);
        Task<int> GetRecentPasswordHistoryAsync(string loginId, TimeSpan withinTime);
        Task AddPasswordHistoryAsync(string loginId, string passwordHash);


    }
}
