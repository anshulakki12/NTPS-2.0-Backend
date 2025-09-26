using AuthenticationService.DtoModels;
using AuthenticationService.Models;

namespace AuthenticationService.Services
{
    public interface ITokenService
    {
        TokenResult GenerateToken(MasterRegistration user, string role);
    }
}
