using ApplicantAuthenticationService.Models;
using AuthenticationService.Data;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Repositories
{
    public class ApplicantRegistrationRepository : IApplicantRegistrationRepository
    {
        private readonly AppDbContext _context;

        public ApplicantRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<ApplicantRegistration>> GetAllAsync()
        {
            return await _context.MasterRegistrations.ToListAsync();
        }

        public async Task<ApplicantRegistration?> GetByIdAsync(int id) // ✅ Implementation
        {
            return await _context.MasterRegistrations
                                 .FirstOrDefaultAsync(r => r.RegistrationId == id);
        }

        public async Task<ApplicantRegistration> AddAsync(ApplicantRegistration registration)
        {
            _context.MasterRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task<ApplicantRegistration> UpdateAsync(ApplicantRegistration registration)
        {
            _context.MasterRegistrations.Update(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.MasterRegistrations.FindAsync(id);
            if (entity == null) return false;

            _context.MasterRegistrations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplicantRegistration?> GetByLoginIdAsync(string loginId)
        {
            return await _context.MasterRegistrations
                .FirstOrDefaultAsync(r => r.LoginId == loginId);
        }

        public async Task<ApplicantPersonalDetails> AddAsyncApplicant(ApplicantPersonalDetails details)
        {
            _context.ApplicantPersonalDetails.Add(details);
            await _context.SaveChangesAsync();
            return details;
        }

        public async Task<ApplicantPersonalDetails?> GetApplicantPersonalDetailsByLoginIdAsync(string loginId)
        {
            return await _context.ApplicantPersonalDetails
                                 .FirstOrDefaultAsync(x => x.LoginId == loginId);
        }

        public async Task<ApplicantRegistration?> GetApplicantLoggedPersonalDetailsByLoginIdAsync(string loginId)
        {
            return await _context.MasterRegistrations
                                 .FirstOrDefaultAsync(x => x.LoginId == loginId);
        }

        public async Task<ApplicantPersonalDetails> UpdateApplicantAsync(ApplicantPersonalDetails applicantPersonalDetails)
        {
            _context.ApplicantPersonalDetails.Update(applicantPersonalDetails);
            await _context.SaveChangesAsync();
            return applicantPersonalDetails;
        }

        public async Task<int> GetRecentPasswordHistoryAsync(string loginId, TimeSpan withinTime)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(withinTime);
            return await _context.PasswordHistory
                .Where(x => x.LoginId == loginId && x.CreatedDate >= cutoffTime)
                .CountAsync();
        }

        public async Task AddPasswordHistoryAsync(string loginId, string passwordHash)
        {
            var history = new PasswordHistory
            {
                LoginId = loginId,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.UtcNow
            };
            _context.PasswordHistory.Add(history);
            await _context.SaveChangesAsync();
        }

    }
}
