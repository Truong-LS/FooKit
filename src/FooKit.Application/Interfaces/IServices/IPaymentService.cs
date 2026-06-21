using FooKit.Application.DTOs.PaymentDtos;
using PayOS.Models.Webhooks;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentAsync(Guid userId, Guid planId);
        Task<PaymentResultDto> ProcessReturnAsync(long orderCode);
        Task<PayOsWebhookResponse> ProcessWebhookAsync(Webhook webhookBody);
    }
}
