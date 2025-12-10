// MasterLoginService/Controllers/MasterLoginController.cs
using MasterLoginService.Data;
using MasterLoginService.DtoModels;
using MasterLoginService.Messaging;
using MasterLoginService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MasterLoginService.DtoModels.LoginRequestDto;

namespace MasterLoginService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterLoginController : ControllerBase
    {
        private readonly MasterLoginDbContext _context;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<MasterLoginController> _logger;

        public MasterLoginController(
            MasterLoginDbContext context,
            IRabbitMqService rabbitMqService,
            ILogger<MasterLoginController> logger)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckUser([FromQuery] string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return BadRequest(new { message = "Login ID is required." });

            // Clean the loginId by removing any prefixes
            var cleanLoginId = CleanLoginId(loginId);

            _logger.LogInformation($"Checking user: {cleanLoginId}");

            var user = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.LoginId == cleanLoginId && u.IsActive);

            if (user == null)
            {
                return Ok(new
                {
                    exists = false,
                    message = "User not found in directory"
                });
            }

            // Add appropriate prefix based on user type for response
            var prefixedLoginId = AddPrefixBasedOnUserType(user.UserType, user.LoginId);

            return Ok(new
            {
                exists = true,
                userType = user.UserType, // Returns string
                loginId = prefixedLoginId,
                rawLoginId = user.LoginId, // Without prefix
                email = user.Email,
                mobile = user.MobileNo,
                name = user.Name,
                registrationType = user.RegistrationType
            });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser([FromBody] SyncUserRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LoginId))
                return BadRequest(new { message = "Invalid request." });

            var cleanLoginId = CleanLoginId(request.LoginId);

            var existingUser = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.LoginId == cleanLoginId);

            if (existingUser == null)
            {
                // Normalize user type
                var normalizedUserType = NormalizeUserType(request.UserType);

                // Create new user
                var masterUser = new MasterUser
                {
                    LoginId = cleanLoginId,
                    Email = request.Email,
                    MobileNo = request.MobileNo,
                    UserType = normalizedUserType, // Store as string
                    Name = request.Name,
                    RegistrationType = request.RegistrationType,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    IsActive = true
                };

                _context.MasterUsers.Add(masterUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created master user record via API for {cleanLoginId} with type {normalizedUserType}");
            }
            else
            {
                // Normalize user type
                var normalizedUserType = NormalizeUserType(request.UserType);

                // Update existing user
                existingUser.Email = request.Email;
                existingUser.MobileNo = request.MobileNo;
                existingUser.UserType = normalizedUserType; // Update as string
                existingUser.Name = request.Name;
                existingUser.RegistrationType = request.RegistrationType;
                existingUser.LastUpdated = DateTime.UtcNow;
                existingUser.IsActive = true;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated master user record via API for {cleanLoginId}");
            }

            return Ok(new { message = "User synchronized successfully." });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? userType = null)
        {
            IQueryable<MasterUser> query = _context.MasterUsers;

            if (!string.IsNullOrEmpty(userType))
            {
                var normalizedUserType = NormalizeUserType(userType);
                query = query.Where(u => u.UserType == normalizedUserType);
            }

            var users = await query
                .Where(u => u.IsActive)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    LoginId = AddPrefixBasedOnUserType(u.UserType, u.LoginId),
                    RawLoginId = u.LoginId,
                    Email = u.Email,
                    MobileNo = u.MobileNo,
                    Name = u.Name,
                    UserType = u.UserType,
                    RegistrationType = u.RegistrationType,
                    CreatedDate = u.CreatedDate,
                    LastUpdated = u.LastUpdated,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                count = users.Count,
                users = users
            });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                LoginId = AddPrefixBasedOnUserType(user.UserType, user.LoginId),
                RawLoginId = user.LoginId,
                Email = user.Email,
                MobileNo = user.MobileNo,
                Name = user.Name,
                UserType = user.UserType,
                RegistrationType = user.RegistrationType,
                CreatedDate = user.CreatedDate,
                LastUpdated = user.LastUpdated,
                IsActive = user.IsActive
            });
        }

        [HttpPut("users/{id}/type")]
        public async Task<IActionResult> UpdateUserType(int id, [FromBody] UpdateUserTypeRequest request)
        {
            var user = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            // Normalize user type
            var normalizedUserType = NormalizeUserType(request.UserType);
            var oldUserType = user.UserType;

            user.UserType = normalizedUserType;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Publish event for other services
            var userTypeEvent = new UserTypeUpdatedEvent
            {
                LoginId = user.LoginId,
                UserType = normalizedUserType
            };

            _rabbitMqService.Publish("user.type.updated", userTypeEvent);

            _logger.LogInformation($"Updated user type for {user.LoginId} from {oldUserType} to {normalizedUserType}");

            return Ok(new
            {
                message = "User type updated successfully.",
                oldUserType = oldUserType,
                newUserType = normalizedUserType
            });
        }

        [HttpPut("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.IsActive = false;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deactivated user: {user.LoginId}");

            return Ok(new { message = "User deactivated successfully." });
        }

        [HttpPut("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var user = await _context.MasterUsers
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            user.IsActive = true;
            user.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Activated user: {user.LoginId}");

            return Ok(new { message = "User activated successfully." });
        }

        private string CleanLoginId(string loginId)
        {
            return loginId
                .Replace("AP-", "", StringComparison.OrdinalIgnoreCase)
                .Replace("OF-", "", StringComparison.OrdinalIgnoreCase)
                .Replace("EN-", "", StringComparison.OrdinalIgnoreCase)
                .Replace("RE-", "", StringComparison.OrdinalIgnoreCase);
        }

        private string AddPrefixBasedOnUserType(string userType, string loginId)
        {
            var normalizedUserType = userType?.ToLower() ?? "applicant";

            return normalizedUserType switch
            {
                "applicant" => $"AP-{loginId}",
                "officer" => $"OF-{loginId}",
                "enumerator" => $"EN-{loginId}",
                "revenue" => $"RE-{loginId}",
                _ => loginId
            };
        }

        private string NormalizeUserType(string userType)
        {
            if (string.IsNullOrWhiteSpace(userType))
                return "Applicant";

            // Convert to proper case
            return userType.ToLower() switch
            {
                "officer" => "Officer",
                "applicant" => "Applicant",
                "revenue" => "Revenue",
                "enumerator" => "Enumerator",
                "1" => "Officer",
                "2" => "Applicant",
                "3" => "Revenue",
                "4" => "Enumerator",
                _ => "Applicant" // Default fallback
            };
        }
    }

    public class SyncUserRequest
    {
        public string LoginId { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string UserType { get; set; } = "Applicant";
        public string Name { get; set; }
        public string RegistrationType { get; set; }
    }

    public class UpdateUserTypeRequest
    {
        public string UserType { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string LoginId { get; set; } // With prefix
        public string RawLoginId { get; set; } // Without prefix
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string UserType { get; set; } // String representation
        public string RegistrationType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
    }
}