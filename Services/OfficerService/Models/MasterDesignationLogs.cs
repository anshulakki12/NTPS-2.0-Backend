using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Designation_Logs")]
    public class MasterDesignationLogs
    {
        [Key]
        [Column("Log_Id")]
        public int LogId { get; set; }

        [Column("Designation_Id")]
        [Required]
        public int DesignationId { get; set; }

        [Column("Designation_Name")]
        [Required]
        [MaxLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        [Column("Rank")]
        public int? Rank { get; set; }

        [Column("Abbreviation")]
        [MaxLength(20)]
        public string? Abbreviation { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; }

        [Column("Operation_Type")]
        [Required]
        [MaxLength(20)]
        public string OperationType { get; set; } = string.Empty; // UPDATE, DELETE, DEACTIVATE, REACTIVATE

        [Column("Operation_Date")]
        [Required]
        public DateTime OperationDate { get; set; } = DateTime.UtcNow;

        [Column("Operation_By")]
        [Required]
        [MaxLength(100)]
        public string OperationBy { get; set; } = string.Empty;

        [Column("Old_Designation_Name")]
        [MaxLength(100)]
        public string? OldDesignationName { get; set; }

        [Column("New_Designation_Name")]
        [MaxLength(100)]
        public string? NewDesignationName { get; set; }

        [Column("Old_Rank")]
        public int? OldRank { get; set; }

        [Column("New_Rank")]
        public int? NewRank { get; set; }

        [Column("Old_Abbreviation")]
        [MaxLength(20)]
        public string? OldAbbreviation { get; set; }

        [Column("New_Abbreviation")]
        [MaxLength(20)]
        public string? NewAbbreviation { get; set; }

        [Column("Old_Is_Active")]
        public bool? OldIsActive { get; set; }

        [Column("New_Is_Active")]
        public bool? NewIsActive { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Created_By")]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Updated_By")]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        [Column("Deactivated_On")]
        public DateTime? DeactivatedOn { get; set; }

        [Column("Reactivated_On")]
        public DateTime? ReactivatedOn { get; set; }

        [Column("Remarks")]
        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Column("IP_Address")]
        [MaxLength(50)]
        public string? IpAddress { get; set; }
    }
}
