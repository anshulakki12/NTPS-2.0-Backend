using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class TransportModeRepository : ITransportModeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TransportModeRepository> _logger;

        public TransportModeRepository(AppDbContext context, ILogger<TransportModeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MasterTransportMode>> GetAllAsync()
        {
            return await _context.MasterTransportModes
                .OrderBy(t => t.TransportMode)
                .ToListAsync();
        }

        public async Task<MasterTransportMode?> GetByIdAsync(int id)
        {
            return await _context.MasterTransportModes.FindAsync(id);
        }

        public async Task<MasterTransportMode> CreateAsync(MasterTransportMode transportMode)
        {
            transportMode.CreatedDate = DateTime.UtcNow;
            _context.MasterTransportModes.Add(transportMode);
            await _context.SaveChangesAsync();
            return transportMode;
        }

        public async Task<MasterTransportMode> UpdateAsync(MasterTransportMode transportMode)
        {
            transportMode.ModifiedDate = DateTime.UtcNow;
            _context.MasterTransportModes.Update(transportMode);
            await _context.SaveChangesAsync();
            return transportMode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transportMode = await _context.MasterTransportModes.FindAsync(id);
            if (transportMode == null)
                return false;

            _context.MasterTransportModes.Remove(transportMode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string transportMode, int? excludeId = null)
        {
            var query = _context.MasterTransportModes
                .Where(t => t.TransportMode.ToLower() == transportMode.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.TransportId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ToggleStatusAsync(int id, bool isActive, string modifiedBy)
        {
            var transportMode = await _context.MasterTransportModes.FindAsync(id);
            if (transportMode == null)
                return false;

            transportMode.IsActive = isActive;
            transportMode.ModifiedBy = modifiedBy;
            transportMode.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

