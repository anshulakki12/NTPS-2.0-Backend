using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Source_Lat_Long")]
    public class SourceLatLong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("SourceLatLong_Id")]
        public int LatLongId { get; set; }

        [Column("Registration_No")]
        [StringLength(100, ErrorMessage = "TP Registration cannot exceed 100 characters.")]
        public string? RegistrationNo { get; set; }

        [Column("Latitude")]
        [StringLength(25, ErrorMessage = "Latitude cannot exceed 25 characters.")]
        public string? Latitude { get; set; }

        [Column("Longitude")]
        [StringLength(25, ErrorMessage = "Longitude cannot exceed 25 characters.")]
        public string? Longitude { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
