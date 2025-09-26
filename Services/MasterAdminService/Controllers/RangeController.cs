using MasterAdminService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterAdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RangeController : ControllerBase
    {
        private readonly IRangeRepository _repository;

        public RangeController(IRangeRepository repository)
        {
            _repository = repository;
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet("GetRangesByDivision/{divisionId}")]
        public async Task<IActionResult> GetRangesByDivision(int divisionId)
        {
            var ranges = await _repository.GetRangesByDivisionIdAsync(divisionId);
            return Ok(ranges);
        }
    }
}
