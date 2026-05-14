namespace MyProject.Application.DTOs.PaymentDtos
{
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string TransactionRef { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
