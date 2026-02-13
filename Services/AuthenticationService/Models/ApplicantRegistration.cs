using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationService.Models
{
    [Table("Applicant_Registration")]
    public class ApplicantRegistration
    {
        [Key]
        [Column("Registration_id")]
        public int RegistrationId { get; set; }

        [Column("Name_Title", TypeName = "nvarchar(10)")]
        [StringLength(10, ErrorMessage = "Title cannot exceed 10 characters.")]
        public string? NameTitle { get; set; } = string.Empty;

        [Column("Name", TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Column("Email_id", TypeName = "nvarchar(150)")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string EmailId { get; set; } = string.Empty;

        [Column("Login_Id", TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Login ID is required.")]
        [StringLength(50, ErrorMessage = "Login ID cannot exceed 50 characters.")]
        public string LoginId { get; set; } = string.Empty;

        [Column("Password", TypeName = "nvarchar(256)")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Column("Mobile_No", TypeName = "nvarchar(15)")]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Mobile number must be between 10 and 15 digits.")]
        public string MobileNo { get; set; } = string.Empty;

        [Column("Is_Verified", TypeName = "char(1)")]
        [RegularExpression(@"[YN]", ErrorMessage = "Value must be 'Y' or 'N'.")]
        public char? IsVerified { get; set; }

        [Column("Created_Date", TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Registration_Type", TypeName = "nvarchar(50)")]
        [StringLength(50, ErrorMessage = "Registration type cannot exceed 50 characters.")]
        public string? RegistrationType { get; set; } = string.Empty;

        [Column("UserRole", TypeName = "int")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user role.")]
        public int? UserRole { get; set; } = 28;

        [Column("LoginSource", TypeName = "nvarchar(20)")]
        [StringLength(20, ErrorMessage = "Login source cannot exceed 20 characters.")]
        public string? LoginSource { get; set; } = "Web";
    }
}
