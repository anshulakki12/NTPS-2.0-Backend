using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Level")]
    public class MasterLevel
    {
        [Key]
        [Column("Level_ID")]
        public int LevelId { get; set; }

        [Column("Level_Name")]
        [Required]
        [MaxLength(100)] // Adjust based on actual DB column length
        public string LevelName { get; set; }

        [Column("Is_Active")]
        public bool IsActive { get; set; }
    }
}
