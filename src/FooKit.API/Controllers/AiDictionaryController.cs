using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.DTOs.AiDictionaryDtos;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/admin/ai-dictionary")]
    [Authorize(Roles = "Admin")]
    public class AiDictionaryController : ControllerBase
    {
        private readonly IAiDictionaryService _aiDictionaryService;

        public AiDictionaryController(IAiDictionaryService aiDictionaryServiceVal)
        {
            _aiDictionaryService = aiDictionaryServiceVal;
        }

        [HttpGet]
        public async Task<IActionResult> GetAiDictionary(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery(Name = "search_raw_text")] string? searchRawText = null)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            var result = await _aiDictionaryService.GetAiDictionaryAsync(page, size, searchRawText);
            return Ok(ApiResponse<PagedResult<AiDictionaryDto>>.Ok(result, "AI dictionary cache records retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMapping([FromRoute] Guid id, [FromBody] UpdateAiDictionaryDto dto)
        {
            if (dto == null || dto.NewStandardIngredientId == Guid.Empty)
            {
                return BadRequest(ApiResponse<object>.Fail("New standard ingredient mapping ID is required."));
            }

            try
            {
                var success = await _aiDictionaryService.UpdateMappingAsync(id, dto);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.Fail("AI dictionary record not found."));
                }

                return Ok(ApiResponse<object>.Ok(null!, "AI dictionary mapping updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeyword([FromRoute] Guid id)
        {
            var success = await _aiDictionaryService.DeleteKeywordAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.Fail("AI dictionary record not found."));
            }

            return Ok(ApiResponse<object>.Ok(null!, "English keyword completely removed from AI dictionary cache."));
        }
    }
}
