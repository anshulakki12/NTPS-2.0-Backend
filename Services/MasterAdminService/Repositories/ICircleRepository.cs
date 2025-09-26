using MasterAdminService.Models;

namespace MasterAdminService.Repositories
{
    public interface ICircleRepository
    {
        Task<IEnumerable<Circle>> GetCirclesByStateAsync(int stateId);
    }
}
