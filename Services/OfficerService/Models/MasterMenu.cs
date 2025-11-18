using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Menus")]
    public class MasterMenu
    {
        [Key]
        [Column("Menu_Id")]
        public int MenuId { get; set; }

        [Required]
        [Column("Menu_Name")]
        [MaxLength(100)]
        public string MenuName { get; set; } = string.Empty;

        [Required]
        [Column("Menu_Code")]
        [MaxLength(50)]
        public string MenuCode { get; set; } = string.Empty;

        [Required]
        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Column("Module_Id")]
        public int? ModuleId { get; set; }

        [Column("Parent_Menu_Id")]
        public int? ParentMenuId { get; set; }

        [Required]
        [Column("Label")]
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [Column("Icon")]
        [MaxLength(50)]
        public string? Icon { get; set; }

        [Column("Router_Link")]
        [MaxLength(200)]
        public string? RouterLink { get; set; }

        [Column("Url")]
        [MaxLength(500)]
        public string? Url { get; set; }

        [Column("Target")]
        [MaxLength(20)]
        public string? Target { get; set; }

        [Column("Is_Parent")]
        public bool IsParent { get; set; } = false;

        [Column("Has_Children")]
        public bool HasChildren { get; set; } = false;

        [Column("Description")]
        [MaxLength(500)]
        public string? Description { get; set; }

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

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual MasterRoles? Role { get; set; }

        [ForeignKey("ModuleId")]
        public virtual MasterModule? Module { get; set; }

        [ForeignKey("ParentMenuId")]
        public virtual MasterMenu? ParentMenu { get; set; }

        public virtual ICollection<MasterMenu> ChildMenus { get; set; } = new List<MasterMenu>();
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}
