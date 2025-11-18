using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AuthenticationService.Enums.ApplicantEnums;

namespace AuthenticationService.Models
{
    [Table("Applicant_Personal_Details")]
    public class ApplicantPersonalDetails
    {
        [Key]
        [Column("Details_id")]
        public int DetailsId { get; set; }

        [Column("Login_id", TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Login ID is required.")]
        [StringLength(50, ErrorMessage = "Login ID cannot exceed 50 characters.")]
        public string LoginId { get; set; } = string.Empty;

        [Column("ID_Proof", TypeName = "int")]
        [Required(ErrorMessage = "ID Proof type is required.")]
        public IdentityProof IDProof { get; set; }

        [Column("ID_Number", TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(50, ErrorMessage = "ID Number cannot exceed 50 characters.")]
        public string IDNumber { get; set; } = string.Empty;

        [Column("ID_Upload", TypeName = "nvarchar(255)")]
        [StringLength(255, ErrorMessage = "File path cannot exceed 255 characters.")]
        public string? IDUpload { get; set; } = string.Empty;

        [Column("State_Id", TypeName = "int")]
        [Required(ErrorMessage = "State ID is required.")]
        public int? StateId { get; set; }

        [Column("Circle_id", TypeName = "int")]
        public int? CircleId { get; set; }

        [Column("Division_id", TypeName = "int")]
        public int? DivisionId { get; set; }

        [Column("Sub_division_id", TypeName = "int")]
        public int? SubDivisionId { get; set; }

        [Column("Range_Id", TypeName = "int")]
        public int? RangeId { get; set; }

        [Column("Address", TypeName = "nvarchar(250)")]
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = string.Empty;

        [Column("Pin_Code", TypeName = "nvarchar(10)")]
        [Required(ErrorMessage = "Pin Code is required.")]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Pin Code must be between 5 and 10 digits.")]
        public string PinCode { get; set; } = string.Empty;

        [Column("Created_Date", TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("Modification_Date", TypeName = "datetime")]
        public DateTime? ModificationDate { get; set; }

        [Column("Name", TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Column("Email", TypeName = "nvarchar(150)")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Column("Source_Type", TypeName = "int")]
        [Required(ErrorMessage = "Source Type is required.")]
        public SourceType SourceType { get; set; }
    }
}
