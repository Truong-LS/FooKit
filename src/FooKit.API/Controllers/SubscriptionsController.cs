using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.SubscriptionDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace FooKit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _subscriptionService.GetAllPlansAsync();
            return Ok(ApiResponse<IEnumerable<SubscriptionPlanDto>>.Ok(plans, "Retrieved subscription plans successfully."));
        }

        [Authorize]
        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine user identity.");
            }

            var subscription = await _subscriptionService.GetCurrentSubscriptionAsync(userId);
            return Ok(ApiResponse<UserSubscriptionDto>.Ok(subscription, "Retrieved current subscription."));
        }

        [Authorize]
        [HttpGet("payment-history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine user identity.");
            }

            var history = await _subscriptionService.GetPaymentHistoryAsync(userId);
            return Ok(ApiResponse<IEnumerable<PaymentHistoryDto>>.Ok(history, "Retrieved payment history."));
        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine user identity.");
            }

            await _subscriptionService.CancelSubscriptionAsync(userId);
            return Ok(ApiResponse<object?>.Ok(null, "Subscription cancelled successfully."));
        }
    }
}
