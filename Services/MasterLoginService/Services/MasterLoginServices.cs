using MasterLoginService.DtoModels;
using MasterLoginService.Models;
using MasterLoginService.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MasterLoginService.Services
{
    public class MasterLoginServices : IMasterLoginService
    {
        private readonly IMasterUserRepository _userRepository;
        private readonly IConfiguration _config;

        public MasterLoginServices(IMasterUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        //public async Task<LoginResponseDto> Login(LoginRequestDto request)
        //{
        //    var user = await _userRepository.GetUserByUsername(request.Username);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //    {
        //        throw new Exception("Invalid credentials");
        //    }

        //    var token = GenerateJwtToken(user);

        //    return new LoginResponseDto
        //    {
        //        Token = token,
        //        FullName = user.FullName,
        //        Role = user.Role
        //    };
        //}

        //private string GenerateJwtToken(MasterUser user)
        //{
        //    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim(ClaimTypes.Role, user.Role)
        //    };

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(claims),
        //        Expires = DateTime.UtcNow.AddHours(8),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(key),
        //            SecurityAlgorithms.HmacSha256Signature),
        //        Issuer = _config["Jwt:Issuer"],
        //        Audience = _config["Jwt:Audience"]
        //    };

        //    var handler = new JwtSecurityTokenHandler();
        //    return handler.WriteToken(handler.CreateToken(tokenDescriptor));
        //}
    }
}
