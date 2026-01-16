using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    // Models/DocumentMaster.cs
    public class DocumentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Document_Master_Id")]
        public int DocumentMasterId { get; set; }

        [Required(ErrorMessage = "StateCode is required.")]
        [Column("State_Code")]
        public int StateCode { get; set; }

        [Column("State_Name")]
        [StringLength(50, ErrorMessage = "StateName cannot exceed 50 characters.")]
        public string? StateName { get; set; }

        [Column("Document_Type_Id")]
        [Required(ErrorMessage = "DocumentTypeId is required.")]
        public string? DocumentTypeId { get; set; }

        [Column("Document_Type_Name")]
        [Required(ErrorMessage = "DocumentTypeName is required.")]
        public string? DocumentTypeName { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; }

        [Column("Is_Mandatory")]
        public bool IsMandatory { get; set; }
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; }

        [Column("Created_By")]
        public string? CreatedBy { get; set; }

        [Column("Modified_Date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("Modified_By")]
        public string? ModifiedBy { get; set; }
    }
}
