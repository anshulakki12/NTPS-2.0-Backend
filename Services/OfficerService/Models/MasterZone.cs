using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Master_Zone")]
    public class MasterZone
    {
        [Key]
        [Column("Zone_Id")]
        public int ZoneID { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Zone_Name")]
        public string ZoneName { get; set; }

        [Required]
        [Column("State_ID")]
        public int StateID { get; set; }
        [ForeignKey("StateID")]
        public virtual State State { get; set; }
        [Column("Is_Active")]
        public bool IsActive { get; set; } = true;
        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column("Created_By")]
        public string CreatedBy { get; set; }
    }
}
