using MasterAdminService.Data;
using MasterAdminService.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterAdminService.Repositories
{
    public class CircleRepository : ICircleRepository
    {
        private readonly AppDbContext _context;

        public CircleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Circle>> GetCirclesByStateAsync(int stateId)
        {
            return await _context.Circles
                                 .Where(c => c.StateId == stateId)
                                 .ToListAsync();
        }
    }
}
