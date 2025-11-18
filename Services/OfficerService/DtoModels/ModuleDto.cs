namespace OfficerService.DtoModels
{
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public DateTime? ReactivatedOn { get; set; }
    }

    public class CreateModuleDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public int DisplayOrder { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
