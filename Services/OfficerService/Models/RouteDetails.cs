using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Route_Details")]
    public class RouteDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Registration_No")]
        [Required(ErrorMessage = "Registration No is required.")]
        [StringLength(50, ErrorMessage = "Registration No cannot exceed 50 characters.")]
        public string? RegistrationNo { get; set; }

        [Column("State_Id")]
        [Required(ErrorMessage = "State Id is required.")]
        public int? StateId { get; set; }

        [Column("District_Id")]
        [Required(ErrorMessage = "District Id is required.")]
        public int? DistrictId { get; set; }

        [Column("Created_Date")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
