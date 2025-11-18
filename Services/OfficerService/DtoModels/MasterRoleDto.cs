using System.ComponentModel.DataAnnotations;

namespace OfficerService.DtoModels
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
    }

    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }
    }

    public class RoleResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public DateTime? ReactivatedOn { get; set; }
    }
}