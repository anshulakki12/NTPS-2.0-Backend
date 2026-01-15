using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Species_Logs_RoundTimber")]
    public class SpeciesLogsRoundTimber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RoundTimber_Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "TP Registration is required.")]
        [Column("Registration_No")]
        [StringLength(100, ErrorMessage = "TP Registration cannot exceed 100 characters.")]
        public string RegistrationNo { get; set; }

        [Required]
        [Column("ForestProduceId")]
        public int ForestProduceId { get; set; }

        [ForeignKey("ForestProduceId")]
        public ForestProduce? ForestProduce { get; set; }

        [Required(ErrorMessage = "Species ID is required.")]
        [Column("Species_ID")]
        [Range(1, int.MaxValue, ErrorMessage = "Species ID must be greater than zero.")]
        public int SpeciesID { get; set; }

        [Required(ErrorMessage = "Logs No is required.")]
        [Column("Logs_No")]
        [Range(1, int.MaxValue, ErrorMessage = "Logs No must be greater than zero.")]
        public int LogsNo { get; set; }

        [Required(ErrorMessage = "Girth is required.")]
        [Column("Girth", TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Girth must be greater than 0.")]
        public decimal Girth { get; set; }

        [Required(ErrorMessage = "Length is required.")]
        [Column("Length", TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Length must be greater than 0.")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Column("Quantity", TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Volume is required.")]
        [Column("Volume", TypeName = "decimal(18,3)")]
        [Range(0.001, 999999.999, ErrorMessage = "Volume must be greater than 0.")]
        public decimal Volume { get; set; }

        [Required]
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Application_Id")]
        public long? ApplicationId { get; set; }
        [Required(ErrorMessage = "Species Mapping ID is required.")]
        [Column("Species_Mapping_ID")]
        [Range(1, int.MaxValue, ErrorMessage = "Species Mapping ID must be greater than zero.")]
        public int SpeciesMappingId { get; set; } // Add this
        public MasterSpecies? Species { get; set; }
    }
}