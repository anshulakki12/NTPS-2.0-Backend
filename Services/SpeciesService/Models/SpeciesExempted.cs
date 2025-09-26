using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeciesService.Models
{
    [Table("Species_Exempted")]
    public class SpeciesExempted
    {
        [Key]
        [Column("Exempted_Id")]
        public int ExemptedId { get; set; }

        [Column("State_Id")]
        public int? StateId { get; set; }

        [Column("DIST_CODE")]
        public int? DistCode { get; set; }

        [Column("Species_Id")]
        public int? SpeciesId { get; set; }

        [Column("ProduceSpeciesID")]
        public int? ProduceSpeciesID { get; set; }

        [Column("ExemptORNotExempt")]
        public int? ExemptORNotExempt { get; set; }

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; }

        // ✅ Navigation property & foreign key attribute
        [ForeignKey(nameof(SpeciesId))]
        public MasterSpecies? MasterSpecies { get; set; }
        // ✅ NEW: navigation to ForestProduce using ProduceSpeciesID as FK
        [ForeignKey(nameof(ProduceSpeciesID))]
        public ForestProduce? ForestProduce { get; set; }
    }
}
