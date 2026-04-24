using MyProject.Domain.Entities;
using System.Security.Claims;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
