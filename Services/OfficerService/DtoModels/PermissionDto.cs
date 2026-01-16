namespace OfficerService.DtoModels
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CreatePermissionDto
    {
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }

    public class UpdatePermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
    }

    // Request models
    public class DocumentRequest
    {
        public int StateCode { get; set; }
        public string StateName { get; set; }
        public string OfficerId { get; set; } // Add this
        public List<DocumentDto> Documents { get; set; }
    }

    public class DocumentDto
    {
        public string DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class DocumentUpdateRequest
    {
        public bool IsActive { get; set; }
        public bool IsMandatory { get; set; }
        public string OfficerId { get; set; } // Add this
    }
}
