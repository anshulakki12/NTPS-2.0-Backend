namespace OfficerService.DtoModels
{
    public class MasterRolesLogDto
    {
        public int LogId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string OperationBy { get; set; } = string.Empty;
        public string? OldRoleName { get; set; }
        public string? NewRoleName { get; set; }
        public bool? OldIsActive { get; set; }
        public bool? NewIsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public DateTime? ReactivatedOn { get; set; }
        public string? Remarks { get; set; }
        public string? IpAddress { get; set; }
    }
}
