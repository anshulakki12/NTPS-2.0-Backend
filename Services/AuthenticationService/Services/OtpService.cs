using AuthenticationService.Data;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;

        public OtpService(AppDbContext context)
        {
            _context = context;
        }
        // In-memory store for demo purposes
        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _store = new();

        //public Task SaveOtpAsync(string mobileNumber, string otp, DateTime expiry)
        //{
        //    _store[mobileNumber] = (otp, expiry);
        //    return Task.CompletedTask;
        //}

        //public Task<bool> ValidateOtpAsync(string mobileNumber, string otp)
        //{
        //    if (_store.TryGetValue(mobileNumber, out var data))
        //    {
        //        if (data.Otp == otp && DateTime.UtcNow <= data.Expiry)
        //            return Task.FromResult(true);
        //    }
        //    return Task.FromResult(false);
        //}

        //public async Task<VerifyOtp> AddAsync(VerifyOtp verifyotp)
        //{
        //    _context.VerifyOtps.Add(verifyotp);
        //    await _context.SaveChangesAsync();
        //    return verifyotp;
        //}

        public Task SaveOtpAsync(string mobileNumber, string otp, DateTime expiry)
        {
            _store[mobileNumber] = (otp, expiry);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateOtpAsync(string mobileNumber, string otp)
        {
            if (_store.TryGetValue(mobileNumber, out var data))
            {
                if (data.Otp == otp && DateTime.UtcNow <= data.Expiry)
                    return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task<VerifyOtp> AddAsync(VerifyOtp verifyotp)
        {
            _context.VerifyOtps.Add(verifyotp);
            await _context.SaveChangesAsync();
            return verifyotp;
        }

        // NEW METHOD — get the most recent OTP record for a mobile
        public async Task<VerifyOtp?> GetLastOtpRecordAsync(string mobileNumber)
        {
            return await _context.VerifyOtps
                .Where(v => v.MobileNo == mobileNumber)
                .OrderByDescending(v => v.CreatedDate)
                .FirstOrDefaultAsync();
        }

        // NEW METHOD — update an OTP record
        public async Task UpdateOtpRecordAsync(VerifyOtp verifyOtp)
        {
            _context.VerifyOtps.Update(verifyOtp);
            await _context.SaveChangesAsync();
        }

    }
}
