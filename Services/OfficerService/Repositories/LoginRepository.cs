using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using static OfficerService.DtoModels.OfficerLoginDto;

namespace OfficerService.Repositories
{
    public class LoginRepository : ILogin
    {
        private readonly AppDbContext _context;

        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<OfficerRegistration> GetByLoginIdAsync(string loginId)
        //{
        //    var officer = await _context.OfficerRegistrations
        //        .Include(o => o.OfficerDetails)
        //        .Include(o => o.Role)
        //        .FirstOrDefaultAsync(o => o.LoginId == loginId && o.IsActive);

        //    if (officer == null)
        //    {
        //        throw new InvalidOperationException($"Officer with LoginId '{loginId}' not found or is inactive.");
        //    }

        //    return officer;
        //}

        public async Task<OfficerRegistration> GetByLoginIdAsync(string loginId)
        {
            var officer = await _context.OfficerRegistrations
                .Include(o => o.OfficerDetails)
                .Include(o => o.Role)
                .FirstOrDefaultAsync(o => o.LoginId == loginId && o.IsActive);

            if (officer == null)
                return null;

            return new OfficerRegistration
            {
                OfficerId = officer.OfficerId,
                LoginId = officer.LoginId,
                MobileNo = officer.MobileNo,
                LocationId = officer.LocationId,
                LocationType = officer.LocationType,
                IsActive = officer.IsActive,
                RoleId = officer.RoleId,

                OfficerDetails = officer.OfficerDetails == null ? null : new OfficerDetails
                {
                    OfficerDetailId = officer.OfficerDetails.OfficerDetailId,
                    OfficerLoginId = officer.OfficerDetails.OfficerLoginId,
                    OfficerTitle = officer.OfficerDetails.OfficerTitle,
                    OfficerName = officer.OfficerDetails.OfficerName,
                    OfficerDesignationId = officer.OfficerDetails.OfficerDesignationId,
                    OfficerNumber = officer.OfficerDetails.OfficerNumber,
                    EmailAddress = officer.OfficerDetails.EmailAddress
                }
            };
        }


        public async Task<OfficerRegistration?> GetByLoginIdOrNullAsync(string loginId)
        {
            return await _context.OfficerRegistrations
                .Include(o => o.OfficerDetails)
                .Include(o => o.Role)
                .FirstOrDefaultAsync(o => o.LoginId == loginId && o.IsActive);
        }

        public async Task<bool> ValidateOfficerCredentialsAsync(string loginId, string hashedPassword)
        {
            var officer = await _context.OfficerRegistrations
                .FirstOrDefaultAsync(o => o.LoginId == loginId && o.IsActive);

            if (officer == null)
                return false;

            return string.Equals(officer.Password, hashedPassword, StringComparison.Ordinal);
        }
    }
}