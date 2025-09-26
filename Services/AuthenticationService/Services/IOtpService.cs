using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    public interface IOtpService
    {
        Task SaveOtpAsync(string mobileNumber, string otp, DateTime expiry);
        Task<bool> ValidateOtpAsync(string mobileNumber, string otp);
        Task<VerifyOtp> AddAsync(VerifyOtp verifyotp);
        Task<VerifyOtp?> GetLastOtpRecordAsync(string mobileNumber);
        Task UpdateOtpRecordAsync(VerifyOtp verifyOtp
            
            
            );
    }
}
