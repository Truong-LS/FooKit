using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.SubscriptionDtos;
using FooKit.Application.Interfaces.IServices;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminSubscriptionPlansController : ControllerBase
    {
        private readonly IAdminSubscriptionPlanService _adminSubscriptionPlanService;

        public AdminSubscriptionPlansController(IAdminSubscriptionPlanService adminSubscriptionPlanService)
        {
            _adminSubscriptionPlanService = adminSubscriptionPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPlans([FromQuery] GetSubscriptionPlansRequestDto request)
        {
            var result = await _adminSubscriptionPlanService.GetSubscriptionPlansAsync(request);
            return Ok(ApiResponse<PagedResult<SubscriptionPlanDto>>.Ok(result, "Subscription plans retrieved successfully."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanDto request)
        {
            var result = await _adminSubscriptionPlanService.CreateSubscriptionPlanAsync(request);
            return CreatedAtAction(nameof(GetSubscriptionPlans), new { id = result.Id }, ApiResponse<SubscriptionPlanDto>.Ok(result, "Subscription plan created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscriptionPlan(Guid id, [FromBody] UpdateSubscriptionPlanDto request)
        {
            var result = await _adminSubscriptionPlanService.UpdateSubscriptionPlanAsync(id, request);
            return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result, "Subscription plan updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriptionPlan(Guid id)
        {
            await _adminSubscriptionPlanService.DeleteSubscriptionPlanAsync(id);
            return Ok(ApiResponse<object>.Ok(null!, "Subscription plan soft-deleted successfully."));
        }
    }
}
