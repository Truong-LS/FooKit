using FooKit.Domain.Entities;
using System.Security.Claims;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
