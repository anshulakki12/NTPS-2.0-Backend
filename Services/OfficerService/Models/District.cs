using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("DISTRICT")]
    public class District
    {
        [Key]
        [Column("DIST_CODE")]
        public int DistCode { get; set; }

        [Column("DIST_NAME")]
        [Required]
        [MaxLength(100)] // Adjust length if known
        public string DistName { get; set; }

        [Column("ST_CODE")]
        public int StCode { get; set; }
        // Optional: Add navigation property for one-to-many relationship
        public ICollection<SubDistrict>? SubDistricts { get; set; }
    }
}
