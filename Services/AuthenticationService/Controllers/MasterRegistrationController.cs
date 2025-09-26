using AuthenticationService.DtoModels;
using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tesseract;
using static AuthenticationService.DtoModels.RegstrationDto;
//using Tesseract;
namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterRegistrationController : ControllerBase
    {
        private readonly IMasterRegistrationRepository _repository;
        private readonly string _tessDataPath;
        private readonly IOtpService _otpService;
        private readonly IPasswordService _passwordHelper;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;
        public MasterRegistrationController(
            IMasterRegistrationRepository repository,
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
        [Authorize(Roles = "Applicant")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MasterRegistration registration)
        {
            if (registration == null)
                return BadRequest(new { message = "Invalid data." });

            // Validate mobile
            if (string.IsNullOrWhiteSpace(registration.MobileNo) || !Regex.IsMatch(registration.MobileNo, "^[0-9]{10}$"))
            {
                return BadRequest(new { message = "Invalid mobile number." });
            }

            // ✅ Check duplicate Mobile No before inserting
            var existing = await _repository.GetByLoginIdAsync(registration.MobileNo);
            if (existing != null)
            {
                if (existing.IsVerified == 'Y')
                    return Conflict(new { code = "MOBILE_EXISTS_VERIFIED", message = "Mobile number already registered." });

                if (existing.IsVerified == 'N')
                    return Conflict(new { code = "MOBILE_EXISTS_NOT_VERIFIED", message = "Registration is incomplete for this mobile number." });
            }

            // Validate Email and Password
            if (string.IsNullOrWhiteSpace(registration.EmailId) ||
                string.IsNullOrWhiteSpace(registration.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            // ✅ Hash password before saving
            registration.Password = _passwordHelper.HashPassword(registration.Password);
            registration.CreatedDate = DateTime.UtcNow;

            var created = await _repository.AddAsync(registration);
            return CreatedAtAction(nameof(GetById), new { id = created.RegistrationId }, created);
        }


        //[HttpPost("generate-otp")]
        //public async Task<IActionResult> GenerateOtp([FromBody] string mobileNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(mobileNumber) || !System.Text.RegularExpressions.Regex.IsMatch(mobileNumber, "^[0-9]{10}$"))
        //        return BadRequest(new { message = "Invalid mobile number." });

        //    var otp = new Random().Next(100000, 999999).ToString();
        //    var expiry = DateTime.UtcNow.AddMinutes(1);

        //    // Store OTP temporarily (for example: DB table, in-memory, cache)
        //    await _otpService.SaveOtpAsync(mobileNumber, otp, expiry);

        //    // Send OTP via SMS (mock for now)
        //    Console.WriteLine($"OTP for {mobileNumber}: {otp}");

        //    return Ok(new { message = "OTP sent successfully.", expiry });
        //}

        //[HttpPost("verify-otp")]
        //public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationModel model)
        //{
        //    if (string.IsNullOrWhiteSpace(model.MobileNumber) || string.IsNullOrWhiteSpace(model.Otp))
        //        return BadRequest(new { message = "Mobile number and OTP are required." });

        //    var isValid = await _otpService.ValidateOtpAsync(model.MobileNumber, model.Otp);
        //    if (!isValid)
        //        return BadRequest(new { message = "Invalid or expired OTP." });

        //    return Ok(new { message = "OTP verified successfully." });
        //}



        // POST: api/MasterRegistration/generate-otp
        //[HttpPost("generate-otp")]
        //public IActionResult GenerateOtp([FromBody] string mobileNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(mobileNumber))
        //        return BadRequest(new { message = "Mobile number is required" });

        //    var otp = new Random().Next(100000, 999999).ToString();

        //    // Store OTP in session for verification
        //    HttpContext.Session.SetString("GeneratedOtp", otp);
        //    HttpContext.Session.SetString("OtpMobile", mobileNumber);
        //    HttpContext.Session.SetString("OtpExpiry", DateTime.UtcNow.AddMinutes(5).ToString("o"));

        //    // LOCAL TEST MODE: Return OTP in response for testing without SMS
        //    return Ok(new
        //    {
        //        message = "OTP sent successfully.",
        //        otp, // ⚠️ For local only!
        //        expiry = DateTime.UtcNow.AddMinutes(5)
        //    });
        //}

        // POST: api/MasterRegistration/verify-otp
        //[HttpPost("verify-otp")]
        //public IActionResult VerifyOtp([FromBody] OtpVerificationModel request)
        //{
        //    var storedOtp = HttpContext.Session.GetString("GeneratedOtp");
        //    var storedMobile = HttpContext.Session.GetString("OtpMobile");
        //    var expiryString = HttpContext.Session.GetString("OtpExpiry");

        //    if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(storedMobile) || string.IsNullOrEmpty(expiryString))
        //    {
        //        return BadRequest(new { message = "No OTP found. Please request a new one." });
        //    }

        //    if (DateTime.TryParse(expiryString, out var expiryTime) && DateTime.UtcNow > expiryTime)
        //    {
        //        return BadRequest(new { message = "OTP has expired." });
        //    }

        //    if (request.MobileNumber != storedMobile || request.Otp != storedOtp)
        //    {
        //        return BadRequest(new { message = "Invalid OTP." });
        //    }

        //    return Ok(new { message = "OTP verified successfully." });
        //}
        [Authorize(Roles = "Applicant")]
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

        [Authorize(Roles = "Applicant")]
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

            // Check expiry
            //if (lastOtpRecord.ExpiryDate < DateTime.UtcNow)
            //{
            //    return BadRequest(new { message = "OTP has expired. Please request a new one." });
            //}

            // Check OTP match
            //if (lastOtpRecord.Otp != request.Otp)
            //{
            //    return BadRequest(new { message = "Invalid OTP." });
            //}

            // Mark as verified
            lastOtpRecord.Status = "Y";
            //lastOtpRecord.IsVerified = true; // optional, if in your entity
            lastOtpRecord.UpdatedDate = DateTime.UtcNow;

            await _otpService.UpdateOtpRecordAsync(lastOtpRecord);

            return Ok(new { message = "OTP verified successfully." });
        }




        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
        //        return BadRequest(new { message = "Login ID and Password are required." });

        //    var registration = await _repository.GetByLoginIdAsync(request.LoginId);

        //    if (registration == null)
        //        return Unauthorized(new { message = "Invalid credentials." });

        //    // ✅ Hash entered password
        //    var hashedInput = _passwordHelper.HashPassword(request.Password);

        //    if (registration.Password != hashedInput)
        //        return Unauthorized(new { message = "Invalid credentials." });

        //    return Ok(new { message = "Login successful", user = registration });
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Login ID and Password are required." });

            var registration = await _repository.GetByLoginIdAsync(request.LoginId);

            if (registration == null)
                return Unauthorized(new { message = "Invalid credentials." });

            // Hash entered password (must match how it was saved)
            var hashedInput = _passwordHelper.HashPassword(request.Password);

            if (registration.Password != hashedInput)
                return Unauthorized(new { message = "Invalid credentials." });

            // Role passed in request (or default)
            var role = string.IsNullOrWhiteSpace(request.Role) ? "Applicant" : request.Role;

            // Generate JWT
            var tokenResult = _tokenService.GenerateToken(registration, role);

            // Save session server-side (so server has a record)
            // keep session keys small; store minimal info or a session id pointing to DB if you prefer
            var sessionObj = new
            {
                registration.RegistrationId,
                registration.LoginId,
                registration.Name,
                Role = role,
                Expires = tokenResult.Expires
            };

            HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(sessionObj));

            // Return token to client
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
        [Authorize(Roles = "Applicant")]
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
        [Authorize(Roles = "Applicant")]
        [HttpPost("save")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> Save([FromForm] ApplicantPersonalDetailsDto dto, IFormFile? file)
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

    }

}
