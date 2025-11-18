using AuthenticationService.DtoModels;
using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    public interface ITokenService
    {
        TokenResult GenerateToken(ApplicantRegistration user, int role);
    }
}
