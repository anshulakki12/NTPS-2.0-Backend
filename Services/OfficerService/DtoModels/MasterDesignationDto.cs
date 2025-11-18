using System.ComponentModel.DataAnnotations;

namespace OfficerService.DtoModels
{
    public class CreateDesignationDto
    {
        [Required(ErrorMessage = "Designation name is required")]
        [MaxLength(100, ErrorMessage = "Designation name cannot exceed 100 characters")]
        public string DesignationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rank is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Rank must be greater than 0")]
        public int Rank { get; set; }

        [Required(ErrorMessage = "Abbreviation is required")]
        [MaxLength(20, ErrorMessage = "Abbreviation cannot exceed 20 characters")]
        public string Abbreviation { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
    }

    public class UpdateDesignationDto
    {
        [Required(ErrorMessage = "Designation name is required")]
        [MaxLength(100, ErrorMessage = "Designation name cannot exceed 100 characters")]
        public string DesignationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rank is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Rank must be greater than 0")]
        public int Rank { get; set; }

        [Required(ErrorMessage = "Abbreviation is required")]
        [MaxLength(20, ErrorMessage = "Abbreviation cannot exceed 20 characters")]
        public string Abbreviation { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }
    }

    public class DesignationResponseDto
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int Rank { get; set; }
        public string Abbreviation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public DateTime? ReactivatedOn { get; set; }
    }
}
