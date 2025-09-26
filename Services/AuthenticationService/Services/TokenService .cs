using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Models;
using AuthenticationService.DtoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            _expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");
            var secret = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public TokenResult GenerateToken(MasterRegistration user, string role)
        {
            var claims = new List<Claim>
            {
                // Standard and custom claims
                new Claim(JwtRegisteredClaimNames.Sub, user.LoginId ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.RegistrationId.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.EmailId ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.MobileNo ?? string.Empty),
                new Claim(ClaimTypes.Role, role ?? "Applicant")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResult
            {
                Token = tokenString,
                Expires = expires
            };
        }
    }
}
