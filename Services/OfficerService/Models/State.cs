using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("State", Schema = "dbo")]
    public class State
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ST_CODE")]
        public int StCode { get; set; }

        [Column("ST_Name")]
        [Required]
        [MaxLength(100)] // Adjust if needed based on your DB schema
        public string StName { get; set; }

        [Column("ST_UT")]
        [MaxLength(50)] // Adjust as per column size (e.g., "State" or "UT")
        public string? StUt { get; set; }
        public ICollection<MasterWorkFlow>? WorkFlows { get; set; }
    }
}
