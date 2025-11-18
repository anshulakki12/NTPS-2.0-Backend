using System.ComponentModel.DataAnnotations;

namespace OfficerService.DtoModels
{
    public class ZoneMasterDto
    {
        public class CreateMasterZoneDto
        {
            [Required]
            [StringLength(100)]
            public string ZoneName { get; set; }

            [Required]
            public int StateID { get; set; }

            public bool IsActive { get; set; } = true;
        }

        public class UpdateMasterZoneDto
        {
            [Required]
            public int ZoneID { get; set; }

            [Required]
            [StringLength(100)]
            public string ZoneName { get; set; }

            [Required]
            public int StateID { get; set; }

            public bool IsActive { get; set; }
        }

        public class MasterZoneResponseDto
        {
            public int ZoneID { get; set; }
            public string ZoneName { get; set; }
            public int StateID { get; set; }
            public string StateName { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class CreateZoneDataDto
        {
            [Required]
            public int ZoneID { get; set; }

            [Required]
            public int StateID { get; set; }

            [Required]
            public List<int> DistrictIDs { get; set; } = new List<int>();

            public List<int> SubDistrictIDs { get; set; } = new List<int>();

            public bool IsActive { get; set; } = true;
        }

        public class UpdateZoneDataDto
        {
            [Required]
            public int ID { get; set; }

            [Required]
            public int ZoneID { get; set; }

            [Required]
            public int StateID { get; set; }

            [Required]
            public int DistrictID { get; set; }

            public int? SubDistrictID { get; set; }

            public bool IsActive { get; set; }
        }

        public class ZoneDataResponseDto
        {
            public int ID { get; set; }
            public int ZoneID { get; set; }
            public string ZoneName { get; set; }
            public int StateID { get; set; }
            public string StateName { get; set; }
            public int DistrictID { get; set; }
            public string DistrictName { get; set; }
            public int? SubDistrictID { get; set; }
            public string SubDistrictName { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}
