using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterAdminService.Models
{
    [Table("Division")]  // ✅ Matches your DB table name
    public class Division
    {
        [Key]
        [Column("Division_Id")]
        public int DivisionId { get; set; }

        [Column("Circle_Id")]
        public int? CircleId { get; set; }

        [Column("Division_Name")]
        [StringLength(255)]
        public string? DivisionName { get; set; }

        [Column("DIST_CODE")]
        public int? DistCode { get; set; }

        [Column("TP_Validity")]
        public int? TPValidity { get; set; }
        [ForeignKey("CircleId")]
        public virtual Circle? Circle { get; set; }
        public virtual ICollection<SubDivision> SubDivisions { get; set; } = new List<SubDivision>();
        public virtual ICollection<Range> Ranges { get; set; } = new List<Range>();


    }
}
