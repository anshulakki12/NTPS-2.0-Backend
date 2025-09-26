using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AuthenticationService.Enums.ApplicantEnums;

namespace AuthenticationService.Models
{
    [Table("Applicant_Personal_Details")]   // Table name in your DB
    public class ApplicantPersonalDetails
    {
        [Key]
        [Column("Details_id")]
        public int DetailsId { get; set; }

        [Column("Login_id")]
        public string? LoginId { get; set; }

        [Column("ID_Proof")]
        public IdentityProof IDProof { get; set; }   // ✅ Enum

        [Column("ID_Number")]
        public string? IDNumber { get; set; }

        [Column("ID_Upload")]
        public string? IDUpload { get; set; }

        [Column("State_Id")]
        public int? StateId { get; set; }

        [Column("Circle_id")]
        public int? CircleId { get; set; }

        [Column("Division_id")]
        public int? DivisionId { get; set; }

        [Column("Sub_division_id")]
        public int? SubDivisionId { get; set; }

        [Column("Range_Id")]
        public int? RangeId { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("Pin_Code")]
        public string? PinCode { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Modification_Date")]
        public DateTime? ModificationDate { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Email")]
        public string? Email { get; set; }

        [Column("Source_Type")]
        public SourceType SourceType { get; set; }   // ✅ Enum
    }
}
