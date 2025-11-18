using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Role_Permissions")]
    public class RolePermission
    {
        [Key]
        [Column("Role_Permission_Id")]
        public int RolePermissionId { get; set; }

        [Required]
        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Required]
        [Column("Permission_Id")]
        public int PermissionId { get; set; }

        [Column("Assigned_Date")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Column("Assigned_By")]
        [MaxLength(50)]
        public string? AssignedBy { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual MasterRoles Role { get; set; } = null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
}
