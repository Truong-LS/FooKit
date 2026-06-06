using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FooKit.Application.DTOs.AffiliateProductDtos;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;
using Hangfire;

namespace FooKit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AffiliateLinksController : ControllerBase
    {
        private readonly IAffiliateLinkService _affiliateLinkService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AffiliateLinksController(IAffiliateLinkService affiliateLinkService, IBackgroundJobClient backgroundJobClient)
        {
            _affiliateLinkService = affiliateLinkService;
            _backgroundJobClient = backgroundJobClient;
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
                throw new BadRequestException("Status payload is required.");
            }

            var success = await _affiliateLinkService.ToggleStatusAsync(id, dto);
            if (!success)
            {
                throw new NotFoundException("Affiliate link not found.");
            }

            return Ok(ApiResponse<object>.Ok(null!, $"Affiliate link status toggled successfully to {(dto.IsActive ? "active" : "inactive")}."));
        }

        [HttpPost("sync")]
        public IActionResult TriggerAffiliateSync([FromBody] TriggerAffiliateSyncRequest request)
        {
            _backgroundJobClient.Enqueue<IAffiliateSyncService>(x => x.ManualSyncAsync(request.ForceSyncAll, request.TargetIngredientId));
            return Accepted(ApiResponse<object>.Ok(null, "Affiliate sync job has been enqueued successfully."));
        }
    }
}
