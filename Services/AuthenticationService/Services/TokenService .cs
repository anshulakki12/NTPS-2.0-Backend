using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Models;
using AuthenticationService.DtoModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ApplicantAuthenticationService.Models;

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

        public TokenResult GenerateToken(ApplicantRegistration applicant, MasterRoles role)
        {
            var claims = new List<Claim>
            {
                // Standard JWT claims
                new Claim(JwtRegisteredClaimNames.Sub, applicant.LoginId ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                
                // Identity claims
                new Claim(ClaimTypes.NameIdentifier, applicant.RegistrationId.ToString()),
                new Claim(ClaimTypes.Name, applicant.Name ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, applicant.MobileNo ?? string.Empty),
                
                // Role claims - multiple ways to access role information
                new Claim(ClaimTypes.Role, role.RoleId.ToString()),
                new Claim("RoleId", role.RoleId.ToString()),
                new Claim("RoleName", role.RoleName),
                new Claim("Roles", role.RoleId.ToString()), // For multiple roles in future
                
                // User-specific claims
                new Claim("OfficerId",  applicant.RegistrationId.ToString()),
                new Claim("LoginId", applicant.LoginId  ?? string.Empty),
                new Claim("UserType", "Applicant"), // For easy identification
                
                // Add role-based flags for frontend
                new Claim("IsAdmin", (role.RoleId == 1).ToString()),
                new Claim("IsStateOfficer", (role.RoleId == 19).ToString()),
                new Claim("IsCircleOfficer", (role.RoleId == 20).ToString()),
                new Claim("IsDivisionOfficer", (role.RoleId == 21).ToString()),
                new Claim("IsRangeOfficer", (role.RoleId == 23).ToString()),
                new Claim("IsApplicant", (role.RoleId == 28).ToString())
            };

            // Add officer-specific claims
            if (applicant != null)
            {
                claims.Add(new Claim("OfficerTitle", applicant.NameTitle ?? string.Empty));
                claims.Add(new Claim("OfficerName", applicant.Name ?? string.Empty));
                claims.Add(new Claim("OfficerNumber", applicant.MobileNo ?? string.Empty));
                claims.Add(new Claim("EmailAddress", applicant.EmailId ?? string.Empty));

            }

            // Add timestamp claims
            claims.Add(new Claim("LoginTime", DateTime.UtcNow.ToString("o")));

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

        // Helper method to validate token and get claims
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = _key,
                ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }

        // Extract role ID from token
        public int? GetRoleIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            var roleIdClaim = principal?.FindFirst("RoleId");

            if (roleIdClaim != null && int.TryParse(roleIdClaim.Value, out int roleId))
            {
                return roleId;
            }

            return null;
        }

        // Check if token has specific role
        public bool TokenHasRole(string token, int roleId)
        {
            var principal = ValidateToken(token);
            var roleClaim = principal?.FindFirst("RoleId");

            return roleClaim != null && roleClaim.Value == roleId.ToString();
        }

        // Get all claims as dictionary
        public Dictionary<string, string> GetTokenClaims(string token)
        {
            var principal = ValidateToken(token);
            var claims = new Dictionary<string, string>();

            if (principal != null)
            {
                foreach (var claim in principal.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }
            }

            return claims;
        }
    }
}
