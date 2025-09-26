using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterAdminService.Models
{
    [Table("Range")]  // ✅ Matches your DB table
    public class Range
    {
        [Key]
        [Column("Range_Id")]
        public int RangeId { get; set; }

        [Column("Division_Id")]
        public int? DivisionId { get; set; }

        [Column("Sub_Div_Id")]
        public int? SubDivId { get; set; }

        [Column("Range_Name")]
        [StringLength(255)]
        public string? RangeName { get; set; }

        [Column("Uploaded_Hammer")]
        [StringLength(200)]
        public string? UploadedHammer { get; set; }

        [Column("TP_Validity")]
        public int? TPValidity { get; set; }

        // 🔗 Navigation Properties
        [ForeignKey("DivisionId")]
        public virtual Division? Division { get; set; }

        [ForeignKey("SubDivId")]
        public virtual SubDivision? SubDivision { get; set; }
    }
}
