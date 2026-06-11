using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomepageController : ControllerBase
    {
        private readonly IHomepageSuggestionService _homepageSuggestionService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHomepageCacheSignal _cacheSignal;

        public HomepageController(
            IHomepageSuggestionService homepageSuggestionService,
            IMemoryCache memoryCache,
            IHomepageCacheSignal cacheSignal)
        {
            _homepageSuggestionService = homepageSuggestionService;
            _memoryCache = memoryCache;
            _cacheSignal = cacheSignal;
        }

        [HttpGet("suggestions/breakfast")]
        [Authorize]
        public async Task<IActionResult> GetBreakfastSuggestions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthenticatedException("Invalid or missing UserId in token.");
            }

            var suggestions = await _homepageSuggestionService.GetMealSuggestionsAsync(userId, "breakfast");
            return Ok(ApiResponse<object>.Ok(suggestions, "Breakfast suggestions retrieved successfully."));
        }

        [HttpGet("suggestions/lunch")]
        [Authorize]
        public async Task<IActionResult> GetLunchSuggestions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthenticatedException("Invalid or missing UserId in token.");
            }

            var suggestions = await _homepageSuggestionService.GetMealSuggestionsAsync(userId, "lunch");
            return Ok(ApiResponse<object>.Ok(suggestions, "Lunch suggestions retrieved successfully."));
        }

        [HttpGet("suggestions/dinner")]
        [Authorize]
        public async Task<IActionResult> GetDinnerSuggestions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthenticatedException("Invalid or missing UserId in token.");
            }

            var suggestions = await _homepageSuggestionService.GetMealSuggestionsAsync(userId, "dinner");
            return Ok(ApiResponse<object>.Ok(suggestions, "Dinner suggestions retrieved successfully."));
        }

        [HttpPost("clear-cache")]
        [Authorize(Roles = "Admin")]
        public IActionResult ClearHomepageCache([FromBody] ClearHomepageCacheRequest request)
        {
            if (!string.IsNullOrEmpty(request?.TargetUserId))
            {
                _memoryCache.Remove($"HomepageCache:User_{request.TargetUserId}_breakfast");
                _memoryCache.Remove($"HomepageCache:User_{request.TargetUserId}_lunch");
                _memoryCache.Remove($"HomepageCache:User_{request.TargetUserId}_dinner");
            }
            else
            {
                _cacheSignal.ResetToken();
            }
            
            return Ok(ApiResponse<object>.Ok(null, "Homepage cache cleared successfully. Users will receive newly generated meal recommendations."));
        }
    }
}
