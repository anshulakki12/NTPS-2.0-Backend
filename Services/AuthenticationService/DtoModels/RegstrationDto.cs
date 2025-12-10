namespace AuthenticationService.DtoModels
{
    public class RegstrationDto
    {
        public class OtpVerificationModel
        {
            public string MobileNumber { get; set; }
            public string Otp { get; set; }
        }

        public class LoginRequest
        {
            public string EncryptedUsername { get; set; }
            public string EncryptedPassword { get; set; }
            public string PrivateKey { get; set; }
        }


        public class EncryptedRegistrationRequest
        {
            public string RegistrationType { get; set; }
            public string NameTitle { get; set; }
            public string Name { get; set; }
            public string EmailId { get; set; }
            public string LoginId { get; set; }
            public string CipherText { get; set; }   // ✅ encrypted SHA-512 hash
            public string MobileNo { get; set; }
            public char IsVerified { get; set; }
            public DateTime CreatedDate { get; set; }
            public string PrivateKey { get; set; }   // ✅ RSA private key for decryption
        }

        public class EncryptedUpdatePasswordRequest
        {
            public string LoginId { get; set; }           // encrypted (base64 string)
            public string OldCipherText { get; set; }     // encrypted old password (base64)
            public string NewCipherText { get; set; }     // encrypted new password (base64)
            public string PrivateKey { get; set; }        // PEM encoded private key (string)
        }

        public class CheckLoginIdRequest
        {
            public string LoginId { get; set; } = string.Empty;
        }

        //public class CheckLoginIdResponse
        //{
        //    public bool Exists { get; set; }
        //    public string MobileNo { get; set; } = string.Empty;
        //    public string Role { get; set; } = string.Empty;
        //    public bool IsActive { get; set; }
        //}

        public class ResetPasswordRequest
        {
            public string LoginId { get; set; } = string.Empty;
            public string NewCipherText { get; set; } = string.Empty;
            public string PrivateKey { get; set; } = string.Empty;
            public string Otp { get; set; } = string.Empty;
        }

        public class GenerateOtpRequest
        {
            public string MobileNumber { get; set; } = string.Empty;
            public int CaseId { get; set; } // 1 for registration, 4 for forgot password, etc.
        }

        public class OtpVerificationModels
        {
            public string MobileNumber { get; set; } = string.Empty;
            public string Otp { get; set; } = string.Empty;
            public int CaseId { get; set; }
        }

        public class ForgotPasswordRequest
        {
            public string LoginId { get; set; } = string.Empty;
            public string CaptchaValue { get; set; } = string.Empty;
            public string CaptchaToken { get; set; } = string.Empty;
        }

        public class CheckLoginIdResponse
        {
            public bool Exists { get; set; }
            public bool IsActive { get; set; }
            public string MobileNo { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string LoginId { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class OtpVerificationRequest
        {
            public string MobileNumber { get; set; } = string.Empty;
            public string Otp { get; set; } = string.Empty;
            public string CaseId { get; set; } = "4"; // 4 for forgot password
        }

        public class ApplicantRegisteredEvent
        {
            public string LoginId { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string UserType { get; set; } = "Applicant";
            public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
            public string Name { get; set; }
            public string RegistrationType { get; set; }
        }

        public class UserTypeUpdatedEvent
        {
            public string LoginId { get; set; }
            public string UserType { get; set; }
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        }
    }

}

