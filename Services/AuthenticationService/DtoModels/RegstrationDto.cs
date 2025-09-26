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
            public string LoginId { get; set; }
            public string Password { get; set; }
            // client can pass desired role (or you may fetch role from DB instead)
            public string? Role { get; set; }
        }
    }
}
