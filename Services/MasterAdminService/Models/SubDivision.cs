using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterAdminService.Models
{
    [Table("Sub_Division")]
    public class SubDivision
    {
        [Key]
        [Column("Sub_Div_Id")]
        public int SubDivId { get; set; }

        [Column("Division_Id")]
        public int? DivisionId { get; set; }

        [Column("Sub_Div_Name")]
        [StringLength(50)]
        public string? SubDivName { get; set; }

        [Column("Created_date")]
        public DateTime CreatedDate { get; set; }

        // 🔗 Relationships
        [ForeignKey("DivisionId")]
        public virtual Division? Division { get; set; }

        public virtual ICollection<Range> Ranges { get; set; } = new List<Range>();
    }
    }
