using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;
using OfficerService.Repositories;
using System.ComponentModel.DataAnnotations;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeciesMappingController : ControllerBase
    {
        private readonly ISpeciesMappingRepository _repository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly AppDbContext _context;

        public SpeciesMappingController(
            ISpeciesMappingRepository repository,
            IWorkflowRepository workflowRepository,
            AppDbContext context)
        {
            _repository = repository;
            _workflowRepository = workflowRepository;
            _context = context;
        }

        // Species Mapping Endpoints
        [HttpGet("GetAllSpeciesMappings")]
        public async Task<IActionResult> GetAllSpeciesMappings()
        {
            try
            {
                var mappings = await _repository.GetAllSpeciesMappingsAsync();
                var result = mappings.Select(sm => new SpeciesMappingResponseDto
                {
                    SpeciesMappingId = sm.SpeciesMappingId,
                    ForestProduceId = sm.ForestProduceId,
                    ForestProduceName = sm.ForestProduce?.Name,
                    SpeciesId = sm.SpeciesId,
                    SpeciesName = sm.MasterSpecies?.Name,
                    StateId = sm.StateId,
                    StateName = sm.State?.StName,
                    WorkFlowId = sm.WorkFlowId,
                    WorkFlowName = sm.MasterWorkFlow?.WorkFlowName,
                    ZoneId = sm.ZoneId,
                    ZoneName = sm.MasterZone?.ZoneName,
                    CategoryId = sm.CategoryId,
                    IsActive = sm.IsActive,
                    CreatedOn = sm.CreatedOn
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSpeciesMapping/{speciesMappingId}")]
        public async Task<IActionResult> DeleteSpeciesMapping(int speciesMappingId)
        {
            try
            {
                var result = await _repository.DeleteSpeciesMappingAsync(speciesMappingId);
                if (!result)
                    return NotFound($"Species Mapping with ID {speciesMappingId} not found");

                return Ok(new { message = "Species Mapping deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ToggleSpeciesMappingStatus/{speciesMappingId}/{isActive}")]
        public async Task<IActionResult> ToggleSpeciesMappingStatus(int speciesMappingId, bool isActive)
        {
            try
            {
                var result = await _repository.ToggleSpeciesMappingStatusAsync(speciesMappingId, isActive);
                if (!result)
                    return NotFound($"Species Mapping with ID {speciesMappingId} not found");

                return Ok(new { message = $"Species Mapping {(isActive ? "activated" : "deactivated")} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Related Data Endpoints
        [HttpGet("GetAllForestProduces")]
        public async Task<IActionResult> GetAllForestProduces()
        {
            try
            {
                var forestProduces = await _repository.GetAllForestProducesAsync();
                var result = forestProduces.Select(fp => new
                {
                    forestProduceId = fp.ForestProduceId,
                    name = fp.Name,
                    quantityType = fp.QuantityType,
                    weightType = fp.WeightType,
                    category = fp.Category,
                    requiresLog = fp.RequiresLog
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllSpecies")]
        public async Task<IActionResult> GetAllSpecies()
        {
            try
            {
                var species = await _repository.GetAllSpeciesAsync();
                var result = species.Select(s => new
                {
                    speciesId = s.SpeciesID,
                    name = s.Name
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllZones")]
        public async Task<IActionResult> GetAllZones()
        {
            try
            {
                var zones = await _repository.GetAllZonesAsync();
                var result = zones.Select(z => new
                {
                    zoneId = z.ZoneID,
                    zoneName = z.ZoneName,
                    stateId = z.StateID,
                    stateName = z.State?.StName,
                    isActive = z.IsActive
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetWorkflowsByState/{stateId}")]
        public async Task<IActionResult> GetWorkflowsByState(int stateId)
        {
            try
            {
                var workflows = await _workflowRepository.GetAllMasterWorkflowsAsync();
                var stateWorkflows = workflows
                    .Where(wf => wf.StateId == stateId && wf.IsActive)
                    .Select(wf => new
                    {
                        workFlowId = wf.WorkFlowId,
                        workFlowName = wf.WorkFlowName,
                        isDefault = wf.IsDefault
                    })
                    .OrderBy(wf => wf.workFlowName)
                    .ToList();

                return Ok(stateWorkflows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAvailableSpeciesForStateAndForestProduce/{stateId:int}/{forestProduceId:int}")]
        public async Task<IActionResult> GetAvailableSpeciesForStateAndForestProduce(int stateId, int forestProduceId)
        {
            try
            {
                var availableSpecies = await _repository.GetAvailableSpeciesForStateAndForestProduceAsync(stateId, forestProduceId);

                var result = availableSpecies.Select(s => new
                {
                    speciesId = s.SpeciesID,
                    name = s.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateSpeciesMapping")]
        public async Task<IActionResult> CreateSpeciesMapping([FromBody] SpeciesMappingDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // NEW: Handle NOC category - set ZoneId to null
                int? zoneIdValue = createDto.ZoneId;
                if (createDto.CategoryId == 1) // NOC
                {
                    zoneIdValue = null;
                }

                // Check if species is already mapped to this state and forest produce combination
                var existingMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.SpeciesId == createDto.SpeciesId &&
                        sm.StateId == createDto.StateId &&
                        sm.ForestProduceId == createDto.ForestProduceId &&
                        sm.IsActive);

                if (existingMapping != null)
                {
                    return Conflict($"This species is already mapped to the selected state and forest produce combination.");
                }

                var speciesMapping = new SpeciesMapping
                {
                    ForestProduceId = createDto.ForestProduceId,
                    SpeciesId = createDto.SpeciesId,
                    StateId = createDto.StateId,
                    WorkFlowId = createDto.WorkFlowId,
                    ZoneId = zoneIdValue, // Use the conditional value
                    CategoryId = createDto.CategoryId,
                    IsActive = createDto.IsActive,
                    CreatedOn = DateTime.Now
                };

                var result = await _repository.CreateSpeciesMappingAsync(speciesMapping);
                await LoadNavigationProperties(result);

                var response = new SpeciesMappingResponseDto
                {
                    SpeciesMappingId = result.SpeciesMappingId,
                    ForestProduceId = result.ForestProduceId,
                    ForestProduceName = result.ForestProduce?.Name,
                    SpeciesId = result.SpeciesId,
                    SpeciesName = result.MasterSpecies?.Name,
                    StateId = result.StateId,
                    StateName = result.State?.StName,
                    WorkFlowId = result.WorkFlowId,
                    WorkFlowName = result.MasterWorkFlow?.WorkFlowName,
                    ZoneId = result.ZoneId,
                    ZoneName = result.MasterZone?.ZoneName,
                    CategoryId = result.CategoryId,
                    IsActive = result.IsActive,
                    CreatedOn = result.CreatedOn
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateSpeciesMapping")]
        public async Task<IActionResult> UpdateSpeciesMapping([FromBody] UpdateSpeciesMappingDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingMapping = await _repository.GetSpeciesMappingByIdAsync(updateDto.SpeciesMappingId);
                if (existingMapping == null)
                    return NotFound($"Species Mapping with ID {updateDto.SpeciesMappingId} not found");

                // NEW: Handle NOC category - set ZoneId to null
                int? zoneIdValue = updateDto.ZoneId;
                if (updateDto.CategoryId == 1) // NOC
                {
                    zoneIdValue = null;
                }

                // Check if species is already mapped to this state and forest produce combination (excluding current mapping)
                var duplicateMapping = await _context.SpeciesMapping
                    .FirstOrDefaultAsync(sm =>
                        sm.SpeciesId == updateDto.SpeciesId &&
                        sm.StateId == updateDto.StateId &&
                        sm.ForestProduceId == updateDto.ForestProduceId &&
                        sm.SpeciesMappingId != updateDto.SpeciesMappingId &&
                        sm.IsActive);

                if (duplicateMapping != null)
                {
                    return Conflict($"This species is already mapped to the selected state and forest produce combination.");
                }

                existingMapping.ForestProduceId = updateDto.ForestProduceId;
                existingMapping.SpeciesId = updateDto.SpeciesId;
                existingMapping.StateId = updateDto.StateId;
                existingMapping.WorkFlowId = updateDto.WorkFlowId;
                existingMapping.ZoneId = zoneIdValue; // Use the conditional value
                existingMapping.CategoryId = updateDto.CategoryId;
                existingMapping.IsActive = updateDto.IsActive;

                var result = await _repository.UpdateSpeciesMappingAsync(existingMapping);
                await LoadNavigationProperties(result);

                var response = new SpeciesMappingResponseDto
                {
                    SpeciesMappingId = result.SpeciesMappingId,
                    ForestProduceId = result.ForestProduceId,
                    ForestProduceName = result.ForestProduce?.Name,
                    SpeciesId = result.SpeciesId,
                    SpeciesName = result.MasterSpecies?.Name,
                    StateId = result.StateId,
                    StateName = result.State?.StName,
                    WorkFlowId = result.WorkFlowId,
                    WorkFlowName = result.MasterWorkFlow?.WorkFlowName,
                    ZoneId = result.ZoneId,
                    ZoneName = result.MasterZone?.ZoneName,
                    CategoryId = result.CategoryId,
                    IsActive = result.IsActive,
                    CreatedOn = result.CreatedOn
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task LoadNavigationProperties(SpeciesMapping speciesMapping)
        {
            await _context.Entry(speciesMapping)
                .Reference(sm => sm.ForestProduce)
                .LoadAsync();
            await _context.Entry(speciesMapping)
                .Reference(sm => sm.MasterSpecies)
                .LoadAsync();
            await _context.Entry(speciesMapping)
                .Reference(sm => sm.State)
                .LoadAsync();
            await _context.Entry(speciesMapping)
                .Reference(sm => sm.MasterWorkFlow)
                .LoadAsync();
            await _context.Entry(speciesMapping)
                .Reference(sm => sm.MasterZone)
                .LoadAsync();
        }
    }
}