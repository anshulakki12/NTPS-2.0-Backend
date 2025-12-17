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

        [Column("Menu_Name")]
        [Required]
        [MaxLength(100)]
        public string MenuName { get; set; } = string.Empty;

        [Column("Parent_Menu_Id")]
        public int? ParentMenuId { get; set; }

        [Column("Menu_Order")]
        public int MenuOrder { get; set; }

        [Column("Menu_Icon")]
        [MaxLength(50)]
        public string? MenuIcon { get; set; }

        [Column("Router_Link")]
        [MaxLength(200)]
        public string? RouterLink { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Created_By")]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        // Navigation property for parent menu
        [ForeignKey("ParentMenuId")]
        public virtual MasterMenu? ParentMenu { get; set; }

        // Navigation property for child menus
        public virtual ICollection<MasterMenu> ChildMenus { get; set; } = new List<MasterMenu>();

        // Navigation property for role mappings
        public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
    }
}
