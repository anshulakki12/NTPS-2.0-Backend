using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeciesService.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpeciesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeciesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SpeciesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns forest produce (species) for a given state
        /// </summary>
        [HttpGet("GetForestProduceByState/{stateId}")]
        public async Task<IActionResult> GetForestProduceByState(int stateId)
        {
            if (stateId <= 0)
                return BadRequest("Invalid State Id");

            var species = await _context.ForestProduces
                .Where(s => _context.speciesExempted
                    .Where(se => se.StateId == stateId)
                    .Select(se => se.SpeciesId)
                    .Distinct()
                    .Contains(s.ForestProduceId))
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    label = s.Name,
                    value = s.ForestProduceId.ToString()
                })
                .ToListAsync();

            return Ok(species);
        }
    }
}
