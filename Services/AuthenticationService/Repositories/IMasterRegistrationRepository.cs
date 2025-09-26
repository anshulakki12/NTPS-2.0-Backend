using AuthenticationService.Models;

namespace AuthenticationService.Repositories
{
    public interface IMasterRegistrationRepository
    {
        Task<IEnumerable<MasterRegistration>> GetAllAsync();
        Task<MasterRegistration> AddAsync(MasterRegistration registration);
        Task<MasterRegistration?> GetByIdAsync(int id); // ✅ Add this method
        Task<MasterRegistration> UpdateAsync(MasterRegistration registration);
        Task<bool> DeleteAsync(int id);
        Task<MasterRegistration?> GetByLoginIdAsync(string loginId);
        Task<ApplicantPersonalDetails> AddAsyncApplicant(ApplicantPersonalDetails applicantpersonaldetails);
        Task<ApplicantPersonalDetails?> GetApplicantPersonalDetailsByLoginIdAsync(string loginId);


    }
}
