using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("TP_Status_Multiple")]
    public class TpStatusMultiple
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Multiple_Status_Id")]
        public int MultipleStatusId { get; set; }

        [Column("Registration_No")]
        [Required(ErrorMessage = "Registration No is required.")]
        [StringLength(50, ErrorMessage = "Registration No cannot exceed 50 characters.")]
        public string? RegistrationNo { get; set; }

        [Column("Login_id_from")]
        [StringLength(50, ErrorMessage = "Login From Id cannot exceed 50 characters.")]
        public string? LoginIdFrom { get; set; }

        [Column("Login_id_to")]
        [StringLength(50, ErrorMessage = "Login To Id cannot exceed 50 characters.")]
        public string? LoginIdTo { get; set; }

        [Column("Status")]
        public int? Status { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("remarks")]
        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }
    }
}
