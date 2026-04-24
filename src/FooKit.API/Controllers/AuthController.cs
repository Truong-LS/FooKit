using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyProject.Application.DTOs.AuthDtos;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Exceptions;
using System.Security.Claims;

namespace MyProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var isSuccess = await _authService.RegisterAsync(request);

            if (!isSuccess)
            {
                throw new ConflictException("Username or email already exists in the system.");
            }

            return Ok(ApiResponse<object?>.Ok(null, "Registration successful!"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                throw new UnauthenticatedException("Invalid username or password.");
            }

            return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful."));
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var response = await _authService.GoogleLoginAsync(request);

            if (response == null)
            {
                throw new UnauthenticatedException("Invalid Google token.");
            }

            return Ok(ApiResponse<AuthResponse>.Ok(response, "Google login successful."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            if (response == null)
            {
                throw new UnauthenticatedException("Session expired. Please log in again.");
            }

            return Ok(ApiResponse<AuthResponse>.Ok(response, "Token refreshed successfully."));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var isSuccess = await _authService.LogoutAsync(username);

            if (!isSuccess)
            {
                throw new BadRequestException("An error occurred during logout. Please try again.");
            }

            return Ok(ApiResponse<object?>.Ok(null, "Logout successful. All sessions have been invalidated."));
        }

        [Authorize]
        [HttpPut("set-credentials")]
        public async Task<IActionResult> SetCredentials([FromBody] SetCredentialsRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var isSuccess = await _authService.SetCredentialsAsync(userId, request);

            if (!isSuccess)
            {
                throw new BadRequestException("An error occurred while setting credentials. Please try again.");
            }

            return Ok(ApiResponse<object?>.Ok(null, "Credentials set successfully. You can now login with username and password."));
        }

        [Authorize]
        [HttpPost("link-google")]
        public async Task<IActionResult> LinkGoogle([FromBody] LinkGoogleRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var isSuccess = await _authService.LinkGoogleAsync(userId, request);

            if (!isSuccess)
            {
                throw new BadRequestException("An error occurred while linking Google account. Please try again.");
            }

            return Ok(ApiResponse<object?>.Ok(null, "Google account linked successfully."));
        }
    }
}