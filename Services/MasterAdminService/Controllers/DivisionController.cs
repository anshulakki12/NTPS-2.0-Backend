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
        [Authorize(Roles = "Applicant")]
        [HttpGet("GetDivisionsByCircle/{circleId}")]
        public async Task<IActionResult> GetDivisionsByCircle(int circleId)
        {
            var divisions = await _repository.GetDivisionsByCircleIdAsync(circleId);
            return Ok(divisions);
        }
    }



}
