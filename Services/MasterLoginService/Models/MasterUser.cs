// MasterLoginService/Models/MasterUser.cs
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

        [Required(ErrorMessage = "LoginId is required.")]
        [StringLength(50, ErrorMessage = "LoginId cannot exceed 50 characters.")]
        [Column("Login_Id")]
        public string LoginId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(15)]
        [Column("Mobile_No")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("User_Type")]
        public string UserType { get; set; } = "Applicant"; // Changed to string

        [StringLength(50)]
        [Column("Registration_Type")]
        public string RegistrationType { get; set; } = string.Empty;

        [Column("Is_Verified", TypeName = "char(1)")]
        public char IsVerified { get; set; } = 'N';

        [Column("Created_Date", TypeName = "datetime")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Last_Updated", TypeName = "datetime")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("Is_Active", TypeName = "bit")]
        public bool IsActive { get; set; } = true;
    }

    // Keep the enum for internal use
    public enum UserTypeEnum
    {
        Officer = 1,
        Applicant = 2,
        Revenue = 3,
        Enumerator = 4
    }

    // Helper class for enum-string conversion
    public static class UserTypeConverter
    {
        public static string EnumToString(UserTypeEnum userType)
        {
            return userType.ToString();
        }

        public static UserTypeEnum StringToEnum(string userTypeString)
        {
            if (Enum.TryParse<UserTypeEnum>(userTypeString, out var result))
            {
                return result;
            }

            // Default to Applicant if parsing fails
            return UserTypeEnum.Applicant;
        }

        public static string GetPrefix(string userTypeString)
        {
            var userType = StringToEnum(userTypeString);
            return userType switch
            {
                UserTypeEnum.Applicant => "AP",
                UserTypeEnum.Officer => "OF",
                UserTypeEnum.Enumerator => "EN",
                UserTypeEnum.Revenue => "RE",
                _ => string.Empty
            };
        }
    }
}