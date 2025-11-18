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
    }
}
