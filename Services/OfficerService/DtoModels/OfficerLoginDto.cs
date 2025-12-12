namespace OfficerService.DtoModels
{
    public class OfficerLoginDto
    {
        public class LoginRequest
        {
            public string EncryptedUsername { get; set; }
            public string EncryptedPassword { get; set; }
            public string PrivateKey { get; set; }
        }
        public class OfficerDetailsDto
        {
            public int OfficerDetailId { get; set; }
            public string OfficerLoginId { get; set; }
            public string OfficerTitle { get; set; }
            public string OfficerName { get; set; }
            public string OfficerDesignationId { get; set; }
            public string OfficerNumber { get; set; }
            public string EmailAddress { get; set; }
        }

        public class OfficerRegistrationDto
        {
            public int OfficerId { get; set; }
            public string LoginId { get; set; }
            public string MobileNo { get; set; }
            public int LocationId { get; set; }
            public int LocationType { get; set; }
            public bool IsActive { get; set; }
            public int RoleId { get; set; }
            public OfficerDetailsDto OfficerDetails { get; set; }
        }


    }
}
