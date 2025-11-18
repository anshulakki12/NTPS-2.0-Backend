using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IMasterMenuRepository
    {
        Task<MasterMenu> CreateMenuAsync(CreateMenuDto createDto);
        Task<MasterMenu?> GetMenuByIdAsync(int menuId);
        Task<MasterMenu?> GetMenuByCodeAsync(string menuCode);
        Task<IEnumerable<MasterMenu>> GetAllMenusAsync();
        Task<IEnumerable<MasterMenu>> GetActiveMenusAsync();
        Task<IEnumerable<MasterMenu>> GetParentMenusAsync();
        Task<IEnumerable<MasterMenu>> GetChildMenusAsync(int parentMenuId);
        Task<MasterMenu> UpdateMenuAsync(MasterMenu menu);
        Task<bool> DeleteMenuAsync(int menuId);
        Task<bool> HasChildMenusAsync(int menuId);
    }
}
