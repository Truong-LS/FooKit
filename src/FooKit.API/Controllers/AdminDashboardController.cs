using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
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
                return BadRequest(ApiResponse<object>.Fail("Start date cannot be after end date."));
            }

            var apiUsage = await _dashboardService.GetApiUsageAsync(start, end);
            return Ok(ApiResponse<object>.Ok(apiUsage, "API usage metrics retrieved successfully."));
        }
    }
}
