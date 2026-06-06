using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.Common;
using FooKit.Application.Interfaces.IServices;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Domain.Exceptions;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly IAdminUserService _adminUserService;

        public AdminController(
            IAdminDashboardService dashboardService,
            IAdminUserService adminUserService)
        {
            _dashboardService = dashboardService;
            _adminUserService = adminUserService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return Ok(ApiResponse<object>.Ok(overview, "Dashboard overview retrieved successfully."));
        }

        [HttpGet("api-usage")]
        public async Task<IActionResult> GetApiUsage(
            [FromQuery(Name = "start_date")] DateTime? startDate,
            [FromQuery(Name = "end_date")] DateTime? endDate)
        {
            // Default to start of current month if start_date is not specified
            var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            // Default to today if end_date is not specified
            var end = endDate ?? DateTime.UtcNow;

            if (start > end)
            {
                throw new BadRequestException("Start date cannot be after end date.");
            }

            var apiUsage = await _dashboardService.GetApiUsageAsync(start, end);
            return Ok(ApiResponse<object>.Ok(apiUsage, "API usage metrics retrieved successfully."));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequestDto request)
        {
            var result = await _adminUserService.GetUsersAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Users retrieved successfully."));
        }

        [HttpPut("users/{userId}/grant-premium")]
        public async Task<IActionResult> GrantPremium(Guid userId, [FromBody] GrantPremiumRequestDto request)
        {
            var result = await _adminUserService.GrantPremiumAsync(userId, request);
            return Ok(ApiResponse<object>.Ok(result, "Premium subscription granted successfully."));
        }

        [HttpPut("users/{userId}/toggle-ban")]
        public async Task<IActionResult> ToggleBan(Guid userId, [FromBody] ToggleBanRequestDto request)
        {
            await _adminUserService.ToggleBanAsync(userId, request);
            string message = request.IsActive ? "The user account has been unbanned." : "The user account has been banned.";
            return Ok(ApiResponse<object>.Ok(null, message));
        }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromForm] UpdateUserAdminRequestDto request)
        {
            var result = await _adminUserService.UpdateUserAsync(userId, request);
            return Ok(ApiResponse<UserAdminResponseDto>.Ok(result, "User profile updated successfully."));
        }
    }
}
