using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Photo_Forest_Produce")]
    public class PhotoForestProduce
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Photo_ID")]
        public int PhotoId { get; set; }

        [Required(ErrorMessage = "Registration No is required.")]
        [Column("Registration_No")]
        [StringLength(50, ErrorMessage = "Registration No cannot exceed 50 characters.")]
        public string RegistrationNo { get; set; } = null!;

        [Required(ErrorMessage = "Photo upload path is required.")]
        [Column("Photo_Upload")]
        [StringLength(150, ErrorMessage = "Photo upload path cannot exceed 150 characters.")]
        public string PhotoUpload { get; set; } = null!;

        [Required(ErrorMessage = "Source type is required.")]
        [Column("Source_type")]
        [StringLength(150, ErrorMessage = "Source type cannot exceed 150 characters.")]
        public string SourceType { get; set; } = null!;

        [Required]
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
