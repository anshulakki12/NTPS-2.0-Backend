using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Sub_District")]
    public class SubDistrict
    {
        [Key]
        [Column("Sub_Dist_Code")]
        public int SubDistCode { get; set; }

        [Column("Sub_Dist_Name")]
        [Required]
        [MaxLength(150)] // adjust based on actual database column length
        public string SubDistName { get; set; }

        [Column("Dist_Code")]
        [ForeignKey("District")]
        public int DistCode { get; set; }

        // Optional: Navigation property (if you have District table mapped)
        public District? District { get; set; }
    }
}
