using OfficerService.DtoModels;
using OfficerService.Models;

namespace OfficerService.Services
{
    public interface ITokenService
    {
        TokenResult GenerateToken(OfficerRegistration officer, int roleId);
    }
}