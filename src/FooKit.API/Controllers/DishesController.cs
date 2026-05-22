using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.DTOs.Common;
using MyProject.Application.DTOs.DishDtos;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
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
                return Unauthorized(ApiResponse<object>.Fail("Không xác định được danh tính người dùng."));
            }

            try
            {
                var response = await _dishSuggestionService.GetSuggestionsAsync(userId, request);
                return Ok(ApiResponse<DishSuggestionResponseDto>.Ok(response, "Gợi ý món ăn tối ưu ngân sách thành công."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Lỗi hệ thống: {ex.Message}"));
            }
        }
    }
}
