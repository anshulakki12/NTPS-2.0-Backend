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

        [Required(ErrorMessage = "Document Type is required.")]
        [Column("Document_Type")]
        [StringLength(20, ErrorMessage = "Document Type cannot exceed 20 characters.")]
        public string DocumentType { get; set; } = null!; // DOC001 or DOC004

        [Column("Photo_Upload")]
        [StringLength(500, ErrorMessage = "File name cannot exceed 500 characters.")]
        public string? PhotoUpload { get; set; } = null!; // Comma-separated file names

        [Required(ErrorMessage = "Source type is required.")]
        [Column("Source_type")]
        [StringLength(150, ErrorMessage = "Source type cannot exceed 150 characters.")]
        public string SourceType { get; set; } = "web"; // Default to "web"

        [Required]
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;
        [Column("Other_Document")]
        public string? OtherDocument { get; set; }
        [Column("Application_Id")]
        public int ApplicationId { get; set; }
        [Column("Category_Id")]
        public int CategoryId { get; set; }
    }
}