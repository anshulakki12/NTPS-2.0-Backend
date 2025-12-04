using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using OfficerService.Data;
using OfficerService.DTOs;
using OfficerService.Models;
using OfficerService.Repositories;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GovtDepotsController : ControllerBase
    {
        private readonly IGovtDepotRepository _repository;
        private readonly ILogger<GovtDepotsController> _logger;

        public GovtDepotsController(IGovtDepotRepository repository, ILogger<GovtDepotsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("GetAllGovtDepots")]
        public async Task<ActionResult<IEnumerable<GovtDepotResponseDto>>> GetAllGovtDepots()
        {
            try
            {
                var depots = await _repository.GetAllGovtDepotsAsync();
                return Ok(depots);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all government depots");
                return StatusCode(500, "An error occurred while retrieving government depots");
            }
        }

        [HttpGet("GetGovtDepot/{id}")]
        public async Task<ActionResult<GovtDepotResponseDto>> GetGovtDepot(int id)
        {
            try
            {
                var depot = await _repository.GetGovtDepotByIdAsync(id);
                if (depot == null)
                {
                    return NotFound($"Government depot with ID {id} not found");
                }
                return Ok(depot);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting government depot with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the government depot");
            }
        }

        [HttpGet("GetGovtDepotsByState/{stateId}")]
        public async Task<ActionResult<IEnumerable<GovtDepotResponseDto>>> GetGovtDepotsByState(int stateId)
        {
            try
            {
                var depots = await _repository.GetGovtDepotsByStateAsync(stateId);
                return Ok(depots);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting government depots for state ID {StateId}", stateId);
                return StatusCode(500, "An error occurred while retrieving government depots");
            }
        }

        [HttpPost("CreateGovtDepot")]
        public async Task<ActionResult<GovtDepotResponseDto>> CreateGovtDepot(CreateGovtDepotDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDepot = await _repository.CreateGovtDepotAsync(createDto);
                return CreatedAtAction(nameof(GetGovtDepot), new { id = createdDepot.GovDepotId }, createdDepot);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating government depot");
                return StatusCode(500, "An error occurred while creating the government depot");
            }
        }

        [HttpPut("UpdateGovtDepot/{id}")]
        public async Task<IActionResult> UpdateGovtDepot(int id, UpdateGovtDepotDto updateDto)
        {
            try
            {
                if (id != updateDto.GovDepotId)
                {
                    return BadRequest("ID in route does not match ID in request body");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedDepot = await _repository.UpdateGovtDepotAsync(updateDto);
                return Ok(updatedDepot);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating government depot with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the government depot");
            }
        }

        [HttpDelete("DeleteGovtDepot/{id}")]
        public async Task<IActionResult> DeleteGovtDepot(int id)
        {
            try
            {
                var result = await _repository.DeleteGovtDepotAsync(id);
                if (!result)
                {
                    return NotFound($"Government depot with ID {id} not found");
                }
                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting government depot with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the government depot");
            }
        }

        [HttpGet("GetAllStates")]
        public async Task<ActionResult<IEnumerable<State>>> GetAllStates()
        {
            try
            {
                var states = await _repository.GetAllStatesAsync();
                return Ok(states);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all states");
                return StatusCode(500, "An error occurred while retrieving states");
            }
        }
    }
}

   

