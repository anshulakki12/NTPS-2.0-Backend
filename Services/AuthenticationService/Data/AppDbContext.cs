using ApplicantAuthenticationService.Models;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // This tells EF Core you have a table called MasterRegistration
        public DbSet<ApplicantRegistration> MasterRegistrations { get; set; }

        public DbSet<VerifyOtp> VerifyOtps { get; set; }
        public DbSet<ApplicantPersonalDetails> ApplicantPersonalDetails { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }
        public DbSet<ApplicantResourceCollection> ResourceCollection => Set<ApplicantResourceCollection>();
    }
}
