using AuthenticationService.Data;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Repositories
{
    public class MasterRegistrationRepository : IMasterRegistrationRepository
    {
        private readonly AppDbContext _context;

        public MasterRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<MasterRegistration>> GetAllAsync()
        {
            return await _context.MasterRegistrations.ToListAsync();
        }

        public async Task<MasterRegistration?> GetByIdAsync(int id) // ✅ Implementation
        {
            return await _context.MasterRegistrations
                                 .FirstOrDefaultAsync(r => r.RegistrationId == id);
        }

        public async Task<MasterRegistration> AddAsync(MasterRegistration registration)
        {
            _context.MasterRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        public async Task<MasterRegistration> UpdateAsync(MasterRegistration registration)
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

        public async Task<MasterRegistration?> GetByLoginIdAsync(string loginId)
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

    }
}
