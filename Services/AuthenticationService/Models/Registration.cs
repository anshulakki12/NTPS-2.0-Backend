using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationService.Models
{
    [Table("Master_Registration")]
    public class MasterRegistration
    {
        [Key]
        [Column("Registration_id")]
        public int RegistrationId { get; set; }

        [Column("Name_Title")]
        public string? NameTitle { get; set; } = string.Empty;

        [Column("Name")]
        public string? Name { get; set; } = string.Empty;

        [Column("Email_id")]
        public string? EmailId { get; set; } = string.Empty;

        [Column("Login_Id")]
        public string? LoginId { get; set; } = string.Empty;

        [Column("Password")]
        public string? Password { get; set; } = string.Empty;

        [Column("Mobile_No")]
        public string? MobileNo { get; set; } = string.Empty;

        [Column("Is_Verified")]
        public char? IsVerified { get; set; }     // nullable since DB allows NULL

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Registration_Type")]
        public string? RegistrationType { get; set; } = string.Empty;  // <-- New column
        [Column("UserRole")]
        public string? UserRole { get; set; } = "Applicant";  // Default value

        [Column("LoginSource")]
        public string? LoginSource { get; set; } = "Web";     // Values: "Web", "Phone"
    }
}
