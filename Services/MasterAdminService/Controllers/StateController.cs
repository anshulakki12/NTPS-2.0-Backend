using MasterAdminService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterAdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ControllerBase
    {
        private readonly IStateRepository _repository;

        public StateController(IStateRepository repository)
        {
            _repository = repository;
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet("GetStates")]
        public async Task<IActionResult> GetStates()
        {
            var states = await _repository.GetAllStatesAsync();
            return Ok(states);
        }
    }
}
