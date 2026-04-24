using AutoMapper;

using Microsoft.Extensions.Configuration;
using MyProject.Application.DTOs.AuthDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using System.Security.Claims;


namespace MyProject.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IGoogleAuthProvider _googleAuthProvider;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtGenerator,
            IConfiguration config,
            IMapper mapper,
            IGoogleAuthProvider googleAuthProvider)
        {
            _unitOfWork = unitOfWork;
            _jwtGenerator = jwtGenerator;
            _config = config;
            _mapper = mapper;
            _googleAuthProvider = googleAuthProvider;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (existingUser != null) return false;

            var newUser = _mapper.Map<User>(request);

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Support login with both Username and Email
            var user = await _unitOfWork.Users.GetByUsernameOrEmailAsync(request.Username);

            if (user == null) return null;

            // Google-only users have no password → cannot login with password
            if (string.IsNullOrEmpty(user.PasswordHash)) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) return null;

            return await GenerateAuthTokensAsync(user);
        }

        public async Task<AuthResponse?> GoogleLoginAsync(GoogleLoginRequest request)
        {
            // 1. Validate Google ID Token via Infrastructure layer
            var payload = await _googleAuthProvider.ValidateTokenAsync(request.IdToken);

            if (payload == null)
            {
                return null;
            }

            var googleId = payload.Value.GoogleId;
            var email = payload.Value.Email;

            // 2. Find UserLogin by Provider + ProviderKey (Scenario B)
            var existingLogin = await _unitOfWork.UserLogins.FindAsync("Google", googleId);

            if (existingLogin?.User != null)
            {
                // Already linked → login directly
                return await GenerateAuthTokensAsync(existingLogin.User);
            }

            // 3. No UserLogin found → check user by Email
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user != null)
            {
                // Scenario: User already has an account (registered with email) → Link Google account
                var newLogin = new UserLogin
                {
                    LoginProvider = "Google",
                    ProviderKey = googleId,
                    ProviderDisplayName = "Google",
                    UserId = user.Id
                };

                await _unitOfWork.UserLogins.AddAsync(newLogin);
                await _unitOfWork.SaveChangesAsync();

                return await GenerateAuthTokensAsync(user);
            }

            // 4. Scenario C: Completely new → Auto-register User + UserLogin
            var newUser = new User
            {
                Username = email,
                Email = email,
                PasswordHash = null
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            var googleLogin = new UserLogin
            {
                LoginProvider = "Google",
                ProviderKey = googleId,
                ProviderDisplayName = "Google",
                UserId = newUser.Id
            };

            await _unitOfWork.UserLogins.AddAsync(googleLogin);
            await _unitOfWork.SaveChangesAsync();

            return await GenerateAuthTokensAsync(newUser);
        }

        public async Task<AuthResponse?> RefreshTokenAsync(TokenRequest request)
        {
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null) return null;

            var userIdString = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId)) return null;

            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, userId);

            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiryDate <= DateTime.UtcNow)
                return null;

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;

            existingToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(existingToken);

            return await GenerateAuthTokensAsync(user);
        }

        public async Task<bool> LogoutAsync(string username)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null) return false;

            var activeTokens = await _unitOfWork.RefreshTokens.GetActiveTokensByUserIdAsync(user.Id);
            if (activeTokens.Any())
            {
                foreach (var token in activeTokens) token.IsRevoked = true;

                _unitOfWork.RefreshTokens.UpdateRange(activeTokens);

                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> SetCredentialsAsync(Guid userId, SetCredentialsRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            // Only Google-only users (no password) can use this API
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new ConflictException("Account already has credentials set.");
            }

            // Check if the new username is already taken
            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new ConflictException("Username already exists.");
            }

            user.Username = request.Username;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LinkGoogleAsync(Guid userId, LinkGoogleRequest request)
        {
            // 1. Validate Google ID Token
            var payload = await _googleAuthProvider.ValidateTokenAsync(request.IdToken);
            if (payload == null)
            {
                throw new BadRequestException("Invalid Google token.");
            }

            var googleId = payload.Value.GoogleId;

            // 2. Check if this Google account is already linked to another user
            var existingLogin = await _unitOfWork.UserLogins.FindAsync("Google", googleId);
            if (existingLogin != null)
            {
                throw new ConflictException("This Google account is already linked to another user.");
            }

            // 3. Check if the current user already has a Google login
            var userLogins = await _unitOfWork.UserLogins.GetByUserIdAsync(userId);
            if (userLogins.Any(ul => ul.LoginProvider == "Google"))
            {
                throw new ConflictException("Your account is already linked to a Google account.");
            }

            // 4. Create new UserLogin record
            var newLogin = new UserLogin
            {
                LoginProvider = "Google",
                ProviderKey = googleId,
                ProviderDisplayName = "Google",
                UserId = userId
            };

            await _unitOfWork.UserLogins.AddAsync(newLogin);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Helper: Creates JWT Access Token + Refresh Token for users.
        /// Used for Login, Google Login, and Refresh Token.
        /// </summary>
        private async Task<AuthResponse> GenerateAuthTokensAsync(User user)
        {
            var accessToken = _jwtGenerator.GenerateAccessToken(user);
            var refreshTokenString = _jwtGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_config["REFRESH_TOKEN_EXPIRE_DAYS"]!)),
                UserId = user.Id
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse(accessToken, refreshTokenString);
        }
    }
}
