using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Species_Logs_MinorForestProduce")]
    public class SpeciesLogsMinorForestProduce
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("MinorForestProduce_Id")]
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

        [Required(ErrorMessage = "Plant Part ID is required.")]
        [Column("PlantPartID")]
        [Range(1, int.MaxValue, ErrorMessage = "Plant Part ID must be greater than zero.")]
        public int PlantPartID { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Column("Quantity", TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Column("Unit")]
        [StringLength(1, ErrorMessage = "Unit must be a single character.")]
        public string Unit { get; set; }

        [Required]
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column("Application_Id")]
        public long? ApplicationId { get; set; }
        public MasterSpecies? Species { get; set; }
    }
}