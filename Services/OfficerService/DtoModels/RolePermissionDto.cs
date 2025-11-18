namespace OfficerService.DtoModels
{
    public class RolePermissionDto
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public string? AssignedBy { get; set; }
    }

    public class CreateRolePermissionDto
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
        public string? AssignedBy { get; set; }
    }

    public class UpdateRolePermissionDto
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
        public string? UpdatedBy { get; set; }
    }

    public class UserPermissionDto
    {
        public string LoginId { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
