using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Transport_Mode")]
    public class MasterTransportMode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Transport_Id")]
        public int TransportId { get; set; }

        [Required(ErrorMessage = "Transport Mode is required")]
        [Column("Transport_Mode")]
        [StringLength(100, ErrorMessage = "Transport Mode cannot exceed 100 characters.")]
        public string TransportMode { get; set; } = null!;

        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_By")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Modified_By")]
        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Column("Modified_Date")]
        public DateTime? ModifiedDate { get; set; }
    }
}
