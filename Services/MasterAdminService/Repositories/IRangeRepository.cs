using DbRange = MasterAdminService.Models.Range;

namespace MasterAdminService.Repositories
{
    public interface IRangeRepository
    {
        Task<IEnumerable<DbRange>> GetRangesByDivisionIdAsync(int divisionId);
    }
}
