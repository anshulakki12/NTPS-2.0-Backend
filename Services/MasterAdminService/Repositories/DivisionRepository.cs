using MasterAdminService.Data;
using MasterAdminService.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
