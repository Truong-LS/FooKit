using System;

namespace MyProject.Application.DTOs.SubscriptionDtos
{
    public class PaymentHistoryDto
    {
        public Guid Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionRef { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
