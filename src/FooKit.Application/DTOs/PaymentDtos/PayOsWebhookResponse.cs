namespace FooKit.Application.DTOs.PaymentDtos
{
    /// <summary>
    /// Response returned to PayOS after processing a webhook.
    /// PayOS expects an HTTP 200 with a simple success indicator.
    /// </summary>
    public class PayOsWebhookResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
