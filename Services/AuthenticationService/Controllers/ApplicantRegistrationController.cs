using AuthenticationService.DtoModels;
using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tesseract;
using static AuthenticationService.DtoModels.RegstrationDto;
//using Tesseract;
namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantRegistrationController : ControllerBase
    {
        private readonly IApplicantRegistrationRepository _repository;
        private readonly string _tessDataPath;
        private readonly IOtpService _otpService;
        private readonly IPasswordService _passwordHelper;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;
        public ApplicantRegistrationController(
            IApplicantRegistrationRepository repository,
            IWebHostEnvironment env,
            IOtpService otpService,
            IPasswordService passwordHelper,
            ITokenService tokenService)
        {
            _repository = repository;
            // Path where tessdata is stored (Download from: https://github.com/tesseract-ocr/tessdata)
            _tessDataPath = Path.Combine(env.ContentRootPath, "tessdata");
            _otpService = otpService;
            _passwordHelper = passwordHelper;
            _env = env;
            _tokenService = tokenService;
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registrations = await _repository.GetAllAsync();
            return Ok(registrations);
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var registration = await _repository.GetByIdAsync(id);
            if (registration == null)
                return NotFound(new { message = $"Registration with ID {id} not found." });

            return Ok(registration);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EncryptedRegistrationRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid data." });

            if (string.IsNullOrWhiteSpace(request.EmailId) ||
                string.IsNullOrWhiteSpace(request.CipherText) ||
                string.IsNullOrWhiteSpace(request.LoginId) ||
                string.IsNullOrWhiteSpace(request.MobileNo) ||
                string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return BadRequest(new { message = "Email, LoginId, MobileNo, CipherText and PrivateKey are required." });
            }

            // ✅ Load private key
            string decryptedPassword;
            string decryptedMobile;
            string decryptedLoginId;

            try
            {
                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(request.PrivateKey.ToCharArray());

                decryptedPassword = Decrypt(request.CipherText, rsa);
                decryptedMobile = Decrypt(request.MobileNo, rsa);
                decryptedLoginId = Decrypt(request.LoginId, rsa);
            }
            catch
            {
                return BadRequest(new { message = "Invalid encrypted data." });
            }

            // ✅ Check duplicate
            var existing = await _repository.GetByLoginIdAsync(decryptedLoginId);
            if (existing != null)
            {
                if (existing.IsVerified == 'Y')
                    return Conflict(new { code = "MOBILE_EXISTS_VERIFIED", message = "Mobile number already registered." });

                if (existing.IsVerified == 'N')
                    return Conflict(new { code = "MOBILE_EXISTS_NOT_VERIFIED", message = "Registration is incomplete for this mobile number." });
            }

            if (string.IsNullOrWhiteSpace(decryptedMobile) || !Regex.IsMatch(decryptedMobile, "^[0-9]{10}$"))
            {
                return BadRequest(new { message = "Invalid mobile number." });
            }

            // ✅ Hash password (same as login)
            using SHA512 sha = SHA512.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(decryptedPassword));
            string hashedPassword = Convert.ToHexString(hashBytes);

            // ✅ Saves
            var registration = new ApplicantRegistration
            {
                RegistrationType = request.RegistrationType,
                NameTitle = request.NameTitle,
                Name = request.Name,
                EmailId = request.EmailId,
                LoginId = decryptedLoginId,
                Password = hashedPassword,
                MobileNo = decryptedMobile,
                IsVerified = request.IsVerified,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(registration);

            return CreatedAtAction(nameof(GetById),
                new { id = created.RegistrationId },
                new { message = "Registration successful", id = created.RegistrationId });
        }


        //[Authorize(Roles = "Applicant")]
        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOtp([FromBody] string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return BadRequest(new { message = "Mobile number is required" });

            // Check existing record
            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(mobileNumber);
            int attemptCount = lastOtpRecord?.Attempt + 1 ?? 1;

            if (attemptCount > 3)
                return BadRequest(new { message = "Maximum OTP attempts reached." });

            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            if (lastOtpRecord == null)
            {
                var verifyOtpEntity = new VerifyOtp
                {
                    MobileNo = mobileNumber,
                    ApplicationId = null,
                    OtpCasesId = null,
                    Status = "N",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Otp = otp,
                    Attempt = attemptCount
                };

                await _otpService.AddAsync(verifyOtpEntity); // must call SaveChangesAsync()
            }
            else
            {
                lastOtpRecord.Otp = otp;
                lastOtpRecord.Attempt = attemptCount;
                lastOtpRecord.Status = "N";
                lastOtpRecord.UpdatedDate = DateTime.UtcNow;
                await _otpService.UpdateOtpRecordAsync(lastOtpRecord);
            }

            return Ok(new { message = "OTP sent successfully", otp, attempt = attemptCount });
        }

        //[Authorize(Roles = "Applicant")]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationModel request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Mobile number and OTP are required." });
            }

            // Fetch the latest OTP record for this mobile
            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(request.MobileNumber);

            if (lastOtpRecord == null)
            {
                return BadRequest(new { message = "No OTP found for this number. Please request a new one." });
            }

            // Mark as verified
            lastOtpRecord.Status = "Y";
            //lastOtpRecord.IsVerified = true; // optional, if in your entity
            lastOtpRecord.UpdatedDate = DateTime.UtcNow;

            await _otpService.UpdateOtpRecordAsync(lastOtpRecord);

            return Ok(new { message = "OTP verified successfully." });
        }

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
            catch
            {
                return BadRequest(new { message = "Invalid encrypted data." });
            }

            // ✅ Get user record by loginId
            var registration = await _repository.GetByLoginIdAsync(loginId);
            if (registration == null)
                return Unauthorized(new { message = "Invalid credentials." });

            // ✅ Hash decrypted password using SHA-512
            using SHA512 sha = SHA512.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(decryptedPassword));
            string hashedPassword = Convert.ToHexString(hashBytes);

            // ✅ Compare hashed password with stored password
            if (!string.Equals(registration.Password, hashedPassword, StringComparison.Ordinal))
                return Unauthorized(new { message = "Invalid credentials." });

            // ✅ Default role = Applicant if not provided
            var role = (registration.UserRole.HasValue && registration.UserRole.Value != 0) ? registration.UserRole.Value : 1;

            // ✅ Generate JWT token
            var tokenResult = _tokenService.GenerateToken(registration, role);

            // ✅ Store user session
            var sessionObj = new
            {
                registration.RegistrationId,
                registration.LoginId,
                registration.Name,
                Expires = tokenResult.Expires
            };
            HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(sessionObj));

            // ✅ Return token and user info
            return Ok(new
            {
                message = "Login successful",
                token = tokenResult.Token,
                expires = tokenResult.Expires,
                user = new
                {
                    registration.RegistrationId,
                    registration.LoginId,
                    registration.Name,
                    registration.EmailId
                }
            });
        }

        // ✅ Helper method to decrypt RSA-encrypted string
        private string Decrypt(string base64Encrypted, RSA rsa)
        {
            var encryptedBytes = Convert.FromBase64String(base64Encrypted);
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        [HttpGet("check-mobile/{mobileNo}")]
        public async Task<IActionResult> CheckMobile(string mobileNo)
        {
            if (string.IsNullOrWhiteSpace(mobileNo) || !Regex.IsMatch(mobileNo, "^[0-9]{10}$"))
                return BadRequest(new { message = "Invalid mobile number." });

            // Fetch latest OTP/Registration record

            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(mobileNo);
            var registration = await _repository.GetByLoginIdAsync(mobileNo);

            // ✅ Case 1: Mobile exists & verified
            if (registration != null)
            {
                if (registration.IsVerified == 'Y')
                {
                    return Conflict(new { code = "MOBILE_EXISTS_VERIFIED" });
                }
                else if (registration.IsVerified == 'N')
                {
                    return Conflict(new { code = "MOBILE_EXISTS_NOT_VERIFIED" });
                }
            }

            // ✅ Case 2: Exists but not verified, attempts > 3 within 30 min
            if (registration != null && registration.IsVerified == 'N')
            {
                if (lastOtpRecord != null && lastOtpRecord.Status == "N")
                {
                    var minutesDiff = (DateTime.UtcNow - lastOtpRecord.UpdatedDate.Value).TotalMinutes;
                    if (lastOtpRecord.Attempt > 3 && minutesDiff <= 30)
                    {
                        return BadRequest(new { message = $"Too many OTP attempts. Please try again after {30 - (int)minutesDiff} minutes." });
                    }
                }
            }

            return Ok(new { message = "Mobile number is available for registration." });
        }

        [HttpGet("fetch/{aadhaarNumber}")]
        public IActionResult FetchDetails(string aadhaarNumber)
        {
            // 🔹 Mock data – later replace with real API integration
            var userData = new
            {
                AadhaarNumber = aadhaarNumber,
                Name = aadhaarNumber.EndsWith("1234") ? "Anshul Dixit" : "Anshul Dixit",
                Dob = "1998-03-04",
                Gender = "Male",
                Address = "New Delhi, India",
                Pincode = "110001"
            };

            return Ok(userData);
        }


        [HttpPost("save")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> SaveApplicantDetails([FromForm] ApplicantPersonalDetailsDto dto, IFormFile? file)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid data" });

            // Aadhaar case (no file needed)
            if (dto.IDProof == Enums.ApplicantEnums.IdentityProof.Aadhar)
            {
                dto.IDUpload = null;
            }

            else
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Document upload required" });

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".pdf")
                    return BadRequest(new { message = "Only PDF files are allowed" });

                // 🔹 Choose folder based on IdentityProof
                var proofFolder = dto.IDProof.ToString(); // e.g., "PAN", "VoterID", etc.
                var uploadsPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "identity-docs", proofFolder);

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // 🔹 Unique file name
                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                dto.IDUpload = Path.Combine(proofFolder, uniqueFileName);
                // store relative path like "PAN/xxxx.pdf"
            }

            var entity = new ApplicantPersonalDetails
            {
                LoginId = dto.LoginId,
                IDProof = dto.IDProof,
                IDNumber = dto.IDNumber,
                IDUpload = dto.IDUpload,
                StateId = dto.StateId,
                CircleId = dto.CircleId,
                DivisionId = dto.DivisionId,
                SubDivisionId = dto.SubDivisionId,
                RangeId = dto.RangeId,
                Address = dto.Address,
                PinCode = dto.PinCode,
                Name = dto.Name,
                Email = dto.Email,
                SourceType = dto.SourceType,
                CreatedDate = DateTime.UtcNow
            };

            var saved = await _repository.AddAsyncApplicant(entity);

            return Ok(new { message = "Saved successfully", id = saved.DetailsId });
        }
        [Authorize(Roles = "Applicant")]
        [HttpGet("has-personal-details/{loginId}")]
        public async Task<IActionResult> HasPersonalDetails(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return BadRequest(new { message = "LoginId is required." });

            var details = await _repository.GetApplicantPersonalDetailsByLoginIdAsync(loginId);

            bool hasDetails = details != null;
            return Ok(new { hasDetails });
        }

        [Authorize(Roles = "Applicant")]
        [HttpGet("logged-in-person-details/{loginId}")]
        public async Task<IActionResult> LoggedInPersonDetails(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return BadRequest(new { message = "LoginId is required." });

            var details = await _repository.GetApplicantLoggedPersonalDetailsByLoginIdAsync(loginId);

            return Ok(new { details });
        }

        [HttpGet("getApplicantDetails/{loginId}")]
        public async Task<IActionResult> GetApplicantDetails(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return BadRequest(new { message = "LoginId is required" });

            var details = await _repository.GetApplicantPersonalDetailsByLoginIdAsync(loginId);

            if (details == null)
                return NotFound(new { message = "Applicant details not found" });

            return Ok(details);
        }

        [HttpPut("updateApplicantDetails")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UpdateApplicantDetails([FromForm] ApplicantPersonalDetailsDto dto, IFormFile? file)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.LoginId))
                return BadRequest(new { message = "Invalid request data" });

            var existing = await _repository.GetApplicantPersonalDetailsByLoginIdAsync(dto.LoginId);

            if (existing == null)
                return NotFound(new { message = "Applicant not found" });

            // 🟩 Title and Name are NOT allowed to update
            // So we skip updating those two

            if (dto.IDProof == Enums.ApplicantEnums.IdentityProof.Aadhar)
            {
                existing.IDProof = dto.IDProof;
                existing.IDUpload = null;
                existing.IDNumber = dto.IDNumber;
            }
            else
            {
                if (file != null && file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (ext != ".pdf")
                        return BadRequest(new { message = "Only PDF files are allowed" });

                    var proofFolder = dto.IDProof.ToString();
                    var uploadsPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "identity-docs", proofFolder);
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existing.IDUpload = Path.Combine(proofFolder, uniqueFileName);
                }

                existing.IDProof = dto.IDProof;
                existing.IDNumber = dto.IDNumber;
            }

            // Update other allowed fields
            existing.StateId = dto.StateId;
            existing.CircleId = dto.CircleId;
            existing.DivisionId = dto.DivisionId;
            existing.SubDivisionId = dto.SubDivisionId;
            existing.RangeId = dto.RangeId;
            existing.Address = dto.Address;
            existing.PinCode = dto.PinCode;
            existing.Email = dto.Email;
            existing.SourceType = dto.SourceType;
            existing.ModificationDate = DateTime.UtcNow;

            var saved = await _repository.UpdateApplicantAsync(existing);

            return Ok(new { message = "Applicant details updated successfully" });
        }

        private static string DecryptBase64WithRsa(string base64Cipher, RSA rsa, RSAEncryptionPadding padding)
        {
            if (string.IsNullOrWhiteSpace(base64Cipher))
                return string.Empty;

            var cipherBytes = Convert.FromBase64String(base64Cipher);
            var plainBytes = rsa.Decrypt(cipherBytes, padding);
            return Encoding.UTF8.GetString(plainBytes);
        }


        [HttpPost("check-loginid-forgot-password")]
        public async Task<IActionResult> CheckLoginIdForForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LoginId))
                return BadRequest(new { message = "Login ID is required." });

            // Check in Applicant_Registration table
            var applicant = await _repository.GetByLoginIdAsync(request.LoginId);
            if (applicant != null)
            {
                if (applicant.IsVerified == 'Y')
                {
                    return Ok(new CheckLoginIdResponse
                    {
                        Exists = true,
                        IsActive = true,
                        MobileNo = applicant.MobileNo,
                        Role = "Applicant",
                        LoginId = applicant.LoginId,
                        Message = "User found and verified"
                    });
                }
                else
                {
                    return Ok(new CheckLoginIdResponse
                    {
                        Exists = true,
                        IsActive = false,
                        MobileNo = applicant.MobileNo,
                        Role = "Applicant",
                        LoginId = applicant.LoginId,
                        Message = "Account not verified"
                    });
                }
            }

            return Ok(new CheckLoginIdResponse
            {
                Exists = false,
                IsActive = false,
                Message = "You are not a registered user"
            });
        }

        [HttpPost("send-otp-forgot-password")]
        public async Task<IActionResult> SendOtpForgotPassword([FromBody] OtpVerificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                return BadRequest(new { message = "Mobile number is required" });

            // Check OTP attempts (similar to your existing logic)
            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(request.MobileNumber);
            int attemptCount = lastOtpRecord?.Attempt + 1 ?? 1;

            // Check if user has exceeded attempts (3 attempts within 30 minutes)
            if (lastOtpRecord != null && lastOtpRecord.Attempt >= 3)
            {
                var minutesDiff = (DateTime.UtcNow - lastOtpRecord.CreatedDate.Value).TotalMinutes;
                if (minutesDiff <= 30)
                {
                    return BadRequest(new
                    {
                        code = "BLOCKED",
                        blockedTill = lastOtpRecord.CreatedDate.Value.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss"),
                        message = $"Too many OTP attempts. Please try again after {30 - (int)minutesDiff} minutes."
                    });
                }
            }

            var otp = new Random().Next(100000, 999999).ToString();

            if (lastOtpRecord == null)
            {
                var verifyOtpEntity = new VerifyOtp
                {
                    MobileNo = request.MobileNumber,
                    ApplicationId = null,
                    OtpCasesId = 4, // 4 for forgot password
                    Status = "N",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Otp = otp,
                    Attempt = attemptCount
                };

                await _otpService.AddAsync(verifyOtpEntity);
            }
            else
            {
                lastOtpRecord.Otp = otp;
                lastOtpRecord.Attempt = attemptCount;
                lastOtpRecord.Status = "N";
                lastOtpRecord.UpdatedDate = DateTime.UtcNow;
                lastOtpRecord.OtpCasesId = 4; // Set case ID for forgot password
                await _otpService.UpdateOtpRecordAsync(lastOtpRecord);
            }

            // In production, you would send the OTP via SMS
            // For development, return it in response
            return Ok(new
            {
                message = "OTP sent successfully",
                otp, // Remove this in production
                attempt = attemptCount
            });
        }

        [HttpPost("verify-otp-forgot-password")]
        public async Task<IActionResult> VerifyOtpForgotPassword([FromBody] OtpVerificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Mobile number and OTP are required." });
            }

            // Fetch the latest OTP record for this mobile
            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(request.MobileNumber);

            if (lastOtpRecord == null)
            {
                return BadRequest(new { message = "No OTP found for this number. Please request a new one." });
            }

            // Check if OTP matches and is not expired (5 minutes)
            var minutesDiff = (DateTime.UtcNow - lastOtpRecord.CreatedDate.Value).TotalMinutes;
            if (minutesDiff > 5)
            {
                return BadRequest(new { message = "OTP has expired. Please request a new one." });
            }

            if (lastOtpRecord.Otp != request.Otp)
            {
                // Increment attempt count
                lastOtpRecord.Attempt++;
                lastOtpRecord.UpdatedDate = DateTime.UtcNow;
                await _otpService.UpdateOtpRecordAsync(lastOtpRecord);

                return BadRequest(new { message = "Invalid OTP. Please try again." });
            }

            // Mark as verified
            lastOtpRecord.Status = "Y";
            lastOtpRecord.UpdatedDate = DateTime.UtcNow;
            await _otpService.UpdateOtpRecordAsync(lastOtpRecord);

            return Ok(new { message = "OTP verified successfully." });
        }

        [HttpPost("reset-password-forgot")]
        public async Task<IActionResult> ResetPasswordForgot([FromBody] ResetPasswordRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request." });

            if (string.IsNullOrWhiteSpace(request.LoginId) ||
                string.IsNullOrWhiteSpace(request.NewCipherText) ||
                string.IsNullOrWhiteSpace(request.PrivateKey) ||
                string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "LoginId, NewCipherText, PrivateKey and OTP are required." });
            }

            string decryptedLoginId, decryptedNewPassword;
            try
            {
                using RSA rsa = RSA.Create();
                rsa.ImportFromPem(request.PrivateKey.ToCharArray());

                var padding = RSAEncryptionPadding.OaepSHA256;

                decryptedLoginId = DecryptBase64WithRsa(request.LoginId, rsa, padding);
                decryptedNewPassword = DecryptBase64WithRsa(request.NewCipherText, rsa, padding);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Invalid encrypted payload or private key." });
            }

            if (string.IsNullOrWhiteSpace(decryptedLoginId))
                return BadRequest(new { message = "Decrypted LoginId is empty." });

            // Get user by login ID
            var existing = await _repository.GetByLoginIdAsync(decryptedLoginId);
            if (existing == null)
                return NotFound(new { message = "User not found." });

            // Verify OTP before allowing password reset
            var lastOtpRecord = await _otpService.GetLastOtpRecordAsync(existing.MobileNo);
            if (lastOtpRecord == null || lastOtpRecord.Status != "Y" || lastOtpRecord.Otp != request.Otp)
            {
                return BadRequest(new { message = "OTP verification required before password reset." });
            }

            // Check password history (prevent reuse within 30 minutes)
            var recentPasswordChanges = await _repository.GetRecentPasswordHistoryAsync(decryptedLoginId, TimeSpan.FromMinutes(30));
            if (recentPasswordChanges >= 3)
            {
                return BadRequest(new { message = "You have requested password reset more than 3 times. Wait for 30 minutes for new request." });
            }

            // Hash new password
            using (var sha = SHA512.Create())
            {
                var newHashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(decryptedNewPassword));
                var newHashBase64 = Convert.ToHexString(newHashBytes);

                // Update entity
                existing.Password = newHashBase64;
                await _repository.UpdateAsync(existing);

                // Add to password history
                await _repository.AddPasswordHistoryAsync(decryptedLoginId, newHashBase64);
            }

            return Ok(new { message = "Password reset successfully." });
        }
    }

}
