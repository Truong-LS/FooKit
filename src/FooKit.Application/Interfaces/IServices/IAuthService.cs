using MyProject.Application.DTOs.AuthDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> GoogleLoginAsync(GoogleLoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(TokenRequest request);
        Task<bool> LogoutAsync(string username);
        Task<bool> SetCredentialsAsync(Guid userId, SetCredentialsRequest request);
        Task<bool> LinkGoogleAsync(Guid userId, LinkGoogleRequest request);
    }
}
