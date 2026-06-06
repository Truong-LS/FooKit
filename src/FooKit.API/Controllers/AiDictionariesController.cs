using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.AiDictionaryDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AiDictionariesController : ControllerBase
    {
        private readonly IAiDictionaryService _aiDictionaryService;

        public AiDictionariesController(IAiDictionaryService aiDictionaryService)
        {
            _aiDictionaryService = aiDictionaryService;
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
                throw new BadRequestException("New standard ingredient mapping ID is required.");
            }

            var success = await _aiDictionaryService.UpdateMappingAsync(id, dto);
            if (!success)
            {
                throw new NotFoundException("AI dictionary record not found.");
            }

            return Ok(ApiResponse<object>.Ok(null!, "AI dictionary mapping updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeyword([FromRoute] Guid id)
        {
            var success = await _aiDictionaryService.DeleteKeywordAsync(id);
            if (!success)
            {
                throw new NotFoundException("AI dictionary record not found.");
            }

            return Ok(ApiResponse<object>.Ok(null!, "English keyword completely removed from AI dictionary cache."));
        }
    }
}
