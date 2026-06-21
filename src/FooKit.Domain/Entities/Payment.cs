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
        /// Mã đơn hàng số duy nhất gửi tới PayOS (phải là số nguyên dương int64).
        /// </summary>
        public long OrderCode { get; set; }

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
        /// ID liên kết thanh toán PayOS trả về sau khi tạo link thanh toán.
        /// </summary>
        public string? PaymentLinkId { get; set; }

        /// <summary>
        /// Mã tham chiếu giao dịch PayOS từ webhook callback.
        /// </summary>
        public string? PayOsTransactionRef { get; set; }

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
