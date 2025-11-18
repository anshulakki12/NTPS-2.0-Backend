using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Officer_Registration")]
    public class OfficerRegistration
    {
        [Key]
        [Column("Officer_Id")]
        public int OfficerId { get; set; }

        [Column("Login_Id")]
        [Required]
        [RegularExpression("^[A-Za-z0-9_]+$", ErrorMessage = "LoginId must be alphanumeric (underscores allowed)")]
        [MaxLength(50, ErrorMessage = "LoginId cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "LoginId must be at least 3 characters long")]
        public string LoginId { get; set; } = string.Empty;

        [Column("Password")]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", ErrorMessage = "Password must contain an uppercase letter, a number, and a special character")]
        public string Password { get; set; } = string.Empty;

        [Column("Mobile_No")]
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")] 
        public string MobileNo { get; set; } = string.Empty;

        [Column("Location_id")]
        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }

        [Column("Location_Type")]
        [Required(ErrorMessage = "Location type is required")]
        public int LocationType { get; set; }

        [Column("isActive")]
        [Required]
        public bool IsActive { get; set; } = true;

        [Column("role_id")]
        [Required(ErrorMessage ="Role is required")]
        public int RoleId { get; set; }

        [Column("designation_id")]
        public int? DesignationId { get; set; }

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Created_By")]
        public string? CreatedBy { get; set; }
        
        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
       
        [Column("Updated_By")]
        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual OfficerDetails? OfficerDetails { get; set; }

        // Navigation property for MasterRoles
        [ForeignKey("RoleId")]
        public virtual MasterRoles? Role { get; set; }

        // Navigation property for MasterDesignation
        [ForeignKey("DesignationId")]
        public virtual MasterDesignation? Designation { get; set; }
    }
}
