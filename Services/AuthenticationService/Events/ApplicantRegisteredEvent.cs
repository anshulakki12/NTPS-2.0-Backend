namespace ApplicantAuthenticationService.Events
{
    public class ApplicantRegisteredEvent
    {
        public string LoginId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RegistrationType { get; set; } = string.Empty;
        public string UserType { get; set; } = "Applicant";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public char IsVerified { get; set; } = 'N';
    }
}
