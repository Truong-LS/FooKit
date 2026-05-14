using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyProject.Application.DTOs.Common;
using MyProject.Application.DTOs.PaymentDtos;
using MyProject.Application.Interfaces.IServices;
using System.Security.Claims;

namespace MyProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Creates a payment request and returns the VNPay payment URL.
        /// The client should redirect the user to this URL.
        /// </summary>
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object?>.Fail("Unable to determine a valid user identity."));
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";

            var paymentUrl = await _paymentService.CreatePaymentAsync(userId, request.PlanId, ipAddress);

            return Ok(ApiResponse<object>.Ok(new { PaymentUrl = paymentUrl }, "Payment URL generated successfully."));
        }

        /// <summary>
        /// VNPay redirects the user to this endpoint after payment.
        /// Validates the payment result and returns the outcome.
        /// </summary>
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var result = await _paymentService.ProcessReturnAsync(queryParams);

            if (result.Success)
            {
                return Ok(ApiResponse<PaymentResultDto>.Ok(result, "Payment completed successfully."));
            }

            return BadRequest(ApiResponse<PaymentResultDto>.Fail(result.Message));
        }

        /// <summary>
        /// VNPay server-to-server IPN callback.
        /// This endpoint must be publicly accessible (no authentication).
        /// </summary>
        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpn()
        {
            var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var response = await _paymentService.ProcessIpnAsync(queryParams);
            return Ok(response);
        }
    }
}
