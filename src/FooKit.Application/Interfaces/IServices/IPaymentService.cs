using MyProject.Application.DTOs.PaymentDtos;

namespace MyProject.Application.Interfaces.IServices
{
    /// <summary>
    /// Payment business logic service. Orchestrates VNPay operations and database persistence.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a pending Payment record and generates the VNPay payment URL.
        /// </summary>
        Task<string> CreatePaymentAsync(Guid userId, Guid planId, string ipAddress);

        /// <summary>
        /// Processes the VNPay return redirect. Validates signature and returns payment result.
        /// </summary>
        Task<PaymentResultDto> ProcessReturnAsync(IDictionary<string, string> queryParams);

        /// <summary>
        /// Handles the VNPay IPN (server-to-server callback).
        /// Creates UserSubscription on successful payment.
        /// </summary>
        Task<VnPayIpnResponse> ProcessIpnAsync(IDictionary<string, string> queryParams);
    }
}
