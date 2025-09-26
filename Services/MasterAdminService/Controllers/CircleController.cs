using MasterAdminService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterAdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CircleController : ControllerBase
    {
        private readonly ICircleRepository _repository;

        public CircleController(ICircleRepository repository)
        {
            _repository = repository;
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet("GetCirclesByState/{stateId}")]
        public async Task<IActionResult> GetCirclesByState(int stateId)
        {
            var circles = await _repository.GetCirclesByStateAsync(stateId);
            return Ok(circles);
        }
    }
}
