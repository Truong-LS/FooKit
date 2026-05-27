using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.DTOs.AffiliateProductDtos;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/admin/affiliate-links")]
    [Authorize(Roles = "Admin")]
    public class AffiliateLinksController : ControllerBase
    {
        private readonly IAffiliateLinkService _affiliateLinkService;

        public AffiliateLinksController(IAffiliateLinkService affiliateLinkServiceVal)
        {
            _affiliateLinkService = affiliateLinkServiceVal;
        }

        [HttpGet]
        public async Task<IActionResult> GetAffiliateLinks(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery(Name = "is_active")] bool? isActive = null,
            [FromQuery(Name = "ingredient_id")] Guid? ingredientId = null)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            var result = await _affiliateLinkService.GetAffiliateLinksAsync(page, size, isActive, ingredientId);
            return Ok(ApiResponse<PagedResult<AffiliateLinkDto>>.Ok(result, "Affiliate links retrieved successfully."));
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromRoute] Guid id, [FromBody] ToggleAffiliateStatusDto dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Status payload is required."));
            }

            var success = await _affiliateLinkService.ToggleStatusAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.Fail("Affiliate link not found."));
            }

            return Ok(ApiResponse<object>.Ok(null!, $"Affiliate link status toggled successfully to {(dto.IsActive ? "active" : "inactive")}."));
        }
    }
}
