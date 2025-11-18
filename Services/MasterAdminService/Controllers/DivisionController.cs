using MasterAdminService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterAdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ControllerBase
    {
        private readonly IDivisionRepository _repository;

        public DivisionController(IDivisionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetDivisionsByCircle/{circleId}")]
        public async Task<IActionResult> GetDivisionsByCircle(int circleId)
        {
            var divisions = await _repository.GetDivisionsByCircleIdAsync(circleId);
            return Ok(divisions);
        }
        // NEW method for multiple circleIds
        [HttpGet("GetDivisionsByCircles")]
        public async Task<IActionResult> GetDivisionsByCircles([FromQuery] string circleIds)
        {
            if (string.IsNullOrEmpty(circleIds))
            {
                return BadRequest("Circle IDs are required.");
            }

            try
            {
                // Convert comma-separated string to list of integers
                var circleIdList = circleIds.Split(',').Select(int.Parse).ToList();
                var divisions = await _repository.GetDivisionsByCircleIdsAsync(circleIdList);
                return Ok(divisions);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid circle ID format.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }



}
