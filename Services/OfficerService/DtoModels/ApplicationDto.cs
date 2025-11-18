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
        }

        public class ProduceDetailResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? RegistrationNo { get; set; }
            public long ApplicationDetailId { get; set; }
        }

        public class ProduceDetailDto
        {
            public int SpeciesId { get; set; }

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
            public int ForestProduceId { get; set; }
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
        }
    }
}