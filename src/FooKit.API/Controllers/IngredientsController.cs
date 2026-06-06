using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? category = null)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            var result = await _ingredientService.GetIngredientsAsync(page, size, search, category);
            return Ok(ApiResponse<PagedResult<StandardIngredientDto>>.Ok(result, "Standard ingredients list retrieved successfully."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException("Ingredient name is required.");
            }

            var result = await _ingredientService.CreateIngredientAsync(dto);
            return Ok(ApiResponse<StandardIngredientDto>.Ok(result, "Standard ingredient created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient([FromRoute] Guid id, [FromBody] UpdateIngredientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new BadRequestException("Ingredient name is required.");
            }

            var result = await _ingredientService.UpdateIngredientAsync(id, dto);
            return Ok(ApiResponse<StandardIngredientDto>.Ok(result, "Standard ingredient updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient([FromRoute] Guid id)
        {
            var success = await _ingredientService.DeleteIngredientAsync(id);
            if (!success)
            {
                throw new NotFoundException("Standard ingredient not found.");
            }

            return Ok(ApiResponse<object>.Ok(null!, "Standard ingredient deleted successfully."));
        }
    }
}
