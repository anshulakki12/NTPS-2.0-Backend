using System.ComponentModel.DataAnnotations;

namespace OfficerService.DTOs
{
    public class SpeciesMappingDto
    {
        [Required]
        public int ForestProduceId { get; set; }

        [Required]
        public int SpeciesId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int WorkFlowId { get; set; }

        [Required]
        public int? ZoneId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateSpeciesMappingDto
    {
        [Required]
        public int SpeciesMappingId { get; set; }

        [Required]
        public int ForestProduceId { get; set; }

        [Required]
        public int SpeciesId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int WorkFlowId { get; set; }

        [Required]
        public int? ZoneId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }

    public class SpeciesMappingResponseDto
    {
        public int SpeciesMappingId { get; set; }
        public int ForestProduceId { get; set; }
        public string ForestProduceName { get; set; }
        public int SpeciesId { get; set; }
        public string SpeciesName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; }
        public int? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class SpeciesMappingsDto
    {
        public int ForestProduceId { get; set; }
        public string ForestProduceName { get; set; }
        public int SpeciesId { get; set; }
        public string SpeciesName { get; set; }
    }

    public class ForestProduceDto
    {
        public int ForestProduceId { get; set; }
        public string Name { get; set; }
        public string QuantityType { get; set; }
        public string WeightType { get; set; }
        public int? Category { get; set; }
        public string RequiresLog { get; set; }
    }

    public class SpeciesDto
    {
        public int SpeciesID { get; set; }
        public string Name { get; set; }
    }

    public class CreateGovtDepotDto
    {
        public int StateId { get; set; }
        public int CircleId { get; set; }
        public int DivisionId { get; set; }
        public int RangeId { get; set; }
        public string DepotName { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string Type { get; set; } = "Permanent";
    }

    public class UpdateGovtDepotDto
    {
        public int GovDepotId { get; set; }
        public int StateId { get; set; }
        public int CircleId { get; set; }
        public int DivisionId { get; set; }
        public int RangeId { get; set; }
        public string DepotName { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string Type { get; set; }
    }

    public class GovtDepotResponseDto
    {
        public int GovDepotId { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int CircleId { get; set; }
        public int DivisionId { get; set; }
        public int RangeId { get; set; }
        public string DepotName { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}