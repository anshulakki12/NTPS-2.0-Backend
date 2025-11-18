using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Application_Details")]
    public class ApplicationDetail
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("Application_ID")]
        [ForeignKey("ApplicationMaster")]
        public long? ApplicationId { get; set; }

        [Column("Registration_No")]
        [MaxLength(200)]
        public string? RegistrationNo { get; set; }

        [Column("Application_Type")]
        [ForeignKey("ApplicationCategory")]
        public int? ApplicationCateogryId { get; set; }

        [Column("Createdby_UserID", TypeName = "nvarchar(50)")]
        public string? CreateByUserId { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        // 🔗 Navigation properties
        public ApplicationMaster? ApplicationMaster { get; set; }
        public ApplicationCategory? ApplicationCategory { get; set; }
    }
}
