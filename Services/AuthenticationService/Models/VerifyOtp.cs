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

        [Column("Mobile_No")]
        [StringLength(50)]
        public string? MobileNo { get; set; }

        [Column("Application_Id")]
        [StringLength(50)]
        public string? ApplicationId { get; set; }

        [Column("OTP_Cases_Id")]
        public int? OtpCasesId { get; set; }

        [Column("Status")]
        [StringLength(1)]
        public string? Status { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("OTP")]
        [StringLength(6)]
        public string? Otp { get; set; }

        [Column("Attempt")]
        public int Attempt { get; set; }
    }
}
