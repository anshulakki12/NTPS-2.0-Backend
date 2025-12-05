using MasterLoginService.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterLoginService.Data
{
    public class MasterLoginDbContext : DbContext
    {
        public MasterLoginDbContext(DbContextOptions<MasterLoginDbContext> options)
            : base(options)
        {
        }

        // Example tables
        public DbSet<MasterUser> MasterUsers { get; set; }
       // public DbSet<UserRole> UserRoles { get; set; }
    }
}
