using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Government_Depot")]
    public class GovernmentDepot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Government_Depot_Id")]
        public int GdId { get; set; }

        [Column("Registration_No")]
        [StringLength(100, ErrorMessage = "TP Registration cannot exceed 100 characters.")]
        public string? RegistrationNo { get; set; }

        [Column("Depot_Name")]
        [StringLength(200, ErrorMessage = "Depot Name cannot exceed 200 characters.")]
        public string? DepotName { get; set; }

        [Column("Log_Number")]
        public string? LogNumber { get; set; }   // nvarchar(max)

        [Column("Date_of_Auction")]
        public DateTime? DateOfAuction { get; set; }

        [Column("Amount_Paid", TypeName = "decimal(18,2)")]
        public decimal? AmountPaid { get; set; }

        [Column("Bill_Upload")]
        [StringLength(200, ErrorMessage = "Bill Upload path cannot exceed 200 characters.")]
        public string? BillUpload { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("Gov_Depot_Id")]
        public int? GovDepotId { get; set; }

        [Column("source_Type")]
        [StringLength(10, ErrorMessage = "Source Type cannot exceed 10 characters.")]
        public string? SourceType { get; set; } //web or mobile

        [Column("Type")]
        [StringLength(50, ErrorMessage = "Type must be a single character.")]
        public string? Type { get; set; } // source or destination

        [Column("Place")]
        [StringLength(50, ErrorMessage = "Type must be a single character.")]
        public string? PlaceType { get; set; } // private or government
    }
}
