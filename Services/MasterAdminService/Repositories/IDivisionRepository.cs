using MasterAdminService.Models;

namespace MasterAdminService.Repositories
{
    public interface IDivisionRepository
    {
        Task<IEnumerable<Division>> GetDivisionsByCircleIdAsync(int circleId);
    }
}
