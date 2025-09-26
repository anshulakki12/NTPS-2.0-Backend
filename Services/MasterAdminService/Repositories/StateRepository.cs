using MasterAdminService.Data;
using MasterAdminService.Models;
using Microsoft.EntityFrameworkCore;

namespace MasterAdminService.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly AppDbContext _context;

        public StateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            return await _context.States
                                 .OrderBy(s => s.StateName)
                                 .ToListAsync();
        }
    }
}
