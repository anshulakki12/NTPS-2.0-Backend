using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterLoginService.Models
{
    [Table("Master_User")]
    public class MasterUser
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        // LOGIN ID (UNIQUE)
        [Required(ErrorMessage = "LoginId is required.")]
        [StringLength(50, ErrorMessage = "LoginId cannot exceed 50 characters.")]
        [Column("Login_Id")]
        public string LoginId { get; set; } = string.Empty;

        // MOBILE NO
        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid mobile number.")]
        [StringLength(15)]
        [Column("Mobile_No")]
        public string MobileNo { get; set; } = string.Empty;

        // EMAIL
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        // USER TYPE ENUM
        [Required]
        [Column("User_Type")]
        public UserType UserType { get; set; }
    }

    public enum UserType
    {
        Officer = 1,
        Applicant = 2,
        Revenue = 3,
        Enumerator = 4
    }
}
