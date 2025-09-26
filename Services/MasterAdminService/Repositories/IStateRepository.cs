using MasterAdminService.Models;

namespace MasterAdminService.Repositories
{
    public interface IStateRepository
    {
        Task<IEnumerable<State>> GetAllStatesAsync();
    }
}
