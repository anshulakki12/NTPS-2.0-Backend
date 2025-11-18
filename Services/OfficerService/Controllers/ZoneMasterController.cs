using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.Models;
using OfficerService.Repositories;
using System.ComponentModel.DataAnnotations;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZoneMasterController : ControllerBase
    {
        private readonly IZoneRepository _repository;
        private readonly AppDbContext _context;

        public ZoneMasterController(
            IZoneRepository repository,
            AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // State Endpoints
        [HttpGet("GetAllStates")]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var states = await _repository.GetAllStatesAsync();
                var result = states.Select(s => new
                {
                    stateId = s.Id,
                    stateName = s.StName,
                    stCode = s.StCode,
                    stUt = s.StUt,
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // District Endpoints
        [HttpGet("GetDistrictsByState/{stateCode}")]
        public async Task<IActionResult> GetDistrictsByState(int stateCode)
        {
            try
            {
                var districts = await _repository.GetDistrictsByStateAsync(stateCode);
                var result = districts.Select(d => new
                {
                    districtId = d.DistCode,
                    districtName = d.DistName,
                    stateId = d.StCode,
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // SubDistrict Endpoints
        [HttpGet("GetSubDistrictsByDistricts")]
        public async Task<IActionResult> GetSubDistrictsByDistricts([FromQuery] string districtIds)
        {
            try
            {
                if (string.IsNullOrEmpty(districtIds))
                {
                    return BadRequest("District IDs are required.");
                }

                var districtIdList = districtIds.Split(',').Select(int.Parse).ToList();
                var subDistricts = await _repository.GetSubDistrictsByDistrictsAsync(districtIdList);
                var result = subDistricts.Select(sd => new
                {
                    subDistrictId = sd.SubDistCode,
                    subDistrictName = sd.SubDistName,
                    districtId = sd.DistCode,
                    districtName = sd.District?.DistName,
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Master Zone Endpoints
        [HttpGet("GetAllMasterZones")]
        public async Task<IActionResult> GetAllMasterZones()
        {
            try
            {
                var masterZones = await _repository.GetAllMasterZonesAsync();
                var result = masterZones.Select(mz => new MasterZoneResponseDto
                {
                    ZoneID = mz.ZoneID,
                    ZoneName = mz.ZoneName,
                    StateID = mz.StateID,
                    StateName = mz.State?.StName,
                    StCode = mz.State?.StCode ?? 0,
                    IsActive = mz.IsActive,
                    CreatedDate = mz.CreatedDate,
                    CreatedBy = mz.CreatedBy
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateMasterZone")]
        public async Task<IActionResult> CreateMasterZone([FromBody] CreateMasterZoneDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if zone name already exists
                var existingZone = await _context.MasterZones
                    .FirstOrDefaultAsync(mz => mz.ZoneName == createDto.ZoneName && mz.StateID == createDto.StateID);

                if (existingZone != null)
                {
                    return Conflict($"Zone with name '{createDto.ZoneName}' already exists in this state.");
                }

                var masterZone = new MasterZone
                {
                    ZoneName = createDto.ZoneName,
                    StateID = createDto.StateID,
                    IsActive = createDto.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "System" // You can get this from authenticated user
                };

                var result = await _repository.CreateMasterZoneAsync(masterZone);

                // Include State in the response
                await _context.Entry(result).Reference(mz => mz.State).LoadAsync();

                var response = new MasterZoneResponseDto
                {
                    ZoneID = result.ZoneID,
                    ZoneName = result.ZoneName,
                    StateID = result.StateID,
                    StateName = result.State?.StName,
                    StCode = result.State?.StCode ?? 0,
                    IsActive = result.IsActive,
                    CreatedDate = result.CreatedDate,
                    CreatedBy = result.CreatedBy
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateMasterZone")]
        public async Task<IActionResult> UpdateMasterZone([FromBody] UpdateMasterZoneDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingZone = await _repository.GetMasterZoneByIdAsync(updateDto.ZoneID);
                if (existingZone == null)
                    return NotFound($"Master Zone with ID {updateDto.ZoneID} not found");

                // Check if zone name already exists (excluding current zone)
                var duplicateZone = await _context.MasterZones
                    .FirstOrDefaultAsync(mz => mz.ZoneName == updateDto.ZoneName
                                            && mz.StateID == updateDto.StateID
                                            && mz.ZoneID != updateDto.ZoneID);

                if (duplicateZone != null)
                {
                    return Conflict($"Zone with name '{updateDto.ZoneName}' already exists in this state.");
                }

                existingZone.ZoneName = updateDto.ZoneName;
                existingZone.StateID = updateDto.StateID;
                existingZone.IsActive = updateDto.IsActive;

                var result = await _repository.UpdateMasterZoneAsync(existingZone);

                // Include State in the response
                await _context.Entry(result).Reference(mz => mz.State).LoadAsync();

                var response = new MasterZoneResponseDto
                {
                    ZoneID = result.ZoneID,
                    ZoneName = result.ZoneName,
                    StateID = result.StateID,
                    StateName = result.State?.StName,
                    StCode = result.State?.StCode ?? 0,
                    IsActive = result.IsActive,
                    CreatedDate = result.CreatedDate,
                    CreatedBy = result.CreatedBy
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteMasterZone/{zoneId}")]
        public async Task<IActionResult> DeleteMasterZone(int zoneId)
        {
            try
            {
                // First delete all zone data associated with this zone
                await _repository.DeleteZoneDataByZoneAsync(zoneId);

                var result = await _repository.DeleteMasterZoneAsync(zoneId);
                if (!result)
                    return NotFound($"Master Zone with ID {zoneId} not found");

                return Ok(new { message = "Master Zone deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ToggleMasterZoneStatus/{zoneId}/{isActive}")]
        public async Task<IActionResult> ToggleMasterZoneStatus(int zoneId, bool isActive)
        {
            try
            {
                var result = await _repository.ToggleMasterZoneStatusAsync(zoneId, isActive);
                if (!result)
                    return NotFound($"Master Zone with ID {zoneId} not found");

                return Ok(new { message = $"Master Zone {(isActive ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Zone Data Endpoints
        [HttpPost("CreateZoneDataBulk")]
        public async Task<IActionResult> CreateZoneDataBulk([FromBody] CreateZoneDataDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // First, delete existing zone data for this zone to avoid duplicates
                await _repository.DeleteZoneDataByZoneAsync(createDto.ZoneID);

                var zoneDataList = new List<ZoneData>();

                // Create entries for districts without sub-districts
                foreach (var districtId in createDto.DistrictIDs)
                {
                    // Check if this district has any selected sub-districts
                    var hasSubDistricts = createDto.SubDistrictIDs.Any(sdId =>
                        _context.SubDistrict.Any(sd => sd.SubDistCode == sdId && sd.DistCode == districtId));

                    if (!hasSubDistricts)
                    {
                        zoneDataList.Add(new ZoneData
                        {
                            ZoneID = createDto.ZoneID,
                            StateID = createDto.StateID,
                            DistrictID = districtId,
                            SubDistrictID = null,
                            IsActive = createDto.IsActive,
                            CreatedDate = DateTime.Now,
                            CreatedBy = "System"
                        });
                    }
                }

                // Create entries for sub-districts
                foreach (var subDistrictId in createDto.SubDistrictIDs)
                {
                    var subDistrict = await _context.SubDistrict.FindAsync(subDistrictId);
                    if (subDistrict != null)
                    {
                        zoneDataList.Add(new ZoneData
                        {
                            ZoneID = createDto.ZoneID,
                            StateID = createDto.StateID,
                            DistrictID = subDistrict.DistCode,
                            SubDistrictID = subDistrictId,
                            IsActive = createDto.IsActive,
                            CreatedDate = DateTime.Now,
                            CreatedBy = "System"
                        });
                    }
                }

                // Remove duplicates
                zoneDataList = zoneDataList
                    .GroupBy(zd => new { zd.ZoneID, zd.DistrictID, zd.SubDistrictID })
                    .Select(g => g.First())
                    .ToList();

                var result = await _repository.CreateZoneDataBulkAsync(zoneDataList);

                // Load navigation properties for response
                foreach (var item in result)
                {
                    await _context.Entry(item)
                        .Reference(zd => zd.MasterZone)
                        .LoadAsync();
                    await _context.Entry(item)
                        .Reference(zd => zd.State)
                        .LoadAsync();
                    await _context.Entry(item)
                        .Reference(zd => zd.District)
                        .LoadAsync();
                    if (item.SubDistrictID.HasValue)
                    {
                        await _context.Entry(item)
                            .Reference(zd => zd.SubDistrict)
                            .LoadAsync();
                    }
                }

                var response = result.Select(zd => new ZoneDataResponseDto
                {
                    ID = zd.ID,
                    ZoneID = zd.ZoneID,
                    ZoneName = zd.MasterZone?.ZoneName,
                    StateID = zd.StateID,
                    StateName = zd.State?.StName,
                    StCode = zd.State?.StCode ?? 0,
                    DistrictID = zd.DistrictID,
                    DistrictName = zd.District?.DistName,
                    SubDistrictID = zd.SubDistrictID,
                    SubDistrictName = zd.SubDistrict?.SubDistName,
                    IsActive = zd.IsActive,
                    CreatedDate = zd.CreatedDate,
                    CreatedBy = zd.CreatedBy
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllZoneData")]
        public async Task<IActionResult> GetAllZoneData()
        {
            try
            {
                var zoneData = await _repository.GetAllZoneDataAsync();
                var result = zoneData.Select(zd => new ZoneDataResponseDto
                {
                    ID = zd.ID,
                    ZoneID = zd.ZoneID,
                    ZoneName = zd.MasterZone?.ZoneName,
                    StateID = zd.StateID,
                    StateName = zd.State?.StName,
                    StCode = zd.State?.StCode ?? 0,
                    DistrictID = zd.DistrictID,
                    DistrictName = zd.District?.DistName,
                    SubDistrictID = zd.SubDistrictID,
                    SubDistrictName = zd.SubDistrict?.SubDistName,
                    IsActive = zd.IsActive,
                    CreatedDate = zd.CreatedDate,
                    CreatedBy = zd.CreatedBy
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateZoneData")]
        public async Task<IActionResult> UpdateZoneData([FromBody] UpdateZoneDataDto updateDto)
        {
            try
            {
                var existingData = await _repository.GetZoneDataByIdAsync(updateDto.ID);
                if (existingData == null)
                    return NotFound($"Zone Data with ID {updateDto.ID} not found");

                existingData.ZoneID = updateDto.ZoneID;
                existingData.StateID = updateDto.StateID;
                existingData.DistrictID = updateDto.DistrictID;
                existingData.SubDistrictID = updateDto.SubDistrictID;
                existingData.IsActive = updateDto.IsActive;

                var result = await _repository.UpdateZoneDataAsync(existingData);

                // Load navigation properties
                await _context.Entry(result)
                    .Reference(zd => zd.MasterZone)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(zd => zd.State)
                    .LoadAsync();
                await _context.Entry(result)
                    .Reference(zd => zd.District)
                    .LoadAsync();
                if (result.SubDistrictID.HasValue)
                {
                    await _context.Entry(result)
                        .Reference(zd => zd.SubDistrict)
                        .LoadAsync();
                }

                var response = new ZoneDataResponseDto
                {
                    ID = result.ID,
                    ZoneID = result.ZoneID,
                    ZoneName = result.MasterZone?.ZoneName,
                    StateID = result.StateID,
                    StateName = result.State?.StName,
                    StCode = result.State?.StCode ?? 0,
                    DistrictID = result.DistrictID,
                    DistrictName = result.District?.DistName,
                    SubDistrictID = result.SubDistrictID,
                    SubDistrictName = result.SubDistrict?.SubDistName,
                    IsActive = result.IsActive,
                    CreatedDate = result.CreatedDate,
                    CreatedBy = result.CreatedBy
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteZoneData/{id}")]
        public async Task<IActionResult> DeleteZoneData(int id)
        {
            try
            {
                var result = await _repository.DeleteZoneDataAsync(id);
                if (!result)
                    return NotFound($"Zone Data with ID {id} not found");

                return Ok(new { message = "Zone Data deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ToggleZoneDataStatus/{id}/{isActive}")]
        public async Task<IActionResult> ToggleZoneDataStatus(int id, bool isActive)
        {
            try
            {
                var result = await _repository.ToggleZoneDataStatusAsync(id, isActive);
                if (!result)
                    return NotFound($"Zone Data with ID {id} not found");

                return Ok(new { message = $"Zone Data {(isActive ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetZoneDataByZone/{zoneId}")]
        public async Task<IActionResult> GetZoneDataByZone(int zoneId)
        {
            try
            {
                var zoneData = await _repository.GetZoneDataByZoneIdAsync(zoneId);
                var result = zoneData.Select(zd => new ZoneDataResponseDto
                {
                    ID = zd.ID,
                    ZoneID = zd.ZoneID,
                    ZoneName = zd.MasterZone?.ZoneName,
                    StateID = zd.StateID,
                    StateName = zd.State?.StName,
                    StCode = zd.State?.StCode ?? 0,
                    DistrictID = zd.DistrictID,
                    DistrictName = zd.District?.DistName,
                    SubDistrictID = zd.SubDistrictID,
                    SubDistrictName = zd.SubDistrict?.SubDistName,
                    IsActive = zd.IsActive,
                    CreatedDate = zd.CreatedDate,
                    CreatedBy = zd.CreatedBy
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // DTO Classes
    public class CreateMasterZoneDto
    {
        [Required]
        [StringLength(100)]
        public string ZoneName { get; set; }

        [Required]
        public int StateID { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateMasterZoneDto
    {
        [Required]
        public int ZoneID { get; set; }

        [Required]
        [StringLength(100)]
        public string ZoneName { get; set; }

        [Required]
        public int StateID { get; set; }

        public bool IsActive { get; set; }
    }

    public class MasterZoneResponseDto
    {
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int StCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CreateZoneDataDto
    {
        [Required]
        public int ZoneID { get; set; }

        [Required]
        public int StateID { get; set; }

        public List<int> DistrictIDs { get; set; } = new List<int>();
        public List<int> SubDistrictIDs { get; set; } = new List<int>();
        public bool IsActive { get; set; } = true;
    }

    public class UpdateZoneDataDto
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int ZoneID { get; set; }

        [Required]
        public int StateID { get; set; }

        [Required]
        public int DistrictID { get; set; }

        public int? SubDistrictID { get; set; }
        public bool IsActive { get; set; }
    }

    public class ZoneDataResponseDto
    {
        public int ID { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int StCode { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int? SubDistrictID { get; set; }
        public string SubDistrictName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}