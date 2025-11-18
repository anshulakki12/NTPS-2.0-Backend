using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Species")]
    public class MasterSpecies
    {
        [Key]
        [Column("SpeciesID")]
        public int SpeciesID { get; set; }

        [Column("Name")]
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
