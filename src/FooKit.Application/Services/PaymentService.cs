using FooKit.Application.DTOs.PaymentDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.Webhooks;
using PayOS.Models.V2.PaymentRequests;

namespace FooKit.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSClient _payOs;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IConfiguration _configuration;

        public PaymentService(
            IUnitOfWork unitOfWork,
            PayOSClient payOs,
            ISubscriptionService subscriptionService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _payOs = payOs;
            _subscriptionService = subscriptionService;
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentAsync(Guid userId, Guid planId)
        {
            // Kiểm tra gói đăng ký có tồn tại không
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(planId);
            if (plan == null)
                throw new KeyNotFoundException("Không tìm thấy gói đăng ký.");

            // Sinh mã đơn hàng duy nhất
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var payment = new Payment
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                OrderCode = orderCode,
                Amount = plan.Price.Amount,
                OrderInfo = $"Thanh toán gói: {plan.PlanName}",
                Status = PaymentStatus.Pending
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            // Tạo link thanh toán PayOS
            var returnUrl = _configuration["PAYOS_RETURN_URL"] ?? "http://localhost:3000/payment/result";
            var cancelUrl = _configuration["PAYOS_CANCEL_URL"] ?? "http://localhost:3000/payment/cancel";

            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = (int)payment.Amount,
                Description = payment.OrderInfo.Length > 25
                    ? payment.OrderInfo[..25]
                    : payment.OrderInfo,
                Items = [new PaymentLinkItem
                {
                    Name = plan.PlanName,
                    Quantity = 1,
                    Price = (int)plan.Price.Amount
                }],
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            var createPaymentResult = await _payOs.PaymentRequests.CreateAsync(paymentRequest);

            // Lưu payment link ID để tham chiếu sau này
            payment.PaymentLinkId = createPaymentResult.PaymentLinkId;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return createPaymentResult.CheckoutUrl;
        }

        public async Task<PaymentResultDto> ProcessReturnAsync(long orderCode)
        {
            var result = new PaymentResultDto();

            var payment = await _unitOfWork.Payments.GetByOrderCodeAsync(orderCode);
            if (payment == null)
            {
                result.Success = false;
                result.Message = "Không tìm thấy thanh toán.";
                return result;
            }

            if (string.IsNullOrEmpty(payment.PaymentLinkId))
            {
                result.Success = false;
                result.Message = "Thanh toán chưa có PaymentLinkId.";
                return result;
            }

            // Truy vấn PayOS để lấy trạng thái thanh toán mới nhất
            var paymentInfo = await _payOs.PaymentRequests.GetAsync(payment.PaymentLinkId);

            result.TransactionRef = orderCode.ToString();

            if (paymentInfo.Status.ToString().ToUpper() == "PAID")
            {
                result.Success = true;
                result.Message = "Thanh toán thành công.";
            }
            else if (paymentInfo.Status.ToString().ToUpper() == "CANCELLED")
            {
                result.Success = false;
                result.Message = "Thanh toán đã bị hủy.";
            }
            else
            {
                result.Success = false;
                result.Message = $"Trạng thái thanh toán: {paymentInfo.Status}";
            }

            return result;
        }

        public async Task<PayOsWebhookResponse> ProcessWebhookAsync(Webhook webhookBody)
        {
            WebhookData webhookData;
            try
            {
                webhookData = await _payOs.Webhooks.VerifyAsync(webhookBody);
            }
            catch (Exception)
            {
                return new PayOsWebhookResponse
                {
                    Success = false,
                    Message = "Chữ ký webhook không hợp lệ"
                };
            }

            // Test webhook
            if (webhookData.OrderCode == 123)
            {
                return new PayOsWebhookResponse { Success = true, Message = "Test webhook processed" };
            }

            var payment = await _unitOfWork.Payments.GetByOrderCodeAsync(webhookData.OrderCode);
            if (payment == null)
            {
                return new PayOsWebhookResponse
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                return new PayOsWebhookResponse
                {
                    Success = true,
                    Message = "Đơn hàng đã được xác nhận trước đó"
                };
            }

            if (webhookData.Amount != (int)payment.Amount)
            {
                return new PayOsWebhookResponse
                {
                    Success = false,
                    Message = "Số tiền không hợp lệ"
                };
            }

            payment.PayOsTransactionRef = webhookData.Reference;
            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;

            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(payment.SubscriptionPlanId);
            if (plan != null)
            {
                await _subscriptionService.GrantSubscriptionAsync(payment.UserId, plan);
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return new PayOsWebhookResponse
            {
                Success = true,
                Message = "Xử lý webhook thành công"
            };
        }
    }
}
