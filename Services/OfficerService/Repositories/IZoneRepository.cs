using OfficerService.Models;

namespace OfficerService.Repositories
{
    public interface IZoneRepository
    {
        // State methods
        Task<IEnumerable<State>> GetAllStatesAsync();

        // District methods
        Task<IEnumerable<District>> GetDistrictsByStateAsync(int stateCode);

        // SubDistrict methods
        Task<IEnumerable<SubDistrict>> GetSubDistrictsByDistrictsAsync(List<int> districtIds);

        // Master Zone methods
        Task<IEnumerable<MasterZone>> GetAllMasterZonesAsync();
        Task<MasterZone> GetMasterZoneByIdAsync(int zoneId);
        Task<MasterZone> CreateMasterZoneAsync(MasterZone masterZone);
        Task<MasterZone> UpdateMasterZoneAsync(MasterZone masterZone);
        Task<bool> DeleteMasterZoneAsync(int zoneId);
        Task<bool> ToggleMasterZoneStatusAsync(int zoneId, bool isActive);

        // Zone Data methods
        Task<IEnumerable<ZoneData>> GetAllZoneDataAsync();
        Task<ZoneData> GetZoneDataByIdAsync(int id);
        Task<IEnumerable<ZoneData>> CreateZoneDataBulkAsync(List<ZoneData> zoneDataList);
        Task<ZoneData> UpdateZoneDataAsync(ZoneData zoneData);
        Task<bool> DeleteZoneDataAsync(int id);
        Task<bool> DeleteZoneDataByZoneAsync(int zoneId);
        Task<bool> ToggleZoneDataStatusAsync(int id, bool isActive);
        Task<IEnumerable<ZoneData>> GetZoneDataByZoneIdAsync(int zoneId);
    }
}