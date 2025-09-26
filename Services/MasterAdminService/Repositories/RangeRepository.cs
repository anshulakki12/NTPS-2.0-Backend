using MasterAdminService.Data;
using Microsoft.EntityFrameworkCore;
using DbRange = MasterAdminService.Models.Range;


namespace MasterAdminService.Repositories
{
    public class RangeRepository : IRangeRepository
    {
        private readonly AppDbContext _context;

        public RangeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DbRange>> GetRangesByDivisionIdAsync(int divisionId)
        {
            return await _context.Ranges
                                 .Where(r => r.DivisionId == divisionId)
                                 .ToListAsync();
        }

    }
}
