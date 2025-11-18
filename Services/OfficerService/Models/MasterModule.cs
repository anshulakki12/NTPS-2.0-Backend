using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Modules")]
    public class MasterModule
    {
        [Key]
        [Column("Module_Id")]
        public int ModuleId { get; set; }

        [Required]
        [Column("Module_Name")]
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        [Column("Module_Code")]
        [MaxLength(50)]
        public string ModuleCode { get; set; } = string.Empty;

        [Column("Description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("Icon")]
        [MaxLength(50)]
        public string? Icon { get; set; }

        [Column("Route_Path")]
        [MaxLength(200)]
        public string? RoutePath { get; set; }

        [Required]
        [Column("Display_Order")]
        public int DisplayOrder { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Created_By")]
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [Column("Modified_Date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("Modified_By")]
        [MaxLength(50)]
        public string? ModifiedBy { get; set; }

        [Column("Deactivated_On")]
        public DateTime? DeactivatedOn { get; set; }

        [Column("Reactivated_On")]
        public DateTime? ReactivatedOn { get; set; }
    }
}
