using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Gov_Depot")]
    public class MasterGovDepot
    {
        [Key]
        [Column("Gov_Depot_Id")]
        public int GovDepotId { get; set; }

        [Column("State_Id")]
        [ForeignKey("State")]
        public int StateId { get; set; }

        [Column("Circle_Id")]
        public int CircleId { get; set; }

        [Column("Division_Id")]
        public int DivisionId { get; set; }

        [Column("Range_Id")]
        public int RangeId { get; set; }

        [Column("Depot_Name")]
        [Required]
        [MaxLength(200)] // Adjust as per your DB schema
        public string DepotName { get; set; }

        [Column("Address")]
        [MaxLength(500)] // Adjust if DB allows longer text
        public string? Address { get; set; }

        [Column("PinCode")]
        [MaxLength(10)]
        public string? PinCode { get; set; }

        [Column("Type")]
        [MaxLength(100)]
        public string? Type { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        // 🔗 Optional Navigation Property if needed for relationships add circle district, state, etc.
        [ForeignKey(nameof(StateId))]
        public State State { get; set; } = null!;
    }
}
