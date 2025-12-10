namespace MasterLoginService.DtoModels
{
    public class LoginRequestDto
    {
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
