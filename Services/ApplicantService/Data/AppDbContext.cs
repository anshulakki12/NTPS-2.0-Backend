
using Microsoft.EntityFrameworkCore;

namespace ApplicantService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
   

    }
}
