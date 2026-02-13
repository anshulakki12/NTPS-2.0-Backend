using OfficerService.DtoModels.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    // ApplicationMaster.cs
    [Table("Application_Master")]
    public class ApplicationMaster
    {
        [Key]
        [Column("Application_ID")]
        public long ApplicationId { get; set; }

        [Column("Status")]
        public bool? Status { get; set; }

        // Add Application_Status field to track open/in-progress status
        [Column("ApplicationStatus")]
        [Obsolete("Use ApplicationStatusId instead")]
        public string? ApplicationStatus { get; set; }

        [Column("Createdby_UserID", TypeName = "nvarchar(50)")]
        public string? CreateByUserId { get; set; }

        [Column("Createdby_UserName")]
        [MaxLength(100)]
        public string? CreateByUserName { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("State_ID")]
        public int? StateId { get; set; }   // stores ST_CODE

        [Column("District_ID")]
        public int? DistrictId { get; set; }

        [Column("SubDistrict_ID")]
        public int? SubDistrictId { get; set; }

        [Column("Forest_Produce_ID")]
        public int? ForestProduceId { get; set; }

        [Column("UpdatedBy_UserId")]
        public string? UpdatedByUserId { get; set; }

        [Column("Remarks")]
        public string? Remarks { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get;  set; }

        [Column("UpdatedBy_UserName")] 
        public string? UpdatedByUserName { get;  set; }
        [Column("ApplicationStatusId")]
        public int? ApplicationStatusId { get; set; }

        [NotMapped]
        public ApplicationStatusEnum? StatusEnum
        {
            get => ApplicationStatusId.HasValue ? (ApplicationStatusEnum)ApplicationStatusId.Value : null;
            set => ApplicationStatusId = (int?)value;
        }

        // Navigation properties
        public ICollection<ApplicationDetail>? ApplicationDetails { get; set; }

        [ForeignKey(nameof(StateId))]
        [InverseProperty(nameof(State.Applications))]
        public State? State { get; set; }
        public District? District { get; set; }
        public SubDistrict? SubDistrict { get; set; }
        public ForestProduce? ForestProduce { get; set; }
        
    }
}
