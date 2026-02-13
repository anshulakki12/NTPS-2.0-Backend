using OfficerService.DtoModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace OfficerService.DtoModels
{
    public class ApplicationDto
    {
        public class CreateApplicationRequestDto
        {
            public int StateId { get; set; }
            public int DistrictId { get; set; }
            public int? SubDistrictId { get; set; }
            public int ForestProduceId { get; set; }
            public string? CreatedByUserId { get; set; }
            public string? CreatedByUserName { get; set; }
        }

        public class ApplicationResponseDto
        {
            public long ApplicationId { get; set; }
            public int StateId { get; set; }
            public string StateName { get; set; } = string.Empty;
            public int DistrictId { get; set; }
            public string DistrictName { get; set; } = string.Empty;
            public int? SubDistrictId { get; set; }
            public string SubDistrictName { get; set; } = string.Empty;
            public int ForestProduceId { get; set; }
            public string ForestProduceName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string ApplicationStatus { get; set; } = string.Empty;
        }

        public class OpenApplicationDto
        {
            public long ApplicationId { get; set; }
            public string StateName { get; set; } = string.Empty;
            public string DistrictName { get; set; } = string.Empty;
            public string ForestProduceName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public string ApplicationStatus { get; set; } = string.Empty;
        }

        public class AddProduceDetailRequestDto
        {
            public long ApplicationId { get; set; }
            public int ForestProduceId { get; set; }
            public string UserId { get; set; }
            public List<ProduceDetailDto> ProduceDetails { get; set; } = new();
        }

        public class ApplicationDetailsDto
        {
            public long ApplicationId { get; set; }
            public bool? Status { get; set; }
            public string? ApplicationStatus { get; set; }
            public string? CreateByUserId { get; set; }
            public string? CreateByUserName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int? StateId { get; set; }
            public string? StateName { get; set; }
            public int? DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public int? SubDistrictId { get; set; }
            public string? SubDistrictName { get; set; }
            public int? ForestProduceId { get; set; }
            public string? ForestProduceName { get; set; }
            public string? Remarks { get; set; }
            // Add this property for species logs
            public List<SpeciesLogResponseDto> SpeciesLogs { get; set; } = new List<SpeciesLogResponseDto>();
        }

        public class ProduceDetailResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string RegistrationNo { get; set; } = string.Empty;
            public long ApplicationDetailId { get; set; }
            public List<SpeciesLogResponse> SavedLogs { get; set; } = new();
        }

        public class ProduceDetailDto
        {
            public long?  ApplicationId { get; set; }
            public long? SpeciesLogId { get; set; } // For updates - null for new entries
            public int SpeciesId { get; set; }
            public int SpeciesMappingId { get; set; } // Add this
            public string? SpeciesMappingIdStr { get; set; } // Add this for string parsing
            public int ForestProduceId { get; set; } // Add this
            public bool IsDeleted { get; set; } // For soft delete

            // Common fields
            public decimal? Quantity { get; set; }
            public string? Unit { get; set; }
            public decimal? Volume { get; set; }

            // String versions for parsing
            public string? QuantityStr { get; set; }
            public string? VolumeStr { get; set; }

            // Bamboo specific
            public decimal? GirthClass { get; set; }
            public string? GirthClassStr { get; set; }
            public decimal? Length { get; set; }
            public string? LengthStr { get; set; }

            // Round Timber specific
            public int? NoOfLogs { get; set; }
            public string? NoOfLogsStr { get; set; }
            public decimal? MiddleGirthCm { get; set; }
            public string? MiddleGirthCmStr { get; set; }
            public decimal? LengthCm { get; set; }
            public string? LengthCmStr { get; set; }

            // Sawn Timber specific
            public int? NoOfPieces { get; set; }
            public string? NoOfPiecesStr { get; set; }
            public decimal? Width { get; set; }
            public string? WidthStr { get; set; }
            public decimal? Thickness { get; set; }
            public string? ThicknessStr { get; set; }

            // Minor Forest Produce specific
            public string? Part { get; set; }
            public int? PlantPartID { get; set; }
            public string? TemporaryId { get; set; } // For frontend mapping
        }

        public class UpdateApplicationRequestDto
        {
            public long ApplicationId { get; set; }
            public int? StateId { get; set; }
            public int? DistrictId { get; set; }
            public int? SubDistrictId { get; set; }
            public int? ForestProduceId { get; set; }
            public string? Remarks { get; set; }
            public string? UpdatedByUserId { get; set; }
            public string? UpdatedByUserName { get; set; }
        }

        public class UpdateProduceDetailRequestDto
        {
            public long ApplicationId { get; set; }
            public int SpeciesId { get; set; }
            public List<UpdateProduceDetailDto> ProduceDetails { get; set; } = new();
        }

        public class UpdateProduceDetailDto : ProduceDetailDto
        {
            public long? SpeciesLogId { get; set; } // ID for existing records
            public bool IsDeleted { get; set; } = false; // Flag for deletion
        }

        public class SpeciesLogResponseDto
        {
            public long SpeciesLogId { get; set; }
            public int SpeciesId { get; set; }
            public int ForestProduceId { get; set; } // Add this
            public string SpeciesName { get; set; } = string.Empty;
            public string? Part { get; set; }
            public decimal? Quantity { get; set; }
            public string? Unit { get; set; }
            public decimal? Volume { get; set; }
            public int? NoOfLogs { get; set; }
            public decimal? MiddleGirthCm { get; set; }
            public decimal? LengthCm { get; set; }
            public decimal? GirthClass { get; set; }
            public decimal? Length { get; set; }
            public int? NoOfPieces { get; set; }
            public decimal? Width { get; set; }
            public decimal? Thickness { get; set; }
            public int? PlantPartID { get; set; }
            public string ForestProduceType { get; set; } = string.Empty;
            public string RegistrationNo { get; set; }
            public long? ApplicationId { get; set; }
        }

        // Updated DTOs

      

        public class SpeciesLogResponse
        {
            public long SpeciesLogId { get; set; }
            public int SpeciesId { get; set; }
            public string? TemporaryId { get; set; } // For frontend mapping
        }

        // Update the SaveProduceSourceRequestDto class
        public class SaveProduceSourceRequestDto
        {
            [Required]
            public long ApplicationId { get; set; }

            [Required]
            public string RegistrationNo { get; set; } = string.Empty;

            [Required]
            public int CategoryId { get; set; } // 1 for NOC, 2 for Transit Pass

            // Government Depot Info
            public string? GovernmentDepotName { get; set; }
            public string? GovernmentDepotType { get; set; }

            // Private Land Info
            public string? SurveyNumber { get; set; }

            // Common Source Fields
            [Required]
            public int StateId { get; set; }

            [Required]
            public int CircleId { get; set; }

            [Required]
            public int DivisionId { get; set; }

            [Required]
            public int RangeId { get; set; }

            [Required]
            public string Address { get; set; } = string.Empty;

            public string? PinCode { get; set; }

            public string? Latitude { get; set; }
            public string? Longitude { get; set; }

            // Place obtained type
            [Required]
            public string PlaceObtained { get; set; } = string.Empty;
        }

        // Update SaveDestinationRequestDto similarly
        public class SaveDestinationRequestDto
        {
            [Required]
            public long ApplicationId { get; set; }

            [Required]
            public string RegistrationNo { get; set; } = string.Empty;

            [Required]
            public int CategoryId { get; set; } // 1 for NOC, 2 for Transit Pass

            // Government Depot Info
            public string? GovernmentDepotName { get; set; }
            public string? GovernmentDepotType { get; set; }

            // Common Destination Fields
            [Required]
            public int StateId { get; set; }

            [Required]
            public int CircleId { get; set; }

            [Required]
            public int DivisionId { get; set; }

            [Required]
            public int RangeId { get; set; }

            [Required]
            public string Address { get; set; } = string.Empty;

            public string? PinCode { get; set; }
            public string DestinationPlace { get; set; } = string.Empty;
        }

        public class SourceDestinationResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public long? SourceId { get; set; }
            public long? DestinationId { get; set; }
            public int? GovernmentDepotId { get; set; }
            public List<int> PrivateLandIds { get; set; } = new List<int>();
            public List<int> LatLongIds { get; set; } = new List<int>();
        }

        public class SourceDestinationDetailsDto
        {
            public ProduceSourceDto? ProduceSource { get; set; }
            public DestinationDto? Destination { get; set; }
        }

        public class ProduceSourceDto
        {
            public string? PlaceObtained { get; set; }
            public string? GovernmentDepotName { get; set; }
            public string? GovernmentDepotType { get; set; }
            public string? SurveyNumber { get; set; } // Add this
            public int? StateId { get; set; }
            public int? CircleId { get; set; }
            public int? DivisionId { get; set; }
            public int? RangeId { get; set; }
            public string? Address { get; set; }
            public string? PinCode { get; set; }
            public string? Latitude { get; set; }
            public string? Longitude { get; set; }
        }

        public class DestinationDto
        {
            public string? DestinationPlace { get; set; }
            public string? GovernmentDepotName { get; set; }
            public string? GovernmentDepotType { get; set; }
            public int? StateId { get; set; }
            public int? CircleId { get; set; }
            public int? DivisionId { get; set; }
            public int? RangeId { get; set; }
            public string? Address { get; set; }
            public string? PinCode { get; set; }
        }

        public class RegisteredTpResponseDto
        {
            public long ApplicationId { get; set; }
            public DateTime CreatedDate { get; set; }
            public string ApplicationStatus { get; set; } = "Open";
            public List<ApplicationDetailInfoDto> ApplicationDetails { get; set; } = new();
            public string? StateName { get; set; }
            public string? DistrictName { get; set; }
            public string? ForestProduceName { get; set; }
        }

        public class ApplicationDetailInfoDto
        {
            public long ApplicationDetailId { get; set; }
            public string RegistrationNo { get; set; } = string.Empty;
            public int ApplicationCategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public DateTime? CreatedDate { get; set; }
        }

        public class SaveVehicleDetailsRequestDto
        {
            public string RegistrationNo { get; set; }
            public int TransportId { get; set; }
            public string DriverName { get; set; }
            public string DriverLicenseNo { get; set; }
            public string VehicleNo { get; set; }
            public string VehicleOwnerName { get; set; }
            public IFormFile VehiclePhoto { get; set; }
        }

        public class VehicleDetailsResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public VehicleDetailsDto Data { get; set; }
        }

        public class VehicleDetailsDto
        {
            public int TPId { get; set; }
            public string RegistrationNo { get; set; }
            public int TransportId { get; set; }
            public string DriverName { get; set; }
            public string DriverLicenseNo { get; set; }
            public string VehicleNo { get; set; }
            public string VehicleOwnerName { get; set; }
            public string VehiclePhotograph { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        // Add to ApplicationDto class
        public class SaveRouteDetailsRequestDto
        {
            [Required]
            public string RegistrationNo { get; set; } = string.Empty;

            [Required]
            public int StateId { get; set; }

            [Required]
            public int DistrictId { get; set; }

            public int? ApplicationId { get; set; }
            public int? CategoryId { get; set; }
        }

        public class RouteDetailsResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public RouteDetailsDto? Data { get; set; }
        }

        public class RouteDetailsDto
        {
            public int Id { get; set; }
            public string RegistrationNo { get; set; } = string.Empty;
            public int StateId { get; set; }
            public int DistrictId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
        }

        public class CheckRouteDetailsExistsResponse
        {
            public bool Exists { get; set; }
            public RouteDetailsDto? Details { get; set; }
        }

        public class SubmitApplicationRequestDto
        {
            public long ApplicationId { get; set; }
            public string RegistrationNo { get; set; }
            public int CategoryId { get; set; }
            public string SubmittedByUserId { get; set; }
            public string SubmittedByUserName { get; set; }
            public DateTime SubmissionDate { get; set; }
            public bool ConsentConfirmed { get; set; }
            // Optionally include full form data for audit
        }

        public class SubmitApplicationResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

        // DTO already defined in the user's query – keep it in appropriate namespace
        public class ApplicationOfficerAssignmentDto
        {
            public string RegistrationNo { get; set; }
            public int SpeciesId { get; set; }
            public string SpeciesName { get; set; }
            public int WorkFlowId { get; set; }
            public int StepOrder { get; set; }
            public int LevelId { get; set; }
            public string LevelName { get; set; }
            public long? RequiredLocationId { get; set; }
            public string OfficerLoginId { get; set; }
            public int OfficerId { get; set; }
            public string OfficerMobile { get; set; }
            public string DesignationName { get; set; }
            public string RoleName { get; set; }
        }
        public class UpdateApplicationStatusRequestDto
        {
            public string RegistrationNo { get; set; }
            public ApplicationStatusEnum NewStatus { get; set; }
            public string OfficerLoginId { get; set; }        // current officer (Login_id_from)
            public string? Remarks { get; set; }
            public string? FullStatus { get; set; }           // optional detailed description
        }

        public class UpdateApplicationStatusResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }
    }
}