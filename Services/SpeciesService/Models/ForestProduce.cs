using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeciesService.Models
{
    [Table("Forest_Produce")]
    public class ForestProduce
    {
        [Key]
        [Column("Forest_Produce_Id")]
        public int ForestProduceId { get; set; }

        [Column("Name")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Column("Quantity_Type")]
        [StringLength(1)]
        public string? QuantityType { get; set; } // char(1), nullable

        [Column("Weight_Type")]
        [StringLength(1)]
        public string? WeightType { get; set; } // char(1), nullable

        [Column("Category")]
        public int? Category { get; set; }   // int, nullable

        [Column("Requires_Log")]
        [StringLength(1)]
        public string RequiresLog { get; set; } = string.Empty; // char(1), NOT NULL

        // ✅ Navigation property: one ForestProduce can be referenced by many SpeciesExempted rows
        public ICollection<SpeciesExempted> SpeciesExemptedList { get; set; }
            = new List<SpeciesExempted>();

    }
}
