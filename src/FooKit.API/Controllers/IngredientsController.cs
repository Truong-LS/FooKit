using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.DTOs.Common;
using MyProject.Application.DTOs.IngredientDtos;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/admin/ingredients")]
    [Authorize(Roles = "Admin")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService _ingredientServiceVal)
        {
            _ingredientService = _ingredientServiceVal;
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
                return BadRequest(ApiResponse<object>.Fail("Ingredient name is required."));
            }

            var result = await _ingredientService.CreateIngredientAsync(dto);
            return Ok(ApiResponse<StandardIngredientDto>.Ok(result, "Standard ingredient created successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient([FromRoute] Guid id, [FromBody] UpdateIngredientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(ApiResponse<object>.Fail("Ingredient name is required."));
            }

            try
            {
                var result = await _ingredientService.UpdateIngredientAsync(id, dto);
                return Ok(ApiResponse<StandardIngredientDto>.Ok(result, "Standard ingredient updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient([FromRoute] Guid id)
        {
            try
            {
                var success = await _ingredientService.DeleteIngredientAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.Fail("Standard ingredient not found."));
                }

                return Ok(ApiResponse<object>.Ok(null!, "Standard ingredient deleted successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
