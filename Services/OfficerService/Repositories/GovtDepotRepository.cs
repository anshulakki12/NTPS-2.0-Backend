using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class GovtDepotRepository: IGovtDepotRepository
    {
        private readonly AppDbContext _context;

        public GovtDepotRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GovtDepotResponseDto>> GetAllGovtDepotsAsync()
        {
            return await _context.MasterGovDepots
                .Include(d => d.State)
                .Select(d => new GovtDepotResponseDto
                {
                    GovDepotId = d.GovDepotId,
                    StateId = d.StateId,
                    StateName = d.State.StName,
                    CircleId = d.CircleId,
                    DivisionId = d.DivisionId,
                    RangeId = d.RangeId,
                    DepotName = d.DepotName,
                    Address = d.Address,
                    PinCode = d.PinCode,
                    Type = d.Type,
                    CreatedDate = d.CreatedDate
                })
                .OrderBy(d => d.DepotName)
                .ToListAsync();
        }

        public async Task<GovtDepotResponseDto> GetGovtDepotByIdAsync(int id)
        {
            return await _context.MasterGovDepots
                .Where(d => d.GovDepotId == id)
                .Include(d => d.State)
                .Select(d => new GovtDepotResponseDto
                {
                    GovDepotId = d.GovDepotId,
                    StateId = d.StateId,
                    StateName = d.State.StName,
                    CircleId = d.CircleId,
                    DivisionId = d.DivisionId,
                    RangeId = d.RangeId,
                    DepotName = d.DepotName,
                    Address = d.Address,
                    PinCode = d.PinCode,
                    Type = d.Type,
                    CreatedDate = d.CreatedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<GovtDepotResponseDto> CreateGovtDepotAsync(CreateGovtDepotDto createDto)
        {
            var depot = new MasterGovDepot
            {
                StateId = createDto.StateId,
                CircleId = createDto.CircleId,
                DivisionId = createDto.DivisionId,
                RangeId = createDto.RangeId,
                DepotName = createDto.DepotName,
                Address = createDto.Address,
                PinCode = createDto.PinCode,
                Type = createDto.Type,
                CreatedDate = DateTime.UtcNow
            };

            _context.MasterGovDepots.Add(depot);
            await _context.SaveChangesAsync();

            // Return the created depot with names
            return await GetGovtDepotByIdAsync(depot.GovDepotId);
        }

        public async Task<GovtDepotResponseDto> UpdateGovtDepotAsync(UpdateGovtDepotDto updateDto)
        {
            var depot = await _context.MasterGovDepots
                .FirstOrDefaultAsync(d => d.GovDepotId == updateDto.GovDepotId);

            if (depot == null)
            {
                throw new KeyNotFoundException($"Govt depot with ID {updateDto.GovDepotId} not found.");
            }

            depot.StateId = updateDto.StateId;
            depot.CircleId = updateDto.CircleId;
            depot.DivisionId = updateDto.DivisionId;
            depot.RangeId = updateDto.RangeId;
            depot.DepotName = updateDto.DepotName;
            depot.Address = updateDto.Address;
            depot.PinCode = updateDto.PinCode;
            depot.Type = updateDto.Type;

            _context.MasterGovDepots.Update(depot);
            await _context.SaveChangesAsync();

            // Return the updated depot with names
            return await GetGovtDepotByIdAsync(depot.GovDepotId);
        }

        public async Task<bool> DeleteGovtDepotAsync(int id)
        {
            var depot = await _context.MasterGovDepots.FindAsync(id);
            if (depot == null)
                return false;

            _context.MasterGovDepots.Remove(depot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GovtDepotExistsAsync(string depotName, int stateId, int circleId, int divisionId, int rangeId, int? excludeId = null)
        {
            var query = _context.MasterGovDepots
                .Where(d => d.DepotName == depotName &&
                           d.StateId == stateId &&
                           d.CircleId == circleId &&
                           d.DivisionId == divisionId &&
                           d.RangeId == rangeId);

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.GovDepotId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // Dropdown Data Methods

        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            return await _context.States
                .OrderBy(s => s.StName)
                .ToListAsync();
        }
    }
}

