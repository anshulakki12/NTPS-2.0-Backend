using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_WorkFlow")]
    public class MasterWorkFlow
    {
        [Key]
        [Column("WorkFlow_ID")]
        public int WorkFlowId { get; set; }

        [Column("WorkFlow_Name")]
        [Required]
        [MaxLength(150)] // Adjust based on actual DB column definition
        public string WorkFlowName { get; set; }

        [Column("StateID")]
        [ForeignKey("State")]
        public int StateId { get; set; }

        [Column("Is_Default")]
        public bool IsDefault { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        // 🔗 Optional: Navigation property to State table
        public State? State { get; set; }
        public ICollection<WorkFlowSteps>? WorkFlowSteps { get; set; }
    }
}
