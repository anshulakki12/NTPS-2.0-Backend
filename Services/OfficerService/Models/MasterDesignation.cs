using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Designation")]
    public class MasterDesignation
    {
        [Key]
        [Column("Designation_Id")]
        public int DesignationId { get; set; }

        [Column("Designation_Name")]
        [Required]
        [MaxLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        [Column("Rank")]
        [Required]
        public int Rank { get; set; }

        [Column("Abbreviation")]
        [Required]
        [MaxLength(20)]
        public string Abbreviation { get; set; } = string.Empty;

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Created_By")]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Updated_By")]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [Column("Deactivated_On")]
        public DateTime? DeactivatedOn { get; set; }

        [Column("Reactivated_On")]
        public DateTime? ReactivatedOn { get; set; }

        // Navigation property for related officers
        public virtual ICollection<OfficerRegistration> OfficerRegistrations { get; set; } = new List<OfficerRegistration>();
    }
}
