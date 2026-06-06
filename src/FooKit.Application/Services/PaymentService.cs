using FooKit.Application.DTOs.PaymentDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;

namespace FooKit.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayService _vnPayService;

        public PaymentService(IUnitOfWork unitOfWork, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
        }

        public async Task<string> CreatePaymentAsync(Guid userId, Guid planId, string ipAddress)
        {
            // Validate that the subscription plan exists
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(planId);
            if (plan == null)
                throw new KeyNotFoundException("Subscription plan not found.");

            // Generate unique transaction reference
            var transactionRef = DateTime.UtcNow.Ticks.ToString();

            var payment = new Payment
            {
                UserId = userId,
                SubscriptionPlanId = planId,
                TransactionRef = transactionRef,
                Amount = plan.Price.Amount,
                OrderInfo = $"Payment for plan: {plan.PlanName}",
                Status = PaymentStatus.Pending
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            // Generate VNPay payment URL
            var paymentUrl = _vnPayService.CreatePaymentUrl(payment, ipAddress);

            return paymentUrl;
        }

        public async Task<PaymentResultDto> ProcessReturnAsync(IDictionary<string, string> queryParams)
        {
            var result = new PaymentResultDto();

            // Validate signature
            if (!_vnPayService.ValidateCallback(queryParams))
            {
                result.Success = false;
                result.Message = "Invalid signature.";
                return result;
            }

            var responseCode = _vnPayService.GetResponseCode(queryParams);
            var transactionRef = queryParams.TryGetValue("vnp_TxnRef", out var txnRefStr) ? txnRefStr : string.Empty;

            var payment = await _unitOfWork.Payments.GetByTransactionRefAsync(transactionRef);
            if (payment == null)
            {
                result.Success = false;
                result.Message = "Payment not found.";
                return result;
            }

            result.TransactionRef = transactionRef;

            if (responseCode == "00")
            {
                result.Success = true;
                result.Message = "Payment successful.";
            }
            else
            {
                result.Success = false;
                result.Message = $"Payment failed. VNPay response code: {responseCode}";
            }

            return result;
        }

        public async Task<VnPayIpnResponse> ProcessIpnAsync(IDictionary<string, string> queryParams)
        {
            // Validate signature
            if (!_vnPayService.ValidateCallback(queryParams))
            {
                return new VnPayIpnResponse { RspCode = "97", Message = "Invalid signature" };
            }

            var transactionRef = queryParams.TryGetValue("vnp_TxnRef", out var txnRefStr) ? txnRefStr : string.Empty;
            var responseCode = _vnPayService.GetResponseCode(queryParams);
            var vnpTransactionNo = queryParams.TryGetValue("vnp_TransactionNo", out var txnNoStr) ? txnNoStr : string.Empty;
            var bankCode = queryParams.TryGetValue("vnp_BankCode", out var bankCodeStr) ? bankCodeStr : string.Empty;

            var payment = await _unitOfWork.Payments.GetByTransactionRefAsync(transactionRef);
            if (payment == null)
            {
                return new VnPayIpnResponse { RspCode = "01", Message = "Order not found" };
            }

            // Idempotency check — skip if already processed
            if (payment.Status != PaymentStatus.Pending)
            {
                return new VnPayIpnResponse { RspCode = "02", Message = "Order already confirmed" };
            }

            // Verify amount matches (VNPay sends amount * 100)
            var vnpAmountStr = queryParams.TryGetValue("vnp_Amount", out var amtStr) ? amtStr : "0";
            var vnpAmount = long.Parse(vnpAmountStr) / 100;
            if (vnpAmount != (long)payment.Amount)
            {
                return new VnPayIpnResponse { RspCode = "04", Message = "Invalid amount" };
            }

            // Update payment record
            payment.VnPayTransactionNo = vnpTransactionNo;
            payment.VnPayResponseCode = responseCode;
            payment.BankCode = bankCode;

            if (responseCode == "00")
            {
                payment.Status = PaymentStatus.Success;
                payment.PaidAt = DateTime.UtcNow;

                // Activate subscription
                var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(payment.SubscriptionPlanId);
                if (plan != null)
                {
                    var subscription = new UserSubscription
                    {
                        UserId = payment.UserId,
                        PlanId = payment.SubscriptionPlanId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                        IsActive = true
                    };

                    await _unitOfWork.UserSubscriptions.AddAsync(subscription);
                }
            }
            else
            {
                payment.Status = responseCode == "24" ? PaymentStatus.Cancelled : PaymentStatus.Failed;
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return new VnPayIpnResponse { RspCode = "00", Message = "Confirm Success" };
        }
    }
}
