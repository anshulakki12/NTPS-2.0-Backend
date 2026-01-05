using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficerService.Repositories;
using OfficerService.Services;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static OfficerService.DtoModels.OfficerLoginDto;

namespace OfficerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficerController : ControllerBase
    {
        private readonly ILogin _loginRepository;
        private readonly ITokenService _tokenService;
        private readonly IMasterRoleService _roleService;
        private readonly IWorkflowRepository _repo;
        public OfficerController(ILogin loginRepository, ITokenService tokenService, IMasterRoleService roleService, IWorkflowRepository repo)
        {
            _loginRepository = loginRepository;
            _tokenService = tokenService;
            _roleService = roleService;
            _repo = repo;
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    // ✅ Validate request
        //    if (request == null ||
        //        string.IsNullOrWhiteSpace(request.EncryptedUsername) ||
        //        string.IsNullOrWhiteSpace(request.EncryptedPassword) ||
        //        string.IsNullOrWhiteSpace(request.PrivateKey))
        //    {
        //        return BadRequest(new { message = "EncryptedUsername, EncryptedPassword, and PrivateKey are required." });
        //    }

        //    string loginId;
        //    string decryptedPassword;

        //    try
        //    {
        //        // ✅ Load private key from PEM
        //        using RSA rsa = RSA.Create();
        //        rsa.ImportFromPem(request.PrivateKey.ToCharArray());

        //        // ✅ Decrypt username and password
        //        loginId = Decrypt(request.EncryptedUsername, rsa);
        //        decryptedPassword = Decrypt(request.EncryptedPassword, rsa);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Invalid encrypted data.", error = ex.Message });
        //    }

        //    try
        //    {
        //        // ✅ Hash decrypted password using SHA-512
        //        using SHA512 sha = SHA512.Create();
        //        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(decryptedPassword));
        //        string hashedPassword = Convert.ToHexString(hashBytes);

        //        // ✅ Validate credentials using repository method
        //        var isValid = await _loginRepository.ValidateOfficerCredentialsAsync(loginId, hashedPassword);
        //        if (!isValid)
        //        {
        //            return Unauthorized(new { message = "Invalid credentials." });
        //        }

        //        // ✅ Get officer details after successful validation
        //        var registration = await _loginRepository.GetByLoginIdAsync(loginId);

        //        // ✅ Use role ID instead of role name string
        //        int roleId = registration.RoleId; // Use the role ID directly

        //        // ✅ Generate JWT token with role ID
        //        var tokenResult = _tokenService.GenerateToken(registration, roleId);

        //        // ✅ Store user session
        //        var sessionObj = new
        //        {
        //            registration.OfficerId,
        //            registration.LoginId,
        //            OfficerName = registration.OfficerDetails?.OfficerName,
        //            Expires = tokenResult.Expires
        //        };
        //        HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(sessionObj));

        //        // ✅ Return success response with officer information and JWT token
        //        return Ok(new
        //        {
        //            message = "Login successful",
        //            token = tokenResult.Token,
        //            expires = tokenResult.Expires,
        //            officer = new
        //            {
        //                registration.OfficerId,
        //                registration.LoginId,
        //                registration.MobileNo,
        //                registration.LocationId,
        //                registration.LocationType,
        //                registration.RoleId,
        //                Role = registration.Role?.RoleName,
        //                OfficerDetails = registration.OfficerDetails != null ? new
        //                {
        //                    registration.OfficerDetails.OfficerDetailId,
        //                    registration.OfficerDetails.OfficerTitle,
        //                    registration.OfficerDetails.OfficerName,
        //                    registration.OfficerDetails.OfficerDesignationId,
        //                    registration.OfficerDetails.OfficerNumber,
        //                    registration.OfficerDetails.EmailAddress
        //                } : null
        //            }
        //        });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return Unauthorized(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
        //    }
        //}

        // ✅ Helper method to decrypt RSA-encrypted string


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // ✅ Validate request
            if (request == null ||
                string.IsNullOrWhiteSpace(request.EncryptedUsername) ||
                string.IsNullOrWhiteSpace(request.EncryptedPassword) ||
                string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return BadRequest(new { message = "EncryptedUsername, EncryptedPassword, and PrivateKey are required." });
            }

            string loginId;
            string decryptedPassword;

            try
            {
                // ✅ Load private key from PEM
                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(request.PrivateKey.ToCharArray());

                // ✅ Decrypt username and password
                loginId = Decrypt(request.EncryptedUsername, rsa);
                decryptedPassword = Decrypt(request.EncryptedPassword, rsa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid encrypted data.", error = ex.Message });
            }

            try
            {
                // ✅ Hash decrypted password using SHA-512
                using SHA512 sha = SHA512.Create();
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(decryptedPassword));
                string hashedPassword = Convert.ToHexString(hashBytes);

                // ✅ Validate credentials using repository method
                var isValid = await _loginRepository.ValidateOfficerCredentialsAsync(loginId, hashedPassword);
                if (!isValid)
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                // ✅ Get officer details after successful validation
                var registration = await _loginRepository.GetByLoginIdAsync(loginId);

                if (registration == null)
                {
                    return Unauthorized(new { message = "User not found or inactive." });
                }

                // ✅ Get role information
                var role = await _roleService.GetRoleByIdAsyncs(registration.RoleId);
                if (role == null || !role.IsActive)
                {
                    return Unauthorized(new { message = "Role not found or inactive." });
                }

                // ✅ Generate JWT token with role claims
                var tokenResult = _tokenService.GenerateToken(registration, role);

                // ✅ Store user session
                var sessionObj = new
                {
                    registration.OfficerId,
                    registration.LoginId,
                    OfficerName = registration.OfficerDetails?.OfficerName,
                    RoleId = registration.RoleId,
                    RoleName = role.RoleName,
                    Expires = tokenResult.Expires
                };
                HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(sessionObj));

                // ✅ Return success response with officer information and JWT token
                return Ok(new
                {
                    message = "Login successful",
                    token = tokenResult.Token,
                    expires = tokenResult.Expires,
                    officer = new
                    {
                        registration.OfficerId,
                        registration.LoginId,
                        registration.MobileNo,
                        registration.LocationId,
                        registration.LocationType,
                        registration.RoleId,
                        Role = role.RoleName,
                        OfficerDetails = registration.OfficerDetails != null ? new
                        {
                            registration.OfficerDetails.OfficerDetailId,
                            registration.OfficerDetails.OfficerTitle,
                            registration.OfficerDetails.OfficerName,
                            registration.OfficerDetails.OfficerDesignationId,
                            registration.OfficerDetails.OfficerNumber,
                            registration.OfficerDetails.EmailAddress
                        } : null
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("check-role-access/{roleId}")]
        public async Task<IActionResult> CheckRoleAccess(int roleId)
        {
            // Verify if the requested role exists and is active
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null || !role.IsActive)
            {
                return NotFound(new
                {
                    message = "Role not found or inactive",
                    hasAccess = false
                });
            }

            return Ok(new
            {
                message = "Role accessible",
                hasAccess = true,
                role = new
                {
                    role.RoleId,
                    role.RoleName,
                    role.IsActive
                }
            });
        }

        private string Decrypt(string base64Encrypted, RSA rsa)
        {
            var encryptedBytes = Convert.FromBase64String(base64Encrypted);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        //[HttpGet("logged-in-person-details/{loginId}")]
        //public async Task<IActionResult> LoggedInPersonDetails(string loginId)
        //{
        //    if (string.IsNullOrWhiteSpace(loginId))
        //        return BadRequest(new { message = "LoginId is required." });

        //    var details = await _loginRepository.GetByLoginIdAsync(loginId);

        //    return Ok(new { details });
        //}

        [HttpGet("logged-in-person-details/{loginId}")]
        public async Task<IActionResult> LoggedInPersonDetails(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return BadRequest(new { message = "LoginId is required." });

            var details = await _loginRepository.GetByLoginIdAsync(loginId);

            if (details == null)
                return NotFound(new { message = "No user found." });

            return Ok(new { details });
        }

        [HttpGet("master-levels")]
        public async Task<IActionResult> GetMasterLevels()
        {
            var levels = await _repo.GetAllMasterLevelsAsync();
            return Ok(levels);
        }

    }
}
