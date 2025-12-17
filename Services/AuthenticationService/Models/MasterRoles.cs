using AuthenticationService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicantAuthenticationService.Models
{
    [Table("Master_Roles")]
    public class MasterRoles
    {
        [Key]
        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Column("Role_Name")]
        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;

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
        public virtual ICollection<ApplicantRegistration> ApplicantRegistrations { get; set; } = new List<ApplicantRegistration>();
    }
}
