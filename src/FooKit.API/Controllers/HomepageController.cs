using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/Homepage")]
    public class HomepageController : ControllerBase
    {
        private readonly IHomepageSuggestionService _homepageSuggestionService;

        public HomepageController(IHomepageSuggestionService homepageSuggestionService)
        {
            _homepageSuggestionService = homepageSuggestionService;
        }

        [HttpGet("suggestions")]
        [Authorize]
        public async Task<IActionResult> GetSuggestions()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing UserId in token." });
            }

            var suggestions = await _homepageSuggestionService.GetDailySuggestionsAsync(userId);
            return Ok(suggestions);
        }
    }
}
