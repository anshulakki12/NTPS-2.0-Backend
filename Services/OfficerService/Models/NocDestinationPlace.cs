using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Noc_Destination_Place")]
    public class NocDestinationPlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Destination_Id")]
        public long DestinationId { get; set; }

        [Required(ErrorMessage = "TP Registration No is required.")]
        [Column("Application_Id")]
        [StringLength(100, ErrorMessage = "TP Registration No cannot exceed 100 characters.")]
        public string ApplicationId { get; set; }

        [Required(ErrorMessage = "State ID is required.")]
        [Column("State_Id")]
        [Range(1, int.MaxValue, ErrorMessage = "State ID must be greater than zero.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Circle ID is required.")]
        [Column("Circle_Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Circle ID must be greater than zero.")]
        public int CircleId { get; set; }

        [Required(ErrorMessage = "Division ID is required.")]
        [Column("Division_Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Division ID must be greater than zero.")]
        public int DivisionId { get; set; }

        [Required(ErrorMessage = "Range ID is required.")]
        [Column("Range_Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Range ID must be greater than zero.")]
        public int RangeId { get; set; }

        [Column("Address")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        [Column("Pincode")]
        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters.")]
        public string? PinCode { get; set; }

        [Required]
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
