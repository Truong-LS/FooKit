using System;

namespace FooKit.Domain.Entities
{
    public class UserSubscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual User? User { get; set; }
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}
