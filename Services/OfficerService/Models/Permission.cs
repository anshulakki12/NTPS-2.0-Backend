using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        [Column("Permission_Id")]
        public int PermissionId { get; set; }

        [Required]
        [Column("Permission_Code")]
        [MaxLength(100)]
        public string PermissionCode { get; set; } = string.Empty;

        [Required]
        [Column("Permission_Name")]
        [MaxLength(200)]
        public string PermissionName { get; set; } = string.Empty;

        [Column("Description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column("Module")]
        [MaxLength(100)]
        public string Module { get; set; } = string.Empty;

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Created_By")]
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Updated_By")]
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
