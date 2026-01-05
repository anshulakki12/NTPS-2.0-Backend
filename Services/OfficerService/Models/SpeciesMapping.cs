using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Species_Mapping")]
    public class SpeciesMapping
    {
        [Key]
        [Column("Species_Mapping_ID")]
        public int SpeciesMappingId { get; set; }

        [Column("Forest_Produce_Id")]
        [ForeignKey("ForestProduce")]
        public int ForestProduceId { get; set; }

        [Column("Species_ID")]
        [ForeignKey("MasterSpecies")]
        public int SpeciesId { get; set; }

        [Column("State_ID")]
        [ForeignKey("State")]
        public int StateId { get; set; }

        [Column("WorkFlow_ID")]
        [ForeignKey("MasterWorkFlow")]
        public int WorkFlowId { get; set; }

        [Column("Zone_ID")]
        [ForeignKey("MasterZone")]
        public int? ZoneId { get; set; }

        [Column("CategoryID")]
        public int CategoryId { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        // 🔗 Optional navigation properties for relationships
        [ForeignKey(nameof(StateId))]
        public State State { get; set; } = null!;
        public MasterWorkFlow? MasterWorkFlow { get; set; }
        public ForestProduce? ForestProduce { get; set; }
        public MasterSpecies? MasterSpecies { get; set; }
        public MasterZone? MasterZone { get; set; }
    }
}
