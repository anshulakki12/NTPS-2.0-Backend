using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicantAuthenticationService.Models
{
    [Table("Password_History")]
    public class PasswordHistory
    {
        [Key]
        [Column("History_id")]
        public int HistoryId { get; set; }

        [Column("Login_Id", TypeName = "nvarchar(50)")]
        [Required]
        [StringLength(50)]
        public string LoginId { get; set; } = string.Empty;

        [Column("Password_Hash", TypeName = "nvarchar(256)")]
        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("Created_Date", TypeName = "datetime")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
