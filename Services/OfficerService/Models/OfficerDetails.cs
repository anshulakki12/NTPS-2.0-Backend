using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Officer_Details")]
    public class OfficerDetails
    {
        [Key]
        [Column("Officer_Detail_Id")]
        public int OfficerDetailId { get; set; }

        [Column("Login_id")]
        [Required(ErrorMessage = "Officer Login ID is required")]
        [MaxLength(50, ErrorMessage = "LoginId cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "LoginId must be at least 3 characters long")]
        public string OfficerLoginId { get; set; } = string.Empty;

        [Column("Officer_Title")]
        [Required(ErrorMessage = "Officer Title is required")]
        [MaxLength(10, ErrorMessage = "Officer Title cannot exceed 100 characters")]
        public string OfficerTitle { get; set; } = string.Empty;

        [Column("Officer_Name")]
        [Required(ErrorMessage = "Officer Name is required")]
        [MaxLength(100, ErrorMessage = "Officer Name cannot exceed 100 characters")]
        public string OfficerName { get; set; } = string.Empty;

        [Column("Officer_Designation_Id")]
        [Required(ErrorMessage = "Officer Designation  required")] 
        public string OfficerDesignationId { get; set; } = string.Empty;

        [Column("Officer_Number")]
        [Required(ErrorMessage = "Contact Number is required")]
        [MaxLength(10, ErrorMessage = "Contact Number cannot exceed 15 characters")]
        public string OfficerNumber { get; set; } = string.Empty;

        [Column("Officer_Email_Address")]
        [MaxLength(100, ErrorMessage = "Email Address cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid Email Address format")]
        public string? EmailAddress { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
        
        // Navigation property to OfficerRegistration
        [ForeignKey("OfficerLoginId")]
        public virtual OfficerRegistration? OfficerRegistration { get; set; }
    }
}
