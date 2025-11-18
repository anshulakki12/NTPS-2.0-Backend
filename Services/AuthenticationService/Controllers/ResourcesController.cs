using ApplicantAuthenticationService.Models;
using AuthenticationService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly string _serviceName;

        public ResourcesController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _serviceName = config.GetValue<string>("ServiceName") ?? "ApplicantAuthenticationService";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string culture = "en-US", [FromQuery] string? page = null)
        {
            var query = _db.ResourceCollection.AsNoTracking()
                .Where(r => r.ServiceName == _serviceName && r.Culture == culture);

            if (!string.IsNullOrWhiteSpace(page))
                query = query.Where(r => r.PageOrModule == page);

            var result = await query.ToDictionaryAsync(r => r.ResourceKey, r => r.ResourceValue ?? "");
            return Ok(result);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] ApplicantResourceCollection dto)
        {
            var existing = await _db.ResourceCollection
                .FirstOrDefaultAsync(r => r.ServiceName == _serviceName && r.ResourceKey == dto.ResourceKey && r.Culture == dto.Culture);

            if (existing == null)
                _db.ResourceCollection.Add(dto);
            else
            {
                existing.ResourceValue = dto.ResourceValue;
                existing.UpdatedAt = DateTime.UtcNow;
                _db.ResourceCollection.Update(existing);
            }

            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
