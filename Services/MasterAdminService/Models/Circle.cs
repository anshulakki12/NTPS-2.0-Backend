using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterAdminService.Models
{
    [Table("Circle")]  // ✅ Exact table name from DB
    public class Circle
    {
        [Key]
        [Column("Circle_Id")]
        public int CircleId { get; set; }

        [Column("State_Id")]
        public int? StateId { get; set; }

        [Column("Circle_Name")]
        [StringLength(255)]
        public string? CircleName { get; set; }

        [Column("TP_Validity")]
        public int? TPValidity { get; set; }
        [ForeignKey("StateId")]
        public virtual State? State { get; set; }

        public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();

    }
}
