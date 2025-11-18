namespace OfficerService.DtoModels
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleCode { get; set; }
        public int? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }
        public bool IsParent { get; set; }
        public bool HasChildren { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMenuDto
    {
        public string MenuName { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentMenuId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }
        public bool IsParent { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateMenuDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuCode { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentMenuId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }
        public bool IsParent { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class RoleMenuDto
    {
        public int RoleMenuId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? MenuLabel { get; set; }
        public string? MenuIcon { get; set; }
        public string? RouterLink { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateRoleMenuDto
    {
        public int RoleId { get; set; }
        public List<int> MenuIds { get; set; } = new List<int>();
        public Dictionary<int, int>? DisplayOrders { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateRoleMenuDto
    {
        public int RoleMenuId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class MenuOrderDto
    {
        public int MenuId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ReorderMenusDto
    {
        public List<MenuOrderDto> MenuOrders { get; set; } = new List<MenuOrderDto>();
    }

    public class UserMenuItemDto
    {
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RouterLink { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }
        public List<UserMenuItemDto>? Items { get; set; }
        public int DisplayOrder { get; set; }
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public bool IsVisible { get; set; }
    }
}
