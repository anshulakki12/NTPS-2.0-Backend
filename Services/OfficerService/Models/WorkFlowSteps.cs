using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("WorkFlowSteps")]
    public class WorkFlowSteps
    {
        [Key]
        [Column("Step_ID")]
        public int StepId { get; set; }

        [Column("WorkFlow_ID")]
        [ForeignKey("MasterWorkFlow")]
        public int WorkFlowId { get; set; }

        [Column("Level_ID")]
        [ForeignKey("MasterLevel")]
        public int LevelId { get; set; }

        [Column("Step_Order")]
        public int StepOrder { get; set; }

        [Column("Role_ID")]
        public int RoleId { get; set; }

        [Column("Designation_ID")]
        public int DesignationId { get; set; }

        [Column("IsFinal_Step")]
        public bool IsFinalStep { get; set; }

        [Column("Created_On")]
        public DateTime CreatedOn { get; set; }

        // 🔗 Optional: Navigation properties for relationships
        public MasterWorkFlow? MasterWorkFlow { get; set; }
        public MasterLevel? MasterLevel { get; set; }
        public MasterRoles? Role { get; set; } = null;
        public MasterDesignation? Designation { get; set; } = null;
    }
}
