using MasterAdminService.Data;
using MasterAdminService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MasterAdminService.Repositories
{
    public class DivisionRepository : IDivisionRepository
    {
        private readonly AppDbContext _context;

        public DivisionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Division>> GetDivisionsByCircleIdAsync(int circleId)
        {
            return await _context.Divisions
                                 .Where(d => d.CircleId == circleId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Division>> GetDivisionsByCircleIdsAsync(List<int> circleIds)
        {
            return await _context.Divisions
                                 .Where(d => circleIds.Contains((int)d.CircleId))
                                 .ToListAsync();
        }
    }
}
