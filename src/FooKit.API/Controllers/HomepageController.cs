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

        [HttpGet("suggestions")]
        [Authorize]
        public async Task<IActionResult> GetSuggestions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthenticatedException("Invalid or missing UserId in token.");
            }

            var suggestions = await _homepageSuggestionService.GetDailySuggestionsAsync(userId);
            return Ok(ApiResponse<object>.Ok(suggestions, "Homepage suggestions retrieved successfully."));
        }

        [HttpPost("clear-cache")]
        [Authorize(Roles = "Admin")]
        public IActionResult ClearHomepageCache([FromBody] ClearHomepageCacheRequest request)
        {
            if (!string.IsNullOrEmpty(request?.TargetUserId))
            {
                _memoryCache.Remove($"HomepageCache:User_{request.TargetUserId}");
            }
            else
            {
                _cacheSignal.ResetToken();
            }
            
            return Ok(ApiResponse<object>.Ok(null, "Homepage cache cleared successfully. Users will receive newly generated meal recommendations."));
        }
    }
}
