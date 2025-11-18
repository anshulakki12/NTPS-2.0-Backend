using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;

namespace OfficerService.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly AppDbContext _context;

        public ZoneRepository(AppDbContext context)
        {
            _context = context;
        }

        // State methods
        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            return await _context.States
                .OrderBy(s => s.StName)
                .ToListAsync();
        }

        // District methods
        public async Task<IEnumerable<District>> GetDistrictsByStateAsync(int stateCode)
        {
            return await _context.Districts
                .Where(d => d.StCode == stateCode)
                .OrderBy(d => d.DistName)
                .ToListAsync();
        }

        // SubDistrict methods
        public async Task<IEnumerable<SubDistrict>> GetSubDistrictsByDistrictsAsync(List<int> districtIds)
        {
            return await _context.SubDistrict
                .Include(sd => sd.District)
                .Where(sd => districtIds.Contains(sd.DistCode))
                .OrderBy(sd => sd.SubDistName)
                .ToListAsync();
        }

        // Master Zone methods
        public async Task<IEnumerable<MasterZone>> GetAllMasterZonesAsync()
        {
            return await _context.MasterZones
                .Include(mz => mz.State)
                .Where(mz => mz.IsActive)
                .OrderBy(mz => mz.ZoneName)
                .ToListAsync();
        }

        public async Task<MasterZone> GetMasterZoneByIdAsync(int zoneId)
        {
            return await _context.MasterZones
                .Include(mz => mz.State)
                .FirstOrDefaultAsync(mz => mz.ZoneID == zoneId);
        }

        public async Task<MasterZone> CreateMasterZoneAsync(MasterZone masterZone)
        {
            _context.MasterZones.Add(masterZone);
            await _context.SaveChangesAsync();
            return masterZone;
        }

        public async Task<MasterZone> UpdateMasterZoneAsync(MasterZone masterZone)
        {
            _context.MasterZones.Update(masterZone);
            await _context.SaveChangesAsync();
            return masterZone;
        }

        public async Task<bool> DeleteMasterZoneAsync(int zoneId)
        {
            var masterZone = await _context.MasterZones.FindAsync(zoneId);
            if (masterZone == null) return false;

            _context.MasterZones.Remove(masterZone);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleMasterZoneStatusAsync(int zoneId, bool isActive)
        {
            var masterZone = await _context.MasterZones.FindAsync(zoneId);
            if (masterZone == null) return false;

            masterZone.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // Zone Data methods
        public async Task<IEnumerable<ZoneData>> GetAllZoneDataAsync()
        {
            return await _context.ZoneData
                .Include(zd => zd.MasterZone)
                .Include(zd => zd.State)
                .Include(zd => zd.District)
                .Include(zd => zd.SubDistrict)
                .Where(zd => zd.IsActive)
                .OrderBy(zd => zd.MasterZone.ZoneName)
                .ThenBy(zd => zd.District.DistName)
                .ThenBy(zd => zd.SubDistrict.SubDistName)
                .ToListAsync();
        }

        public async Task<ZoneData> GetZoneDataByIdAsync(int id)
        {
            return await _context.ZoneData
                .Include(zd => zd.MasterZone)
                .Include(zd => zd.State)
                .Include(zd => zd.District)
                .Include(zd => zd.SubDistrict)
                .FirstOrDefaultAsync(zd => zd.ID == id);
        }

        public async Task<IEnumerable<ZoneData>> CreateZoneDataBulkAsync(List<ZoneData> zoneDataList)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.ZoneData.AddRange(zoneDataList);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return zoneDataList;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ZoneData> UpdateZoneDataAsync(ZoneData zoneData)
        {
            _context.ZoneData.Update(zoneData);
            await _context.SaveChangesAsync();
            return zoneData;
        }

        public async Task<bool> DeleteZoneDataAsync(int id)
        {
            var zoneData = await _context.ZoneData.FindAsync(id);
            if (zoneData == null) return false;

            _context.ZoneData.Remove(zoneData);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteZoneDataByZoneAsync(int zoneId)
        {
            var zoneDataList = await _context.ZoneData.Where(zd => zd.ZoneID == zoneId).ToListAsync();
            if (!zoneDataList.Any()) return false;

            _context.ZoneData.RemoveRange(zoneDataList);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleZoneDataStatusAsync(int id, bool isActive)
        {
            var zoneData = await _context.ZoneData.FindAsync(id);
            if (zoneData == null) return false;

            zoneData.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ZoneData>> GetZoneDataByZoneIdAsync(int zoneId)
        {
            return await _context.ZoneData
                .Include(zd => zd.MasterZone)
                .Include(zd => zd.State)
                .Include(zd => zd.District)
                .Include(zd => zd.SubDistrict)
                .Where(zd => zd.ZoneID == zoneId && zd.IsActive)
                .ToListAsync();
        }
    }
}