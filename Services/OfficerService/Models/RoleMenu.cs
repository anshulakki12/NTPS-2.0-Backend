using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Role_Menus")]
    public class RoleMenu
    {
        [Key]
        [Column("Role_Menu_Id")]
        public int RoleMenuId { get; set; }

        [Required]
        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Required]
        [Column("Menu_Id")]
        public int MenuId { get; set; }

        [Column("Display_Order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("Is_Visible")]
        public bool IsVisible { get; set; } = true;

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
        public virtual MasterRoles Role { get; set; } = null!;

        [ForeignKey("MenuId")]
        public virtual MasterMenu Menu { get; set; } = null!;
    }
}
