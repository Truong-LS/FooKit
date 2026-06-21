using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.PaymentDtos;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Exceptions;
using PayOS.Models.Webhooks;
using System.Security.Claims;

namespace FooKit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthenticatedException("Không thể xác định danh tính người dùng hợp lệ.");
            }

            var checkoutUrl = await _paymentService.CreatePaymentAsync(userId, request.PlanId);

            return Ok(ApiResponse<object>.Ok(new { CheckoutUrl = checkoutUrl }, "Tạo link thanh toán thành công."));
        }

        [HttpGet("payos-return")]
        public async Task<IActionResult> PayOsReturn([FromQuery] long orderCode)
        {
            var result = await _paymentService.ProcessReturnAsync(orderCode);

            if (!result.Success)
            {
                throw new BadRequestException(result.Message);
            }

            return Ok(ApiResponse<PaymentResultDto>.Ok(result, "Thanh toán hoàn tất thành công."));
        }

        [HttpPost("payos-webhook")]
        public async Task<IActionResult> PayOsWebhook([FromBody] Webhook webhookBody)
        {
            var response = await _paymentService.ProcessWebhookAsync(webhookBody);
            return Ok(response);
        }
    }
}
