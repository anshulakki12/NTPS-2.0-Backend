namespace AuthenticationService.DtoModels
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }
}
