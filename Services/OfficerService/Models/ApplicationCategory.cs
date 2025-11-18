using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("ApplicationCategory")]
    public class ApplicationCategory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("Category_Name")]
        [MaxLength(100)]
        public string? CategoryName { get; set; }

        // 🔗 Navigation property
        public ICollection<ApplicationDetail>? ApplicationDetails { get; set; }
    }
}
