using System;
using FooKit.Domain.Enums;

namespace FooKit.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid SubscriptionPlanId { get; set; }

        /// <summary>
        /// Unique transaction reference sent to VNPay (vnp_TxnRef).
        /// </summary>
        public string TransactionRef { get; set; } = string.Empty;

        /// <summary>
        /// Payment amount in VND.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the payment order.
        /// </summary>
        public string OrderInfo { get; set; } = string.Empty;

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// VNPay transaction number returned after payment.
        /// </summary>
        public string? VnPayTransactionNo { get; set; }

        /// <summary>
        /// VNPay response code. "00" indicates success.
        /// </summary>
        public string? VnPayResponseCode { get; set; }

        /// <summary>
        /// Bank code used for payment (e.g., NCB, VISA).
        /// </summary>
        public string? BankCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public virtual User? User { get; set; }
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}
