using Microsoft.EntityFrameworkCore;
using SpeciesService.Models;

namespace SpeciesService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ForestProduce> ForestProduces { get; set; }
        public DbSet<MasterSpecies> masterSpecies { get; set; }
        public DbSet<SpeciesExempted> speciesExempted { get; set; }
    }
}
