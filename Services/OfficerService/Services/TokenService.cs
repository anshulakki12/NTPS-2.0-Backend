using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OfficerService.Models;
using OfficerService.DtoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace OfficerService.Services
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

        public TokenResult GenerateToken(OfficerRegistration officer, int roleId)
        {
            var claims = new List<Claim>
            {
                // Standard and custom claims
                new Claim(JwtRegisteredClaimNames.Sub, officer.LoginId ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, officer.OfficerId.ToString()),
                new Claim(ClaimTypes.Name, officer.OfficerDetails?.OfficerName ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, officer.MobileNo ?? string.Empty),
                new Claim(ClaimTypes.Role, roleId.ToString()), // Use roleId as the role claim
                new Claim("OfficerId", officer.OfficerId.ToString()),
                new Claim("LocationId", officer.LocationId.ToString()),
                new Claim("LocationType", officer.LocationType.ToString()),
                new Claim("RoleId", roleId.ToString())
            };

            // Add officer-specific claims if OfficerDetails exists
            if (officer.OfficerDetails != null)
            {
                claims.Add(new Claim("OfficerTitle", officer.OfficerDetails.OfficerTitle ?? string.Empty));
                claims.Add(new Claim("OfficerNumber", officer.OfficerDetails.OfficerNumber ?? string.Empty));
                claims.Add(new Claim("EmailAddress", officer.OfficerDetails.EmailAddress ?? string.Empty));
            }

            // Add role name as a separate claim (optional, for display purposes)
            if (!string.IsNullOrEmpty(officer.Role?.RoleName))
            {
                claims.Add(new Claim("RoleName", officer.Role.RoleName));
            }

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