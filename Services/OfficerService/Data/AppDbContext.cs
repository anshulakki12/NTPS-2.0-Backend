using System;
using Microsoft.EntityFrameworkCore;
using OfficerService.Models;

namespace OfficerService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        // DbSets (Tables)
        public DbSet<MasterRoles> MasterRoles { get; set; }
        public DbSet<OfficerRegistration> OfficerRegistrations { get; set; }
        public DbSet<OfficerDetails> OfficerDetails { get; set; }
        public DbSet<MasterRolesLogs> MasterRolesLogs { get; set; }
        public DbSet<MasterDesignation> MasterDesignations { get; set; }
        public DbSet<MasterDesignationLogs> MasterDesignationLogs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<MasterModule> MasterModules { get; set; }
        public DbSet<MasterMenu> MasterMenus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<MasterZone> MasterZones { get; set; }
        public DbSet<ZoneData> ZoneData { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<SubDistrict> SubDistrict { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<MasterLevel> MasterLevels { get; set; }
        public DbSet<MasterWorkFlow> MasterWorkFlows { get; set; }
        public DbSet<WorkFlowSteps> WorkFlowSteps { get; set; }
        public DbSet<MasterSpecies> MasterSpecies { get; set; }
        public DbSet<ForestProduce> ForestProduce { get; set; }
        public DbSet<SpeciesMapping> SpeciesMapping { get; set; }
        public DbSet<MasterGovDepot> MasterGovDepots { get; set; }
        public DbSet<ApplicationCategory> ApplicationCategorys { get; set; }
        public DbSet<ApplicationDetail> ApplicationDetails { get; set; }
        public DbSet<ApplicationMaster> ApplicationMasters { get; set; }
        public DbSet<SpeciesLogsRoundTimber> SpeciesLogsRoundTimbers { get; set; }
        public DbSet<SpeciesLogsBamboo> SpeciesLogsBamboos { get; set; }
        public DbSet<SpeciesLogsFuelwood> SpeciesLogsFuelwoods { get; set; }
        public DbSet<SpeciesLogsMinorForestProduce> SpeciesLogsMinorForestProduces { get; set; }
        public DbSet<SpeciesLogsSawnTimber> SpeciesLogsSawnTimbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between OfficerRegistration and OfficerDetails
            modelBuilder.Entity<OfficerDetails>()
                .HasOne(od => od.OfficerRegistration)
                .WithOne(or => or.OfficerDetails)
                .HasForeignKey<OfficerDetails>(od => od.OfficerLoginId)
                .HasPrincipalKey<OfficerRegistration>(or => or.LoginId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between OfficerRegistration and MasterRoles
            modelBuilder.Entity<OfficerRegistration>()
                .HasOne(or => or.Role)
                .WithMany(mr => mr.OfficerRegistrations)
                .HasForeignKey(or => or.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between OfficerRegistration and MasterDesignation
            modelBuilder.Entity<OfficerRegistration>()
                .HasOne(or => or.Designation)
                .WithMany(md => md.OfficerRegistrations)
                .HasForeignKey(or => or.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraint on LoginId in OfficerRegistration
            modelBuilder.Entity<OfficerRegistration>()
                .HasIndex(or => or.LoginId)
                .IsUnique();

            // Configure the relationship between RolePermission and MasterRoles
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between RolePermission and Permission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraint on RoleId + PermissionId combination
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            // Configure unique constraint on Permission Code
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.PermissionCode)
                .IsUnique();

            // Configure unique constraint on Module Code
            modelBuilder.Entity<MasterModule>()
                .HasIndex(m => m.ModuleCode)
                .IsUnique();

            // Configure unique constraint on Menu Code
            modelBuilder.Entity<MasterMenu>()
                .HasIndex(m => m.MenuCode)
                .IsUnique();

            // Configure the required relationship between MasterMenu and MasterRoles
            modelBuilder.Entity<MasterMenu>()
                .HasOne(m => m.Role)
                .WithMany()
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configure the optional relationship between MasterMenu and MasterModule
            modelBuilder.Entity<MasterMenu>()
                .HasOne(m => m.Module)
                .WithMany()
                .HasForeignKey(m => m.ModuleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure self-referencing relationship for MasterMenu (Parent-Child)
            modelBuilder.Entity<MasterMenu>()
                .HasOne(m => m.ParentMenu)
                .WithMany(m => m.ChildMenus)
                .HasForeignKey(m => m.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between RoleMenu and MasterRoles
            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Role)
                .WithMany()
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between RoleMenu and MasterMenu
            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Menu)
                .WithMany(m => m.RoleMenus)
                .HasForeignKey(rm => rm.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraint on RoleId + MenuId combination
            modelBuilder.Entity<RoleMenu>()
                .HasIndex(rm => new { rm.RoleId, rm.MenuId })
                .IsUnique();

            // Disable cascade delete to avoid multiple cascade paths
            modelBuilder.Entity<ZoneData>()
                .HasOne(z => z.State)
                .WithMany()
                .HasForeignKey(z => z.StateID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZoneData>()
                .HasOne(z => z.District)
                .WithMany()
                .HasForeignKey(z => z.DistrictID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZoneData>()
                .HasOne(z => z.MasterZone)
                .WithMany()
                .HasForeignKey(z => z.ZoneID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZoneData>()
                .HasOne(z => z.SubDistrict)
                .WithMany()
                .HasForeignKey(z => z.SubDistrictID)
                .OnDelete(DeleteBehavior.Restrict);

            // Disable cascade delete on MasterZone → State
            modelBuilder.Entity<MasterZone>()
                .HasOne(z => z.State)
                .WithMany()
                .HasForeignKey(z => z.StateID)
                .OnDelete(DeleteBehavior.Restrict);

            // Disable cascade delete on SpeciesMapping → MasterZone
            modelBuilder.Entity<SpeciesMapping>()
                .HasOne(sm => sm.MasterZone)
                .WithMany()
                .HasForeignKey(sm => sm.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optionally disable others if needed
            modelBuilder.Entity<SpeciesMapping>()
                .HasOne(sm => sm.State)
                .WithMany()
                .HasForeignKey(sm => sm.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
