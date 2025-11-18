using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicantAuthenticationService.Models
{
    [Table("Applicant_Resource_Collection")]
    public class ApplicantResourceCollection
    {
        [Key]
        [Column("Resource_id")]
        public long Id { get; set; }
        [Column("Service_Name")]
        public string ServiceName { get; set; } = null!;
        [Column("Resource_Key")]
        public string ResourceKey { get; set; } = null!;
        [Column("Resource_Value")]
        public string? ResourceValue { get; set; }
        public string Culture { get; set; } = "en-US";
        [Column("Page_Or_Module")]
        public string? PageOrModule { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
