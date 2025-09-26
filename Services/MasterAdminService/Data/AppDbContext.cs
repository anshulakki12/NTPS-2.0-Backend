using MasterAdminService.Models;
using Microsoft.EntityFrameworkCore;
using Range = MasterAdminService.Models.Range;

namespace MasterAdminService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // ✅ DbSets (Tables)
        public DbSet<State> States { get; set; }
        public DbSet<Circle> Circles { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<SubDivision> SubDivisions { get; set; }
        public DbSet<Range> Ranges { get; set; }

        // ✅ Relationships configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // State → Circles
            modelBuilder.Entity<Circle>()
                .HasOne(c => c.State)
                .WithMany()
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Circle → Divisions
            modelBuilder.Entity<Division>()
                .HasOne(d => d.Circle)
                .WithMany(c => c.Divisions)
                .HasForeignKey(d => d.CircleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Division → SubDivisions
            modelBuilder.Entity<SubDivision>()
                .HasOne(sd => sd.Division)
                .WithMany()
                .HasForeignKey(sd => sd.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            // SubDivision → Ranges
            modelBuilder.Entity<Range>()
                .HasOne(r => r.SubDivision)
                .WithMany(sd => sd.Ranges)
                .HasForeignKey(r => r.SubDivId)
                .OnDelete(DeleteBehavior.Restrict);

            // Division → Ranges
            modelBuilder.Entity<Range>()
                .HasOne(r => r.Division)
                .WithMany()
                .HasForeignKey(r => r.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
