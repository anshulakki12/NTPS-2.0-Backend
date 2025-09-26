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
        public DbSet<MasterRegistration> MasterRegistrations { get; set; }

        public DbSet<VerifyOtp> VerifyOtps { get; set; }
        public DbSet<ApplicantPersonalDetails> ApplicantPersonalDetails { get; set; }
    }
}
