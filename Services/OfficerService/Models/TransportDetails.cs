using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficerService.Models
{
    [Table("Transport_Details")]
    public class TransportDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TP_id")]
        public int TPId { get; set; }

        [Column("Registration_No")]
        [Required(ErrorMessage = "Registration No is required.")]
        [StringLength(50, ErrorMessage = "Registration No cannot exceed 50 characters.")]
        public string? RegistrationNo { get; set; }

        [Column("Transport_Id")]
        [Required(ErrorMessage = "Transport Id is required.")]
        public int? TransportId { get; set; }

        [Column("Driver_Name")]
        [Required(ErrorMessage = "Driver Name is required.")]
        [StringLength(100, ErrorMessage = "Driver Name cannot exceed 100 characters.")]
        public string? DriverName { get; set; }

        [Column("Driver_licence_No")]
        [Required(ErrorMessage = "Driver licence No is required.")]
        [StringLength(50, ErrorMessage = "Driver Licence Number cannot exceed 50 characters.")]
        public string? DriverLicenceNo { get; set; }

        [Column("Vehicle_Owner_Name")]
        [Required(ErrorMessage = "Vehicle Owner Name is required.")]
        [StringLength(100, ErrorMessage = "Vehicle Owner Name cannot exceed 100 characters.")]
        public string? VehicleOwnerName { get; set; }

        [Column("Vehicle_No")]
        [Required(ErrorMessage = "Vehicle No is required.")]
        [StringLength(100, ErrorMessage = "Vehicle Number cannot exceed 100 characters.")]
        public string? VehicleNo { get; set; }

        [Column("Vehicle_Photograph")]
        [Required(ErrorMessage = "Vehicle Photograph is required.")]
        [StringLength(200, ErrorMessage = "Vehicle Photograph path cannot exceed 200 characters.")]
        public string? VehiclePhotograph { get; set; }

        [Column("Created_Date")]
        [Required(ErrorMessage = "Created Date is required.")]
        public DateTime? CreatedDate { get; set; }

        [Column("Updated_Date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("source_type")]
        [StringLength(10, ErrorMessage = "Source Type cannot exceed 10 characters.")]
        public string? SourceType { get; set; }

        [Column("Vehicle_LoadingCertificate")]
        [StringLength(200, ErrorMessage = "Loading Certificate path cannot exceed 200 characters.")]
        public string? VehicleLoadingCertificate { get; set; }
    }
}
