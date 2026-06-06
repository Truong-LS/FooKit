using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.UserDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;

namespace FooKit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var isSuccess = await _userService.ChangePasswordAsync(userId, request);
            if (!isSuccess)
            {
                throw new BadRequestException("Failed to change password.");
            }

            return Ok(ApiResponse<object?>.Ok(null, "Password changed successfully."));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var updatedProfile = await _userService.UpdateProfileAsync(userId, request);
            if (updatedProfile == null)
            {
                throw new BadRequestException("Failed to update profile.");
            }

            return Ok(ApiResponse<UserProfileResponse>.Ok(updatedProfile, "Profile updated successfully."));
        }
    }
}
