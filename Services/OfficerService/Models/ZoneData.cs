using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    public class ZoneData
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int StateID { get; set; }

        [Required]
        public int DistrictID { get; set; }

        public int? SubDistrictID { get; set; }

        [Required]
        public int ZoneID { get; set; }

        [ForeignKey("StateID")]
        public virtual State State { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        [ForeignKey("SubDistrictID")]
        public virtual SubDistrict SubDistrict { get; set; }

        [ForeignKey("ZoneID")]
        public virtual MasterZone MasterZone { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
    }
}

