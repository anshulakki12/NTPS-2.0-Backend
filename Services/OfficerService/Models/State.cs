using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("State", Schema = "dbo")]
    public class State
    {
        [Key]
        [Column("State_Id")]
        public int StateId { get; set; }

        [Column("State_Name")]
        [StringLength(255)]
        public string? StateName { get; set; }

        [Column("Region_Id")]
        public int? RegionId { get; set; }

        [Column("State_Code")]
        [StringLength(255)]
        public string? StateCode { get; set; }

        [Column("StatURL")]
        [StringLength(255)]
        public string? StatURL { get; set; }

        [Column("TP_Validity")]
        public int? TPValidity { get; set; }

        [Column("Required_Field")]
        [StringLength(255)]
        public string? RequiredField { get; set; }

        [Column("Language_Id")]
        public int? LanguageId { get; set; }

        [Column("ST_CODE")]
        public int? STCode { get; set; }

        [Column("Species_DOC")]
        [StringLength(200)]
        public string? SpeciesDOC { get; set; }

        [Column("Local_Rule")]
        [StringLength(255)]
        public string? LocalRule { get; set; }

        [Column("Show")]
        public char? Show { get; set; }

        [Column("Lock")]
        [StringLength(5)]
        public string? Lock { get; set; }

        // Navigation collections (optional but recommended)
        public ICollection<ApplicationMaster>? Applications { get; set; }
        public ICollection<ZoneData>? Zones { get; set; }
        public ICollection<MasterWorkFlow>? WorkFlows { get; set; }
        public ICollection<MasterZone>? MasterZones { get; set; }
        public ICollection<SpeciesMapping>? SpeciesMappings { get; set; }
        public ICollection<MasterGovDepot>? GovDepots { get; set; }
    }
}
