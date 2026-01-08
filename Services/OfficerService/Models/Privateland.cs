using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Private_land")]
    public class Privateland
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Registration No is required.")]
        [Column("Registration_No")]
        [StringLength(50, ErrorMessage = "Registration No cannot exceed 50 characters.")]
        public string RegistrationNo { get; set; } = null!;

        [Required(ErrorMessage = "Survey No is required.")]
        [Column("Survey_No")]
        [StringLength(150, ErrorMessage = "Survey No cannot exceed 150 characters.")]
        public string SurveyNo { get; set; } = null!;

        [Column("Patta_Details")]
        [StringLength(150, ErrorMessage = "Patta Details cannot exceed 150 characters.")]
        public string? PattaDetails { get; set; }

        [Required]
        [Column("Type")]
        [StringLength(20, ErrorMessage = "Type cannot exceed 20 characters.")]
        public string Type { get; set; } = null!;

        [Required]
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }
}
