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
        private readonly IDishRecipeService _dishRecipeService;

        public DishesController(IDishSuggestionService dishSuggestionService, IDishRecipeService dishRecipeService)
        {
            _dishSuggestionService = dishSuggestionService;
            _dishRecipeService = dishRecipeService;
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

        [Authorize]
        [HttpGet("{dishCacheId:guid}/recipe")]
        public async Task<IActionResult> GetDishRecipe(Guid dishCacheId)
        {
            var recipe = await _dishRecipeService.GetRecipeDetailAsync(dishCacheId);
            return Ok(ApiResponse<DishRecipeDetailDto>.Ok(recipe, "Lấy chi tiết công thức nấu ăn thành công."));
        }
    }
}
