using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.DishDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;

namespace FooKit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly IDishSuggestionService _dishSuggestionService;

        public DishesController(IDishSuggestionService dishSuggestionService)
        {
            _dishSuggestionService = dishSuggestionService;
        }

        [Authorize]
        [HttpPost("suggest")]
        public async Task<IActionResult> SuggestDishes([FromBody] DishSuggestionRequestDto request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Unable to determine a valid user identity.");
            }

            var response = await _dishSuggestionService.GetSuggestionsAsync(userId, request);
            return Ok(ApiResponse<DishSuggestionResponseDto>.Ok(response, "Budget-optimized dish suggestions retrieved successfully."));
        }
    }
}
