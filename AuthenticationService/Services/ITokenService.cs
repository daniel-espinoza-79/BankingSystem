using AuthenticationService.Models;

namespace AuthenticationService.Services;

public interface ITokenService
{
    Task<string> GenerateJwtToken(ApplicationUser user);
}