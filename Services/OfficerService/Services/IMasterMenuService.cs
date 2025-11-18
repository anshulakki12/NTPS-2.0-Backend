using OfficerService.DtoModels;

namespace OfficerService.Services
{
    public interface IMasterMenuService
    {
        Task<MenuDto> CreateMenuAsync(CreateMenuDto createDto);
        Task<MenuDto?> GetMenuByIdAsync(int menuId);
        Task<IEnumerable<MenuDto>> GetAllMenusAsync();
        Task<IEnumerable<MenuDto>> GetActiveMenusAsync();
        Task<IEnumerable<MenuDto>> GetParentMenusAsync();
        Task<IEnumerable<MenuDto>> GetChildMenusAsync(int parentMenuId);
        Task<MenuDto> UpdateMenuAsync(int menuId, UpdateMenuDto updateDto);
        Task<MenuDto> ActivateMenuAsync(int menuId, string activatedBy);
        Task<MenuDto> DeactivateMenuAsync(int menuId, string deactivatedBy);
        Task<bool> DeleteMenuAsync(int menuId);
    }
}
