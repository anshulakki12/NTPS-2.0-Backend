using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationService.Models
{
    [Table("Verify_Otp")]
    public class VerifyOtp
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Mobile_No", TypeName = "nvarchar(15)")]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Mobile number must be between 10 and 15 digits.")]
        public string MobileNo { get; set; } = string.Empty;

        [Column("Application_Id", TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Application ID is required.")]
        [StringLength(50, ErrorMessage = "Application ID cannot exceed 50 characters.")]
        public string ApplicationId { get; set; } = string.Empty;

        [Column("OTP_Cases_Id", TypeName = "int")]
        [Required(ErrorMessage = "OTP case ID is required.")]
        public int? OtpCasesId { get; set; }

        [Column("Status", TypeName = "char(1)")]
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"[A-Za-z]", ErrorMessage = "Status must be a single alphabetic character (e.g., 'S', 'F').")]
        public string Status { get; set; } = "N"; // N = New, S = Success, F = Failed, etc.

        [Column("Created_Date", TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Updated_Date", TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [Column("OTP", TypeName = "nvarchar(6)")]
        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be between 4 and 6 characters.")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "OTP must be numeric and 4–6 digits long.")]
        public string Otp { get; set; } = string.Empty;

        [Column("Attempt", TypeName = "int")]
        [Range(0, 10, ErrorMessage = "Attempt count must be between 0 and 10.")]
        public int Attempt { get; set; } = 0;
    }
}
